FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DivarScraper.Crawler/DivarScraper.Crawler.csproj", "DivarScraper.Crawler/"]
COPY ["DivarScraper.Core/DivarScraper.Core.csproj", "DivarScraper.Core/"]
RUN dotnet restore "DivarScraper.Crawler/DivarScraper.Crawler.csproj"
COPY . .
WORKDIR "/src/DivarScraper.Crawler"
RUN dotnet build "DivarScraper.Crawler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DivarScraper.Crawler.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DivarScraper.Crawler.dll"] 