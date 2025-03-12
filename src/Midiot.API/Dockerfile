FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Midiot.API/Midiot.API.csproj", "Midiot.API/"]
COPY ["Midiot.BL/Midiot.BL.csproj", "Midiot.BL/"]
COPY ["Midiot.Common/Midiot.Common.csproj", "Midiot.Common/"]
COPY ["Midiot.Infrastructure/Midiot.Infrastructure.csproj", "Midiot.Infrastructure/"]
RUN dotnet restore "./Midiot.API/Midiot.API.csproj"
COPY . .
WORKDIR "/src/Midiot.API"
RUN dotnet build "./Midiot.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Midiot.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Midiot.API.dll"]
