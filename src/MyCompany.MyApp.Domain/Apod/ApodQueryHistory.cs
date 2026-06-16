using System;
using Volo.Abp.Domain.Entities;

namespace MyCompany.MyApp.Apod;

/// <summary>
/// 記錄使用者查詢 APOD 的歷史。
/// </summary>
public class ApodQueryHistory : Entity<Guid>
{
    /// <summary>
    /// 查詢者使用者 Id。
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 對應的 APOD 資料 Id。
    /// </summary>
    public Guid ApodImageId { get; set; }

    /// <summary>
    /// 使用者查詢的 APOD 日期（yyyy-MM-dd）。
    /// </summary>
    public string ApodDate { get; set; } = null!;

    /// <summary>
    /// 查詢發生時間（UTC）。
    /// </summary>
    public DateTime QueryTime { get; set; }

    protected ApodQueryHistory()
    {
    }

    public ApodQueryHistory(Guid id, Guid userId, Guid apodImageId, string apodDate, DateTime queryTime)
        : base(id)
    {
        UserId = userId;
        ApodImageId = apodImageId;
        ApodDate = apodDate;
        QueryTime = queryTime;
    }
}
