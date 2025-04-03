FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DivarScraper.Trainer/DivarScraper.Trainer.csproj", "DivarScraper.Trainer/"]
COPY ["DivarScraper.Core/DivarScraper.Core.csproj", "DivarScraper.Core/"]
RUN dotnet restore "DivarScraper.Trainer/DivarScraper.Trainer.csproj"
COPY . .
WORKDIR "/src/DivarScraper.Trainer"
RUN dotnet build "DivarScraper.Trainer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DivarScraper.Trainer.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DivarScraper.Trainer.dll"] 