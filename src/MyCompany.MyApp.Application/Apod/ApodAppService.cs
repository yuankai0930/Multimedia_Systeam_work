using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace MyCompany.MyApp.Apod;

/// <summary>
/// APOD 應用服務：負責從 NASA API 抓取資料並存入資料庫。
/// </summary>
[Authorize]
public class ApodAppService : MyAppAppService, IApodAppService
{
    private const string DefaultNasaApiKey = "DEMO_KEY";
    private const string ApodDateFormat = "yyyy-MM-dd";
    private static readonly DateOnly ApodEarliestDate = new(1995, 6, 16);

    private readonly IRepository<ApodImage, Guid> _apodRepository;
    private readonly IRepository<ApodQueryHistory, Guid> _apodQueryHistoryRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ApodAppService(
        IRepository<ApodImage, Guid> apodRepository,
        IRepository<ApodQueryHistory, Guid> apodQueryHistoryRepository,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _apodRepository = apodRepository;
        _apodQueryHistoryRepository = apodQueryHistoryRepository;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    /// <summary>
    /// 從 NASA API 抓取今日天文圖片並儲存至資料庫。
    /// </summary>
    public async Task<ApodImageDto> FetchAndSaveAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow).ToString(ApodDateFormat, CultureInfo.InvariantCulture);
        return await GetByDateAsync(today);
    }

    /// <summary>
    /// 依指定日期取得 APOD，若資料庫中不存在則從 NASA API 抓取後保存。
    /// </summary>
    public async Task<ApodImageDto> GetByDateAsync(string date)
    {
        var normalizedDate = NormalizeDate(date);
        var entity = await GetOrFetchByDateAsync(normalizedDate);

        await RecordQueryAsync(entity, normalizedDate);

        return MapToDto(entity);
    }

    /// <summary>
    /// 取得資料庫中所有已儲存的天文圖片。
    /// </summary>
    public async Task<List<ApodImageDto>> GetListAsync()
    {
        var list = await _apodRepository.GetListAsync();
        var result = new List<ApodImageDto>();
        foreach (var item in list)
        {
            result.Add(MapToDto(item));
        }
        return result;
    }

    /// <summary>
    /// 取得目前登入使用者的 APOD 查詢歷史。
    /// 排序規則：星號優先（IsStarred=true，按 PinnedOrder ASC），非星號區按 QueryTime DESC。
    /// </summary>
    public async Task<List<ApodQueryHistoryDto>> GetMyHistoryAsync()
    {
        var userId = CurrentUser.GetId();
        var histories = await _apodQueryHistoryRepository.GetListAsync(x => x.UserId == userId);
        var orderedHistories = histories
            .OrderByDescending(x => x.IsStarred)
            .ThenBy(x => x.PinnedOrder)
            .ThenByDescending(x => x.QueryTime)
            .ToList();

        if (orderedHistories.Count == 0)
        {
            return new List<ApodQueryHistoryDto>();
        }

        var imageIds = orderedHistories
            .Select(x => x.ApodImageId)
            .Distinct()
            .ToList();

        var images = await _apodRepository.GetListAsync(x => imageIds.Contains(x.Id));
        var imageMap = images.ToDictionary(x => x.Id);

        return orderedHistories
            .Where(x => imageMap.ContainsKey(x.ApodImageId))
            .Select(x => MapHistoryToDto(x, imageMap[x.ApodImageId]))
            .ToList();
    }

    private async Task<ApodImage> GetOrFetchByDateAsync(string normalizedDate)
    {
        var existing = await _apodRepository.FindAsync(x => x.Date == normalizedDate);
        if (existing != null)
        {
            return existing;
        }

        var nasaData = await FetchApodAsync(normalizedDate);
        var entity = new ApodImage(
            GuidGenerator.Create(),
            nasaData.Date,
            nasaData.Title,
            nasaData.Explanation,
            NormalizeMediaType(nasaData.MediaType, nasaData.Url),
            nasaData.Url
        );

        await _apodRepository.InsertAsync(entity, autoSave: true);

        return entity;
    }

    private async Task<NasaApodResponse> FetchApodAsync(string normalizedDate)
    {
        var apiKey = ResolveNasaApiKey();
        var url = $"https://api.nasa.gov/planetary/apod?api_key={apiKey}&date={Uri.EscapeDataString(normalizedDate)}";

        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(url);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            if (TryGetNoDataReason(response.StatusCode, normalizedDate, json, out var noDataReason))
            {
                throw new UserFriendlyException(noDataReason);
            }

            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                throw new UserFriendlyException("NASA 服務暫時不可用，請稍後再試。");
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw new UserFriendlyException("NASA API 請求次數過多，請稍後再試。");
            }

            throw new HttpRequestException(
                $"NASA APOD API returned {(int)response.StatusCode} ({response.StatusCode}).",
                null,
                response.StatusCode
            );
        }

        return JsonSerializer.Deserialize<NasaApodResponse>(json)
            ?? throw new InvalidOperationException("NASA APOD API returned an invalid payload.");
    }

    private static bool TryGetNoDataReason(HttpStatusCode statusCode, string normalizedDate, string payload, out string reason)
    {
        reason = string.Empty;

        if (statusCode == HttpStatusCode.NotFound)
        {
            reason = $"你選擇的日期 {normalizedDate} 查無 APOD 資料。";
            return true;
        }

        if (string.IsNullOrWhiteSpace(payload))
        {
            reason = $"你選擇的日期 {normalizedDate} 查無 APOD 資料。";
            return false;
        }

        if (payload.Contains("Date must be between", StringComparison.OrdinalIgnoreCase)
            || payload.Contains("out of range", StringComparison.OrdinalIgnoreCase))
        {
            var latestDate = DateOnly.FromDateTime(DateTime.UtcNow).ToString(ApodDateFormat, CultureInfo.InvariantCulture);
            var earliestDate = ApodEarliestDate.ToString(ApodDateFormat, CultureInfo.InvariantCulture);
            reason = $"你選擇的日期超出可查詢範圍，APOD 目前僅支援 {earliestDate} 到 {latestDate}。";
            return true;
        }

        if (payload.Contains("No data available for date", StringComparison.OrdinalIgnoreCase))
        {
            reason = $"你選擇的日期 {normalizedDate} 尚未發布 APOD 內容。";
            return true;
        }

        return false;
    }

    private async Task RecordQueryAsync(ApodImage entity, string normalizedDate)
    {
        var history = new ApodQueryHistory(
            GuidGenerator.Create(),
            CurrentUser.GetId(),
            entity.Id,
            normalizedDate,
            DateTime.UtcNow
        );

        await _apodQueryHistoryRepository.InsertAsync(history, autoSave: true);
    }

    private static ApodImageDto MapToDto(ApodImage entity)
    {
        return new ApodImageDto
        {
            Date = entity.Date,
            Title = entity.Title,
            Explanation = entity.Explanation,
            MediaType = entity.MediaType,
            Url = entity.Url
        };
    }

    private static ApodQueryHistoryDto MapHistoryToDto(ApodQueryHistory history, ApodImage entity)
    {
        return new ApodQueryHistoryDto
        {
            Id = history.Id,
            Date = entity.Date,
            Title = entity.Title,
            Explanation = entity.Explanation,
            MediaType = entity.MediaType,
            Url = entity.Url,
            QueryTime = history.QueryTime,
            IsStarred = history.IsStarred,
            PinnedOrder = history.PinnedOrder
        };
    }

    /// <summary>
    /// 刪除查詢歷史紀錄（硬刪除）。
    /// 只允許刪除自己的紀錄，試圖刪除他人資料會拋出授權例外。
    /// </summary>
    public async Task DeleteHistoryAsync(Guid historyId)
    {
        var history = await _apodQueryHistoryRepository.GetAsync(historyId);
        if (history.UserId != CurrentUser.GetId())
        {
            throw new Volo.Abp.Authorization.AbpAuthorizationException("You can only delete your own history.");
        }

        await _apodQueryHistoryRepository.DeleteAsync(history, autoSave: true);
    }

    /// <summary>
    /// 切換指定歷史的星號狀態。
    /// 當由非星號轉星號時，自動分配 PinnedOrder；轉非星號時清除 PinnedOrder。
    /// </summary>
    public async Task<ApodQueryHistoryDto> ToggleStarredAsync(Guid historyId)
    {
        var history = await _apodQueryHistoryRepository.GetAsync(historyId);
        if (history.UserId != CurrentUser.GetId())
        {
            throw new Volo.Abp.Authorization.AbpAuthorizationException("You can only toggle your own history.");
        }

        history.IsStarred = !history.IsStarred;
        if (!history.IsStarred)
        {
            history.PinnedOrder = null;
        }
        else
        {
            // 若新轉星號，自動分配最大序號 + 1
            var maxOrder = await _apodQueryHistoryRepository.GetQueryableAsync();
            var maxPinnedOrder = maxOrder
                .Where(x => x.UserId == CurrentUser.GetId() && x.IsStarred)
                .Max(x => (int?)x.PinnedOrder) ?? -1;
            history.PinnedOrder = maxPinnedOrder + 1;
        }

        await _apodQueryHistoryRepository.UpdateAsync(history, autoSave: true);

        // 重新查詢以取得完整 DTO（含 ApodImage 資訊）
        var updated = await _apodQueryHistoryRepository.GetAsync(historyId);
        var image = await _apodRepository.GetAsync(updated.ApodImageId);
        return MapHistoryToDto(updated, image);
    }

    /// <summary>
    /// 批次更新星號區歷史的排序順序。
    /// 提供的 ID 順序決定新的 PinnedOrder（由 0 開始）。
    /// </summary>
    public async Task ReorderStarredHistoriesAsync(ReorderStarredHistoriesInput input)
    {
        var starredHistoryIds = input.StarredHistoryIds;
        var userId = CurrentUser.GetId();
        var histories = await _apodQueryHistoryRepository.GetListAsync(
            x => starredHistoryIds.Contains(x.Id) && x.UserId == userId
        );

        // 驗證全部都是星號項
        if (histories.Any(x => !x.IsStarred))
        {
            throw new Volo.Abp.UserFriendlyException("Only starred items can be reordered.");
        }

        // 驗證數量一致
        if (histories.Count != starredHistoryIds.Count)
        {
            throw new Volo.Abp.Authorization.AbpAuthorizationException("Invalid history IDs.");
        }

        // 按提供順序重新分配 PinnedOrder
        for (int i = 0; i < starredHistoryIds.Count; i++)
        {
            var history = histories.FirstOrDefault(x => x.Id == starredHistoryIds[i]);
            if (history != null)
            {
                history.PinnedOrder = i;
            }
        }

        await _apodQueryHistoryRepository.UpdateManyAsync(histories, autoSave: true);
    }

    private static string NormalizeDate(string date)
    {
        if (!DateOnly.TryParseExact(date, ApodDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            throw new UserFriendlyException("日期格式錯誤，請使用 yyyy-MM-dd。", nameof(date));
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (parsedDate < ApodEarliestDate || parsedDate > today)
        {
            var earliestDate = ApodEarliestDate.ToString(ApodDateFormat, CultureInfo.InvariantCulture);
            var latestDate = today.ToString(ApodDateFormat, CultureInfo.InvariantCulture);
            throw new UserFriendlyException($"日期超出可查詢範圍，請選擇 {earliestDate} 到 {latestDate}。", nameof(date));
        }

        return parsedDate.ToString(ApodDateFormat, CultureInfo.InvariantCulture);
    }

    private static string NormalizeMediaType(string? mediaType, string? url)
    {
        var normalized = mediaType?.Trim().ToLowerInvariant();
        if (normalized == "image" || normalized == "video")
        {
            return normalized;
        }

        if (!string.IsNullOrWhiteSpace(url) && Regex.IsMatch(url, @"(youtube\.com|youtu\.be|vimeo\.com)", RegexOptions.IgnoreCase))
        {
            return "video";
        }

        return "image";
    }

    private string ResolveNasaApiKey()
    {
        // Prefer environment variables so secrets don't need to be committed.
        var apiKey = Environment.GetEnvironmentVariable("NASA_API_KEY")
            ?? Environment.GetEnvironmentVariable("Nasa__ApiKey")
            ?? _configuration["Nasa:ApiKey"];

        return string.IsNullOrWhiteSpace(apiKey) ? DefaultNasaApiKey : apiKey;
    }

    /// <summary>
    /// 對應 NASA APOD API 回傳的 JSON 結構。
    /// </summary>
    private class NasaApodResponse
    {
        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("explanation")]
        public string Explanation { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("media_type")]
        public string? MediaType { get; set; }
    }
}
