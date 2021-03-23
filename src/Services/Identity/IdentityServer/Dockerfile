#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Services/Identity/IdentityServer/IdentityServer.fsproj", "src/Services/Identity/IdentityServer/"]
COPY ["src/Services/Identity/IdentityServer.Migrations/IdentityServer.Migrations.csproj", "src/Services/Identity/IdentityServer.Migrations/"]
RUN dotnet restore "src/Services/Identity/IdentityServer/IdentityServer.fsproj"
COPY . .
WORKDIR "/src/src/Services/Identity/IdentityServer"
RUN dotnet build "IdentityServer.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityServer.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityServer.dll"]