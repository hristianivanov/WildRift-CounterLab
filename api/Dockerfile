FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY WildRiftCounterLab.Api.slnx ./
COPY WildRiftCounterLab.Api/WildRiftCounterLab.Api.csproj WildRiftCounterLab.Api/
COPY WildRiftCounterLab.Application/WildRiftCounterLab.Application.csproj WildRiftCounterLab.Application/
COPY WildRiftCounterLab.Domain/WildRiftCounterLab.Domain.csproj WildRiftCounterLab.Domain/
COPY WildRiftCounterLab.Infrastructure/WildRiftCounterLab.Infrastructure.csproj WildRiftCounterLab.Infrastructure/
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
