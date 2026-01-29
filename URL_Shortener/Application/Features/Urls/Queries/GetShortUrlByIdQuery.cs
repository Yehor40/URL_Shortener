using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using URL_Shortener.Application.Common.DTOs;
using URL_Shortener.Infrastructure.Persistence;

namespace URL_Shortener.Application.Features.Urls.Queries;

public record GetShortUrlByIdQuery(Guid Id, string? UserId = null, bool IsAdmin = false) : IRequest<ShortUrlDto?>;

public class GetShortUrlByIdQueryHandler : IRequestHandler<GetShortUrlByIdQuery, ShortUrlDto?>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GetShortUrlByIdQueryHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ShortUrlDto?> Handle(GetShortUrlByIdQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ShortUrls
            .AsNoTracking()
            .Include(x => x.CreatedByUser)
            .AsQueryable();

        if (!request.IsAdmin && !string.IsNullOrEmpty(request.UserId))
        {
            query = query.Where(x => x.CreatedByUserId == request.UserId);
        }

        var url = await query.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return _mapper.Map<ShortUrlDto>(url);
    }
}
