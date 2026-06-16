using System;

namespace MyCompany.MyApp.Apod;

/// <summary>
/// 使用者 APOD 查詢歷史資料傳輸物件。
/// </summary>
public class ApodQueryHistoryDto
{
    /// <summary>
    /// 歷史紀錄唯一識別碼，供刪除/星號/排序時使用。
    /// </summary>
    public Guid Id { get; set; }

    public string Date { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Explanation { get; set; } = null!;
    public string MediaType { get; set; } = null!;
    public string Url { get; set; } = null!;
    public DateTime QueryTime { get; set; }

    /// <summary>
    /// 是否標記星號。
    /// </summary>
    public bool IsStarred { get; set; }

    /// <summary>
    /// 星號區的排序順序（IsStarred=true 時有效）。
    /// </summary>
    public int? PinnedOrder { get; set; }
}
