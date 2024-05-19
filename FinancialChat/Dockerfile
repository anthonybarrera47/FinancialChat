# Use the official .NET 8 SDK image as a build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Install curl and netcat-openbsd
RUN apt-get update && apt-get install -y curl netcat-openbsd

# Trust the developer certificates
RUN dotnet dev-certs https --trust

# Copy wait-for-rabbitmq.sh script
COPY wait-for-it.sh ./
RUN chmod +x wait-for-it.sh

# Copy nuget.config and restore as distinct layers
COPY nuget.config ./
COPY . ./
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o out

# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-env /app/wait-for-it.sh ./
COPY appsettings.Container.json ./appsettings.Container.json

# Expose port 80
EXPOSE 80

# Set the entry point
ENV ASPNETCORE_ENVIRONMENT=Container
ENTRYPOINT ["./wait-for-it.sh", "rabbitmq:5672", "--", "dotnet", "FinancialChat.dll"]
