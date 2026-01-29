using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using URL_Shortener.Application.Common.DTOs;
using URL_Shortener.Infrastructure.Persistence;

namespace URL_Shortener.Application.Features.Urls.Queries;

public record GetAllShortUrlsQuery(string? UserId = null, bool IsAdmin = false) : IRequest<List<ShortUrlDto>>;

public class GetAllShortUrlsQueryHandler : IRequestHandler<GetAllShortUrlsQuery, List<ShortUrlDto>>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GetAllShortUrlsQueryHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ShortUrlDto>> Handle(GetAllShortUrlsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ShortUrls
            .AsNoTracking()
            .Include(x => x.CreatedByUser)
            .OrderByDescending(x => x.CreatedAtUtc)
            .AsQueryable();

        if (!request.IsAdmin && !string.IsNullOrEmpty(request.UserId))
        {
            query = query.Where(x => x.CreatedByUserId == request.UserId);
        }

        var urls = await query.ToListAsync(cancellationToken);

        return _mapper.Map<List<ShortUrlDto>>(urls);
    }
}
