FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BaltimoreBot/BaltimoreBot.csproj", "BaltimoreBot/"]
RUN dotnet restore "BaltimoreBot/BaltimoreBot.csproj"
COPY . .
WORKDIR "/src/BaltimoreBot"
RUN dotnet build "BaltimoreBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BaltimoreBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BaltimoreBot.dll"]
