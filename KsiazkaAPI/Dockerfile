FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["KsiazkaAPI.csproj", "./"]
RUN dotnet restore "KsiazkaAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "KsiazkaAPI.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "KsiazkaAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KsiazkaAPI.dll"]