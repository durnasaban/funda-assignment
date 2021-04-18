#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/APIs/TopAgentsApi/TopAgentsApi.csproj", "src/APIs/TopAgentsApi/"]
RUN dotnet restore "src/APIs/TopAgentsApi/TopAgentsApi.csproj"
COPY . .
WORKDIR "/src/src/APIs/TopAgentsApi"
RUN dotnet build "TopAgentsApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TopAgentsApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TopAgentsApi.dll"]