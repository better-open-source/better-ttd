#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Services/OpenTTD/OpenTTD.API/OpenTTD.API.fsproj", "src/Services/OpenTTD/OpenTTD.API/"]
COPY ["src/Services/OpenTTD/OpenTTD.Network/OpenTTD.Network.fsproj", "src/Services/OpenTTD/OpenTTD.Network/"]
COPY ["src/Services/OpenTTD/OpenTTD.Actors/OpenTTD.Actors.fsproj", "src/Services/OpenTTD/OpenTTD.Actors/"]
RUN dotnet restore "src/Services/OpenTTD/OpenTTD.API/OpenTTD.API.fsproj"
RUN dotnet restore "src/Services/OpenTTD/OpenTTD.Actors/OpenTTD.Actors.fsproj"
RUN dotnet restore "src/Services/OpenTTD/OpenTTD.Network/OpenTTD.Network.fsproj"
COPY . .
WORKDIR "/src/src/Services/OpenTTD"
RUN dotnet build "OpenTTD.API/OpenTTD.API.fsproj" -c Release -o /app/build
RUN dotnet build "OpenTTD.Actors/OpenTTD.Actors.fsproj" -c Release -o /app/build
RUN dotnet build "OpenTTD.Network/OpenTTD.Network.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenTTD.API/OpenTTD.API.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenTTD.API.dll"]
