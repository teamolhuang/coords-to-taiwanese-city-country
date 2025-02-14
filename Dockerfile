FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["coords-to-taiwanese-city-country.csproj", "./"]
RUN dotnet restore "coords-to-taiwanese-city-country.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "coords-to-taiwanese-city-country.csproj" -c $BUILD_CONFIGURATION -o /app/build -r linux-arm64

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "coords-to-taiwanese-city-country.csproj" -c $BUILD_CONFIGURATION -o /app/publish -r linux-arm64 /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "coords-to-taiwanese-city-country.dll"]
