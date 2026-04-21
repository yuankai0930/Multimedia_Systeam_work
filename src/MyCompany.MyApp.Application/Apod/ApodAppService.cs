using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace MyCompany.MyApp.Apod;

/// <summary>
/// APOD 應用服務：負責從 NASA API 抓取資料並存入資料庫。
/// </summary>
public class ApodAppService : MyAppAppService, IApodAppService
{
    private const string DefaultNasaApiKey = "DEMO_KEY";

    private readonly IRepository<ApodImage, Guid> _apodRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ApodAppService(
        IRepository<ApodImage, Guid> apodRepository,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _apodRepository = apodRepository;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    /// <summary>
    /// 從 NASA API 抓取今日天文圖片並儲存至資料庫。
    /// </summary>
    public async Task<ApodImageDto> FetchAndSaveAsync()
    {
        var apiKey = ResolveNasaApiKey();
        var url = $"https://api.nasa.gov/planetary/apod?api_key={apiKey}";

        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var nasaData = JsonSerializer.Deserialize<NasaApodResponse>(json);

        // 若今天的資料已存在，直接回傳，避免重複儲存
        var existing = await _apodRepository.FindAsync(x => x.Date == nasaData.Date);
        if (existing != null)
        {
            return MapToDto(existing);
        }

        var entity = new ApodImage(
            GuidGenerator.Create(),
            nasaData.Date,
            nasaData.Title,
            nasaData.Explanation,
            nasaData.Url
        );

        await _apodRepository.InsertAsync(entity, autoSave: true);

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

    private static ApodImageDto MapToDto(ApodImage entity)
    {
        return new ApodImageDto
        {
            Date = entity.Date,
            Title = entity.Title,
            Explanation = entity.Explanation,
            Url = entity.Url
        };
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
        public string Date { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("explanation")]
        public string Explanation { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
