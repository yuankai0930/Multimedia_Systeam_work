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
    /// 取得資料庫中所有已儲存的天文圖片。
    /// </summary>
    Task<List<ApodImageDto>> GetListAsync();
}
