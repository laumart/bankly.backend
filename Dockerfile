FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/TestBankly.Api/TestBankly.Api.csproj", "src/TestBankly.Api/"]
RUN dotnet restore "src/TestBankly.Api/TestBankly.Api.csproj"
COPY . .
WORKDIR "/src/src/TestBankly.Api"
RUN dotnet build "TestBankly.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestBankly.Api.csproj" -c Release -o /app/publish

ENV TZ=America/Sao_Paulo
ENV LANG pt-BR
ENV LANGUAGE pt-BR
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestBankly.Api.dll"]