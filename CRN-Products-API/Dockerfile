FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CRN.sln ./
COPY src/API/CRN.API.csproj src/API/
COPY src/Application/CRN.Application.csproj src/Application/
COPY src/Infrastructure/CRN.Infrastructure.csproj src/Infrastructure/
COPY src/Domain/CRN.Domain.csproj src/Domain/
COPY tests/Application.Tests/CRN.Application.Tests.csproj tests/Application.Tests/
COPY tests/API.Tests/CRN.API.Tests.csproj tests/API.Tests/

RUN dotnet restore src/API/CRN.API.csproj

COPY . .
RUN dotnet publish src/API/CRN.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CRN.API.dll"]
