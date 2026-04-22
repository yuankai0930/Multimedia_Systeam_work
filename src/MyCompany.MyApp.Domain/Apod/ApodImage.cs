using System;
using Volo.Abp.Domain.Entities;

namespace MyCompany.MyApp.Apod;

/// <summary>
/// 代表 NASA 每日天文圖片（APOD）的領域實體。
/// </summary>
public class ApodImage : Entity<Guid>
{
    /// <summary>
    /// 圖片日期（格式：yyyy-MM-dd）
    /// </summary>
    public string Date { get; set; } = null!;

    /// <summary>
    /// 圖片標題
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 圖片說明
    /// </summary>
    public string Explanation { get; set; } = null!;

    /// <summary>
    /// 媒體型態（例如：image、video）
    /// </summary>
    public string MediaType { get; set; } = null!;

    /// <summary>
    /// 圖片或媒體的網址
    /// </summary>
    public string Url { get; set; } = null!;

    // 供 EF Core 使用的無參數建構子
    protected ApodImage() { }

    public ApodImage(Guid id, string date, string title, string explanation, string mediaType, string url)
        : base(id)
    {
        Date = date;
        Title = title;
        Explanation = explanation;
        MediaType = mediaType;
        Url = url;
    }
}
