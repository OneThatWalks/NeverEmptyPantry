#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["NeverEmptyPantry.Api/NeverEmptyPantry.Api.csproj", "NeverEmptyPantry.Api/"]
COPY ["NeverEmptyPantry.Application/NeverEmptyPantry.Application.csproj", "NeverEmptyPantry.Application/"]
COPY ["NeverEmptyPantry.Authorization/NeverEmptyPantry.Authorization.csproj", "NeverEmptyPantry.Authorization/"]
COPY ["NeverEmptyPantry.Common/NeverEmptyPantry.Common.csproj", "NeverEmptyPantry.Common/"]
COPY ["NeverEmptyPantry.Repository/NeverEmptyPantry.Repository.csproj", "NeverEmptyPantry.Repository/"]
RUN dotnet restore "NeverEmptyPantry.Api/NeverEmptyPantry.Api.csproj"
COPY . .
WORKDIR "/src/NeverEmptyPantry.Api"
RUN dotnet build "NeverEmptyPantry.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NeverEmptyPantry.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NeverEmptyPantry.Api.dll"]