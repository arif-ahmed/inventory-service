using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InventoryService.Api.Filters;
public class SalesConcurrencyFilter : IAsyncActionFilter
{
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(3);

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!await _semaphore.WaitAsync(0))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status429TooManyRequests);
            return;
        }

        try
        {
            await next();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}

