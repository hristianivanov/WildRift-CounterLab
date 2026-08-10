FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY WildRiftCounterLab.Api.slnx ./
COPY WildRiftCounterLab.Api/WildRiftCounterLab.Api.csproj WildRiftCounterLab.Api/
COPY WildRiftCounterLab.Services/WildRiftCounterLab.Services.csproj WildRiftCounterLab.Services/
COPY WildRiftCounterLab.Data/WildRiftCounterLab.Data.csproj WildRiftCounterLab.Data/
COPY WildRiftCounterLab.Infrastructure/WildRiftCounterLab.Infrastructure.csproj WildRiftCounterLab.Infrastructure/
COPY WildRiftCounterLab.Common/WildRiftCounterLab.Common.csproj WildRiftCounterLab.Common/
COPY WildRiftCounterLab.Contracts/WildRiftCounterLab.Contracts.csproj WildRiftCounterLab.Contracts/
COPY WildRiftCounterLab.Data.Models/WildRiftCounterLab.Data.Models.csproj WildRiftCounterLab.Data.Models/
COPY WildRiftCounterLab.Engine/WildRiftCounterLab.Engine.csproj WildRiftCounterLab.Engine/
COPY WildRiftCounterLab.Services.Models/WildRiftCounterLab.Services.Models.csproj WildRiftCounterLab.Services.Models/
RUN dotnet restore WildRiftCounterLab.Api/WildRiftCounterLab.Api.csproj

COPY . .
RUN dotnet publish WildRiftCounterLab.Api/WildRiftCounterLab.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
CMD ["sh", "-c", "dotnet WildRiftCounterLab.Api.dll --urls http://0.0.0.0:${PORT:-8080}"]
