using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Volo.Abp.Application.Services;

namespace MyCompany.MyApp.Apod;

/// <summary>
/// APOD 應用服務介面，定義對外開放的操作。
/// </summary>
public interface IApodAppService : IApplicationService
{
    /// <summary>
    /// 從 NASA API 抓取今日天文圖片並存入資料庫。
    /// </summary>
    Task<ApodImageDto> FetchAndSaveAsync();

    /// <summary>
    /// 依指定日期取得 APOD，若資料庫中不存在則從 NASA API 抓取後保存。
    /// </summary>
    Task<ApodImageDto> GetByDateAsync(string date);

    /// <summary>
    /// 取得資料庫中所有已儲存的天文圖片。
    /// </summary>
    Task<List<ApodImageDto>> GetListAsync();

    /// <summary>
    /// 取得目前登入使用者的 APOD 查詢歷史。
    /// </summary>
    Task<List<ApodQueryHistoryDto>> GetMyHistoryAsync();

    /// <summary>
    /// 刪除指定的查詢歷史紀錄（只能刪除自己的）。
    /// </summary>
    Task DeleteHistoryAsync(Guid historyId);

    /// <summary>
    /// 切換指定歷史的星號狀態。
    /// </summary>
    Task<ApodQueryHistoryDto> ToggleStarredAsync(Guid historyId);

    /// <summary>
    /// 更新星號區歷史項的排序順序（批次操作）。
    /// 需提供星號項的 ID 清單，系統按順序重新分配 PinnedOrder。
    /// </summary>
    Task ReorderStarredHistoriesAsync(List<Guid> starredHistoryIds);
}
