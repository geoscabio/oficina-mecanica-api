FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY OficinaMecanica.sln ./
COPY src/OficinaMecanica.Domain/OficinaMecanica.Domain.csproj src/OficinaMecanica.Domain/
COPY src/OficinaMecanica.Application/OficinaMecanica.Application.csproj src/OficinaMecanica.Application/
COPY src/OficinaMecanica.Infrastructure/OficinaMecanica.Infrastructure.csproj src/OficinaMecanica.Infrastructure/
COPY src/OficinaMecanica.API/OficinaMecanica.API.csproj src/OficinaMecanica.API/

RUN dotnet restore src/OficinaMecanica.API/OficinaMecanica.API.csproj

COPY src/ src/

RUN dotnet publish src/OficinaMecanica.API/OficinaMecanica.API.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["dotnet", "OficinaMecanica.API.dll"]
