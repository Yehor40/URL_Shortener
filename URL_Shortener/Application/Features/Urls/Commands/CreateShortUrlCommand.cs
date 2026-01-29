using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using URL_Shortener.Application.Common.DTOs;
using URL_Shortener.Domain.Entities;
using URL_Shortener.Infrastructure.Persistence;

namespace URL_Shortener.Application.Features.Urls.Commands;

public record CreateShortUrlCommand(string OriginalUrl, string UserId) : IRequest<ShortUrlDto>;

public class CreateShortUrlCommandHandler : IRequestHandler<CreateShortUrlCommand, ShortUrlDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public CreateShortUrlCommandHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ShortUrlDto> Handle(CreateShortUrlCommand request, CancellationToken cancellationToken)
    {
        if (await _context.ShortUrls.AnyAsync(x => x.OriginalUrl == request.OriginalUrl, cancellationToken))
        {
            throw new InvalidOperationException("This URL already exists.");
        }

        var shortUrl = new ShortUrl
        {
            Id = Guid.NewGuid(),
            OriginalUrl = request.OriginalUrl,
            CreatedByUserId = request.UserId,
            CreatedAtUtc = DateTime.UtcNow,
            ShortCode = await GenerateUniqueCodeAsync(cancellationToken)
        };

        _context.ShortUrls.Add(shortUrl);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ShortUrlDto>(shortUrl);
    }

    private async Task<string> GenerateUniqueCodeAsync(CancellationToken cancellationToken)
    {
        var random = new Random();
        while (true)
        {
            var code = new string(Enumerable.Repeat(Alphabet, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            if (!await _context.ShortUrls.AnyAsync(x => x.ShortCode == code, cancellationToken))
            {
                return code;
            }
        }
    }
}
