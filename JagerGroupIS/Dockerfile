#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JagerGroupIS/JagerGroupIS.csproj", "JagerGroupIS/"]
COPY ["JagerGroupIS.DatabaseContext/JagerGroupIS.DatabaseContext.csproj", "JagerGroupIS.DatabaseContext/"]
COPY ["JagerGroupIS.Models/JagerGroupIS.Models.csproj", "JagerGroupIS.Models/"]
COPY ["JagerGroupIS.DiscordBot/JagerGroupIS.DiscordBot.csproj", "JagerGroupIS.DiscordBot/"]
RUN dotnet restore "./JagerGroupIS/./JagerGroupIS.csproj"
COPY . .
WORKDIR "/src/JagerGroupIS"
RUN dotnet build "./JagerGroupIS.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./JagerGroupIS.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JagerGroupIS.dll"]