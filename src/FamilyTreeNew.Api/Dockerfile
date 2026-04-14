# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY ["src/FamilyTreeNew.Api/FamilyTreeNew.Api.csproj", "FamilyTreeNew.Api/"]
COPY ["src/FamilyTreeNew.BLL/FamilyTreeNew.BLL.csproj", "FamilyTreeNew.BLL/"]
COPY ["src/FamilyTreeNew.DAL/FamilyTreeNew.DAL.csproj", "FamilyTreeNew.DAL/"]
COPY ["src/FamilyTreeNew.Models/FamilyTreeNew.Models.csproj", "FamilyTreeNew.Models/"]

RUN dotnet restore "FamilyTreeNew.Api/FamilyTreeNew.Api.csproj"

COPY src/FamilyTreeNew.Api/. FamilyTreeNew.Api/
COPY src/FamilyTreeNew.BLL/. FamilyTreeNew.BLL/
COPY src/FamilyTreeNew.DAL/. FamilyTreeNew.DAL/
COPY src/FamilyTreeNew.Models/. FamilyTreeNew.Models/

WORKDIR "/src/FamilyTreeNew.Api"
RUN dotnet build "FamilyTreeNew.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "FamilyTreeNew.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    && rm -rf /var/lib/apt/lists/*

EXPOSE 5000
EXPOSE 5001

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

RUN mkdir -p /app/wwwroot/uploads/photos /app/wwwroot/uploads/thumbnails /app/wwwroot/backups /app/DataProtection-Keys

COPY --from=publish /app/publish .

RUN adduser --disabled-password --gecos '' appuser && \
    chown -R appuser:appuser /app
USER appuser

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/api/system/health || exit 1

ENTRYPOINT ["dotnet", "FamilyTreeNew.Api.dll"]
