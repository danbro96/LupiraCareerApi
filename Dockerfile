# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Copy both project files before restore (the host references Core) so layer caching works.
COPY src/LupiraCareerApi/LupiraCareerApi.csproj src/LupiraCareerApi/
COPY src/LupiraCareerApi.Core/LupiraCareerApi.Core.csproj src/LupiraCareerApi.Core/
RUN dotnet restore src/LupiraCareerApi/LupiraCareerApi.csproj
COPY . .
RUN dotnet publish src/LupiraCareerApi/LupiraCareerApi.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
# curl: compose healthcheck (curl -fsS .../readyz).
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "LupiraCareerApi.dll"]
