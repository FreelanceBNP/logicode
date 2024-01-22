#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["LGC.BNP.MIKUNI.WebSocket/LGC.BNP.MIKUNI.WebSocket.csproj", "LGC.BNP.MIKUNI.WebSocket/"]
RUN dotnet restore "LGC.BNP.MIKUNI.WebSocket/LGC.BNP.MIKUNI.WebSocket.csproj"
COPY . .
WORKDIR "/src/LGC.BNP.MIKUNI.WebSocket"
RUN dotnet build "LGC.BNP.MIKUNI.WebSocket.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LGC.BNP.MIKUNI.WebSocket.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LGC.BNP.MIKUNI.WebSocket.dll"]