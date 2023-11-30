#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN dotnet workload install wasm-tools
RUN apt-get update -y && \
    apt-get install -y python3 libatomic1
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./ButtsBlazor/ButtsBlazor.Server.csproj", "ButtsBlazor/"]
COPY ["./ButtsBlazor.Client/ButtsBlazor.Client.csproj", "ButtsBlazor.Client/"]
RUN dotnet restore "ButtsBlazor/ButtsBlazor.Server.csproj"
COPY . .
WORKDIR "/src/ButtsBlazor"
RUN dotnet build "ButtsBlazor.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "ButtsBlazor.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ButtsBlazor.Server.dll"]