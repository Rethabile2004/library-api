FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY LibraryApi/*.csproj ./
RUN dotnet restore

COPY LibraryApi/ ./
RUN rm -f appsettings.Development.json
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "LibraryApi.dll"]
