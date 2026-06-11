FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WildRiftCounterLab.Api/WildRiftCounterLab.Api.csproj", "WildRiftCounterLab.Api/"]
COPY ["WildRiftCounterLab.Application/WildRiftCounterLab.Application.csproj", "WildRiftCounterLab.Application/"]
COPY ["WildRiftCounterLab.Domain/WildRiftCounterLab.Domain.csproj", "WildRiftCounterLab.Domain/"]
COPY ["WildRiftCounterLab.Infrastructure/WildRiftCounterLab.Infrastructure.csproj", "WildRiftCounterLab.Infrastructure/"]
RUN dotnet restore "./WildRiftCounterLab.Api/WildRiftCounterLab.Api.csproj"
COPY . .
WORKDIR "/src/WildRiftCounterLab.Api"
RUN dotnet build "./WildRiftCounterLab.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WildRiftCounterLab.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WildRiftCounterLab.Api.dll"]