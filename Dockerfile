FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/GasApp.API/GasApp.API.csproj", "GasApp.API/"]
COPY ["src/GasApp.Application/GasApp.Application.csproj", "GasApp.Application/"]
COPY ["src/GasApp.Domain/GasApp.Domain.csproj", "GasApp.Domain/"]
COPY ["src/GasApp.Infrastructure/GasApp.Infrastructure.csproj", "GasApp.Infrastructure/"]
RUN dotnet restore "GasApp.API/GasApp.API.csproj"

COPY src/ .
RUN dotnet publish "GasApp.API/GasApp.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "GasApp.API.dll"]
