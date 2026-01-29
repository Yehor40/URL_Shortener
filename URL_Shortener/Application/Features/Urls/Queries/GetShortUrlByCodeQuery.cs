using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using URL_Shortener.Application.Common.DTOs;
using URL_Shortener.Infrastructure.Persistence;

namespace URL_Shortener.Application.Features.Urls.Queries;

public record GetShortUrlByCodeQuery(string Code) : IRequest<ShortUrlDto?>;

public class GetShortUrlByCodeQueryHandler : IRequestHandler<GetShortUrlByCodeQuery, ShortUrlDto?>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GetShortUrlByCodeQueryHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ShortUrlDto?> Handle(GetShortUrlByCodeQuery request, CancellationToken cancellationToken)
    {
        var url = await _context.ShortUrls
            .Include(x => x.CreatedByUser)
            .FirstOrDefaultAsync(x => x.ShortCode == request.Code, cancellationToken);

        return _mapper.Map<ShortUrlDto>(url);
    }
}
