FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WypozyczalniaAPI.csproj", "./"]
#COPY ["Database/rental.db", "./"]
RUN dotnet restore "WypozyczalniaAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "WypozyczalniaAPI.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "WypozyczalniaAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WypozyczalniaAPI.dll"]