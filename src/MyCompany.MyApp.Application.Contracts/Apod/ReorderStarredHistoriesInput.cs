using System;
using System.Collections.Generic;

namespace MyCompany.MyApp.Apod;

/// <summary>
/// 批次重排星號歷史時使用的輸入模型。
/// </summary>
public class ReorderStarredHistoriesInput
{
    public List<Guid> StarredHistoryIds { get; set; } = [];
}