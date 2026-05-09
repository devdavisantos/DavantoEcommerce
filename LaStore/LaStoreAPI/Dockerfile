FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base

WORKDIR /app

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /src

COPY . .

RUN dotnet publish -c Release -o /app/publish \
    /p:UseAppHost=false

FROM base

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "LaStoreAPI.dll"]