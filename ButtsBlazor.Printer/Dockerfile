#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN apt-get update -y && \
    apt-get install -y python3 libatomic1
RUN dotnet workload install wasm-tools
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./ButtsBlazor.Shared.Client/ButtsBlazor.Shared.Client.csproj", "ButtsBlazor.Shared.Client/"]
COPY ["./ButtsBlazor.Shared/ButtsBlazor.Shared.csproj", "ButtsBlazor.Shared/"]
COPY ["./ButtsBlazor/ButtsBlazor.Printer.csproj", "ButtsBlazor.Printer/"]
RUN dotnet restore "ButtsBlazor.Printer/ButtsBlazor.Printer.csproj"
COPY . .
WORKDIR "/src/ButtsBlazor.Printer"
RUN dotnet build "ButtsBlazor.Printer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "ButtsBlazor.Printer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
WORKDIR /app/wwwroot
COPY --from=build /src/ButtsBlazor/wwwroot .
WORKDIR /app
ENTRYPOINT ["dotnet", "ButtsBlazor.Printer.exe"]