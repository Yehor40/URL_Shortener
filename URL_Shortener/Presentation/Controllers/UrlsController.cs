using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using URL_Shortener.Application.Features.Urls.Commands;
using URL_Shortener.Application.Features.Urls.Queries;

namespace URL_Shortener.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrlsController : ControllerBase
{
    private readonly ISender _sender;

    public UrlsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var urls = await _sender.Send(new GetAllShortUrlsQuery(userId, isAdmin), cancellationToken);
        return Ok(urls);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var url = await _sender.Send(new GetShortUrlByIdQuery(id, userId, isAdmin), cancellationToken);
        if (url == null) return NotFound();

        return Ok(url);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateUrlRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _sender.Send(new CreateShortUrlCommand(request.OriginalUrl, userId), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var success = await _sender.Send(new DeleteShortUrlCommand(id, userId!, isAdmin), cancellationToken);
        if (!success) return Forbid();

        return NoContent();
    }
}

public class CreateUrlRequest
{
    public string OriginalUrl { get; set; } = string.Empty;
}
