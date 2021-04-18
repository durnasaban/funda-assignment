#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/CachingObjects/CachingObjectsWorkerService/CachingObjectsWorkerService.csproj", "src/CachingObjects/CachingObjectsWorkerService/"]
RUN dotnet restore "src/CachingObjects/CachingObjectsWorkerService/CachingObjectsWorkerService.csproj"
COPY . .
WORKDIR "/src/src/CachingObjects/CachingObjectsWorkerService"
RUN dotnet build "CachingObjectsWorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CachingObjectsWorkerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CachingObjectsWorkerService.dll"]
