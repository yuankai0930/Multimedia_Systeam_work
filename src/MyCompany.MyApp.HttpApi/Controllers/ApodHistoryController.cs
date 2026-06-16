using System;
using System.Threading.Tasks;
using MyCompany.MyApp.Apod;
using Microsoft.AspNetCore.Mvc;

namespace MyCompany.MyApp.Controllers;

[Route("api/app/apod/history")]
public class ApodHistoryController : MyAppController
{
    private readonly IApodAppService _apodAppService;

    public ApodHistoryController(IApodAppService apodAppService)
    {
        _apodAppService = apodAppService;
    }

    [HttpDelete("{historyId}")]
    public Task DeleteHistoryAsync(Guid historyId)
    {
        return _apodAppService.DeleteHistoryAsync(historyId);
    }

    [HttpPost("{historyId}/toggle-starred")]
    public Task<ApodQueryHistoryDto> ToggleStarredAsync(Guid historyId)
    {
        return _apodAppService.ToggleStarredAsync(historyId);
    }

    [HttpPost("reorder-starred")]
    public Task ReorderStarredHistoriesAsync([FromBody] ReorderStarredHistoriesInput input)
    {
        return _apodAppService.ReorderStarredHistoriesAsync(input);
    }
}