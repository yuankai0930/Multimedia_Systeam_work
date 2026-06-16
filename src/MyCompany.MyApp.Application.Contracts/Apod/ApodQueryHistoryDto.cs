using System;

namespace MyCompany.MyApp.Apod;

/// <summary>
/// 使用者 APOD 查詢歷史資料傳輸物件。
/// </summary>
public class ApodQueryHistoryDto
{
    public string Date { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Explanation { get; set; } = null!;
    public string MediaType { get; set; } = null!;
    public string Url { get; set; } = null!;
    public DateTime QueryTime { get; set; }
}
