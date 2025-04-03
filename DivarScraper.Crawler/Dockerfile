FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["DivarScraper.Crawler/DivarScraper.Crawler.csproj", "DivarScraper.Crawler/"]
COPY ["DivarScraper.Core/DivarScraper.Core.csproj", "DivarScraper.Core/"]
RUN dotnet restore "DivarScraper.Crawler/DivarScraper.Crawler.csproj"
COPY . .
WORKDIR "/src/DivarScraper.Crawler"
RUN dotnet build "DivarScraper.Crawler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DivarScraper.Crawler.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# نصب Chrome و ChromeDriver
RUN apt-get update && apt-get install -y \
    wget \
    gnupg \
    unzip \
    && wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list \
    && apt-get update \
    && apt-get install -y google-chrome-stable \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DivarScraper.Crawler.dll"] 