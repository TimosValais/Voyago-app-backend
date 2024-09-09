#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Voyago.Auth.API/Voyago.Auth.API.csproj", "Voyago.Auth.API/"]
COPY ["Voyago.App.Contracts/Voyago.App.Contracts.csproj", "Voyago.App.Contracts/"]
COPY ["Voyago.Auth.BusinessLogic/Voyago.Auth.BusinessLogic.csproj", "Voyago.Auth.BusinessLogic/"]
COPY ["Voyago.Auth.DataAccessLayer/Voyago.Auth.DataAccessLayer.csproj", "Voyago.Auth.DataAccessLayer/"]
RUN dotnet restore "./Voyago.Auth.API/Voyago.Auth.API.csproj"
COPY . .
WORKDIR "/src/Voyago.Auth.API"
RUN dotnet build "./Voyago.Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Voyago.Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Voyago.Auth.API.dll"]