# En Visual Studio: Click derecho > Agregar > Nuevo elemento > Archivo de texto > Nombrar "Dockerfile" (sin extensión)

# Contenido COPIAR Y PEGAR:
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ImportacionesSusu.csproj", "."]
RUN dotnet restore
COPY . .
WORKDIR "/src"
RUN dotnet build "ImportacionesSusu.csproj" -c Release -o /app/build
RUN dotnet publish "ImportacionesSusu.csproj" -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "ImportacionesSusu.dll"]