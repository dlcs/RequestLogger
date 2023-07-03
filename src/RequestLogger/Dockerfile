FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RequestLogger/RequestLogger.csproj", "RequestLogger/"]
RUN dotnet restore "RequestLogger/RequestLogger.csproj"
COPY . .
WORKDIR "/src/RequestLogger"
RUN dotnet build "RequestLogger.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RequestLogger.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RequestLogger.dll"]
