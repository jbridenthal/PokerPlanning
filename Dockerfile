#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PokerPlanning/Server/PokerPlanning.Server.csproj", "PokerPlanning/Server/"]
COPY ["PokerPlanning/Shared/PokerPlanning.Shared.csproj", "PokerPlanning/Shared/"]
COPY ["PokerPlanning/Client/PokerPlanning.Client.csproj", "PokerPlanning/Client/"]
RUN dotnet restore "PokerPlanning/Server/PokerPlanning.Server.csproj"
COPY . .
WORKDIR "/src/PokerPlanning/Server"
RUN dotnet build "PokerPlanning.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PokerPlanning.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PokerPlanning.Server.dll"]
