FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy the library project files first
COPY CertificateGeneratorCore/CertificateGeneratorCore.csproj ./CertificateGeneratorCore/
# Copy the main project files
COPY ./CertificateGeneratorApi/CertificateGeneratorApi.csproj ./CertificateGeneratorApi/

# Restore dependencies for both projects
RUN dotnet restore ./CertificateGeneratorApi/CertificateGeneratorApi.csproj

# Copy the rest of the library code, so Docker will cache the restored dependencies.
COPY ../CertificateGeneratorCore ./CertificateGeneratorCore/
# Copy the rest of the main project code
COPY ./CertificateGeneratorApi ./CertificateGeneratorApi/

# Build and publish the main project
WORKDIR /src/CertificateGeneratorApi
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "CertificateGeneratorApi.dll"]
