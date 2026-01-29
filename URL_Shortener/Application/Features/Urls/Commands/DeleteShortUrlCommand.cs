using MediatR;
using URL_Shortener.Infrastructure.Persistence;

namespace URL_Shortener.Application.Features.Urls.Commands;

public record DeleteShortUrlCommand(Guid Id, string UserId, bool IsAdmin) : IRequest<bool>;

public class DeleteShortUrlCommandHandler : IRequestHandler<DeleteShortUrlCommand, bool>
{
    private readonly AppDbContext _context;

    public DeleteShortUrlCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteShortUrlCommand request, CancellationToken cancellationToken)
    {
        var shortUrl = await _context.ShortUrls.FindAsync(new object[] { request.Id }, cancellationToken);
        if (shortUrl == null) return false;

        if (!request.IsAdmin && shortUrl.CreatedByUserId != request.UserId)
        {
            return false;
        }

        _context.ShortUrls.Remove(shortUrl);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
