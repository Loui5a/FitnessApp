# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration). based on Linux distrobution Debian Bookworm
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base 
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project. based on Linux distrobution Debian Bookworm
FROM debian:bookworm-slim AS build 
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./FitnessApp/FitnessApp/FitnessApp.csproj", "FitnessApp/FitnessApp/"]
COPY ["./FitnessApp/FitnessApp.Client/FitnessApp.Client.csproj", "FitnessApp/FitnessApp.Client/"]
RUN wget https://packages.microsoft.com/config/debian/13/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb # adds the Microsoft package signing key to your list of trusted keys and add the package repository
RUN sudo apt-get update && \ sudo apt-get install -y dotnet-sdk-8.0 # install the .NET SDK
RUN sudo apt-get update && \  sudo apt-get install -y aspnetcore-runtime-8.0 # install the ASP.NET Core runtime
RUN dotnet restore "./FitnessApp/FitnessApp/FitnessApp.csproj"
COPY . .
WORKDIR "/src/FitnessApp/FitnessApp"
RUN dotnet build "./FitnessApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FitnessApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FitnessApp.dll"]