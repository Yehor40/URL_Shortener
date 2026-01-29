using MediatR;
using Microsoft.AspNetCore.Mvc;
using URL_Shortener.Application.Features.Urls.Queries;

namespace URL_Shortener.Api.Controllers;

public class RedirectController : ControllerBase
{
    private readonly ISender _sender;

    public RedirectController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("/{shortCode}")]
    public async Task<IActionResult> RedirectToOriginal(string shortCode, CancellationToken cancellationToken)
    {
        var shortUrl = await _sender.Send(new GetShortUrlByCodeQuery(shortCode), cancellationToken);
        if (shortUrl == null)
        {
            return NotFound();
        }

        return Redirect(shortUrl.OriginalUrl);
    }
}
