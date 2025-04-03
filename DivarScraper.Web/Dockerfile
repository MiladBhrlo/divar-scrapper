FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DivarScraper.Web/DivarScraper.Web.csproj", "DivarScraper.Web/"]
COPY ["DivarScraper.Core/DivarScraper.Core.csproj", "DivarScraper.Core/"]
RUN dotnet restore "DivarScraper.Web/DivarScraper.Web.csproj"
COPY . .
WORKDIR "/src/DivarScraper.Web"
RUN dotnet build "DivarScraper.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DivarScraper.Web.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DivarScraper.Web.dll"]