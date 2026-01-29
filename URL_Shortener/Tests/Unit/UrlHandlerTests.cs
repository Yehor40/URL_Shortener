using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URL_Shortener.Application.Common.DTOs;
using URL_Shortener.Application.Common.Mappings;
using URL_Shortener.Application.Features.Urls.Commands;
using URL_Shortener.Application.Features.Urls.Queries;
using URL_Shortener.Domain.Entities;
using URL_Shortener.Infrastructure.Persistence;
using Xunit;
using FluentValidation.TestHelper;

namespace URL_Shortener.Tests.Unit;

public class UrlHandlerTests
{
    private readonly IMapper _mapper;

    public UrlHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateCommandHandler_ShouldGenerateShortCodeAndReturnDto()
    {
        // Arrange
        var context = GetDbContext();
        var handler = new CreateShortUrlCommandHandler(context, _mapper);
        var command = new CreateShortUrlCommand("https://google.com", "user1");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<ShortUrlDto>(result);
        Assert.Equal("https://google.com", result.OriginalUrl);
        Assert.Equal(6, result.ShortCode.Length);
    }

    [Fact]
    public async Task GetAllQueryHandler_ShouldReturnDtoList()
    {
        // Arrange
        var context = GetDbContext();
        var user = new ApplicationUser { Id = "u", UserName = "testuser", Email = "test@test.com" };
        context.Users.Add(user);
        context.ShortUrls.Add(new ShortUrl { Id = Guid.NewGuid(), OriginalUrl = "test", ShortCode = "123", CreatedByUserId = "u", CreatedAtUtc = DateTime.UtcNow, CreatedByUser = user });
        await context.SaveChangesAsync();
        
        var handler = new GetAllShortUrlsQueryHandler(context, _mapper);

        // Act
        var result = await handler.Handle(new GetAllShortUrlsQuery(), CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.IsType<ShortUrlDto>(result[0]);
        Assert.Equal("testuser", result[0].CreatedBy);
    }

    [Fact]
    public void CreateShortUrlCommandValidator_ShouldFail_WhenUrlIsInvalid()
    {
        // Arrange
        var validator = new CreateShortUrlCommandValidator();
        var command = new CreateShortUrlCommand("not-a-url", "user1");

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(v => v.OriginalUrl);
    }
}
