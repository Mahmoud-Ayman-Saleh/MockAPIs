FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the solution file and restore dependencies
COPY MockAPIs.sln ./
COPY MockAPIs.API/MockAPIs.API.csproj MockAPIs.API/
COPY MockAPIs.BLL/MockAPIs.BLL.csproj MockAPIs.BLL/
COPY MockAPIs.DAL/MockAPIs.DAL.csproj MockAPIs.DAL/

RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build and publish the API
WORKDIR /src/MockAPIs.API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Generate the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 8080 (the default for .NET 8+ containers)
EXPOSE 8080

ENTRYPOINT ["dotnet", "MockAPIs.API.dll"]
