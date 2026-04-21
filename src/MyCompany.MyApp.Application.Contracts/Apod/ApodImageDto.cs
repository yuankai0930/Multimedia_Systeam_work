namespace MyCompany.MyApp.Apod;

/// <summary>
/// NASA 每日天文圖片（APOD）的資料傳輸物件。
/// </summary>
public class ApodImageDto
{
    public string Date { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Explanation { get; set; } = null!;
    public string Url { get; set; } = null!;
}
