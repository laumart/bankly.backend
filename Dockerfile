FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["bari.api.mensageria/bari.api.mensageria.csproj", "bari.api.mensageria/"]
RUN dotnet restore "bari.api.mensageria/bari.api.mensageria.csproj"
COPY . .
WORKDIR "/src/bari.api.mensageria"
RUN dotnet build "bari.api.mensageria.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "bari.api.mensageria.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "bari.api.mensageria.dll"]