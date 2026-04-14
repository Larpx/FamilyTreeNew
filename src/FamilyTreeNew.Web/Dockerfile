# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY ["src/FamilyTreeNew.Web/FamilyTreeNew.Web.csproj", "FamilyTreeNew.Web/"]
COPY ["src/FamilyTreeNew.BLL/FamilyTreeNew.BLL.csproj", "FamilyTreeNew.BLL/"]
COPY ["src/FamilyTreeNew.DAL/FamilyTreeNew.DAL.csproj", "FamilyTreeNew.DAL/"]
COPY ["src/FamilyTreeNew.Models/FamilyTreeNew.Models.csproj", "FamilyTreeNew.Models/"]

RUN dotnet restore "FamilyTreeNew.Web/FamilyTreeNew.Web.csproj"

COPY src/FamilyTreeNew.Web/. FamilyTreeNew.Web/
COPY src/FamilyTreeNew.BLL/. FamilyTreeNew.BLL/
COPY src/FamilyTreeNew.DAL/. FamilyTreeNew.DAL/
COPY src/FamilyTreeNew.Models/. FamilyTreeNew.Models/

WORKDIR "/src/FamilyTreeNew.Web"
RUN dotnet build "FamilyTreeNew.Web.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "FamilyTreeNew.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    && rm -rf /var/lib/apt/lists/*

EXPOSE 5002

ENV ASPNETCORE_URLS=http://+:5002
ENV ASPNETCORE_ENVIRONMENT=Production

RUN mkdir -p /app/DataProtection-Keys

COPY --from=publish /app/publish .

RUN adduser --disabled-password --gecos '' appuser && \
    chown -R appuser:appuser /app
USER appuser

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5002/ || exit 1

ENTRYPOINT ["dotnet", "FamilyTreeNew.Web.dll"]
