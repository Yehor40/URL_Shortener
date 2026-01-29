ARG BUILD_CONFIGURATION=Release

# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["URL_Shortener/URL_Shortener.csproj", "URL_Shortener/"]
RUN dotnet restore "URL_Shortener/URL_Shortener.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/URL_Shortener"
RUN dotnet build "URL_Shortener.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish Stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "URL_Shortener.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "URL_Shortener.dll"]
