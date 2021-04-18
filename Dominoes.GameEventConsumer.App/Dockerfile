FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Domionoes.GameEventConsumer.App/Domionoes.GameEventConsumer.App.csproj", "Domionoes.GameEventConsumer.App/"]
RUN dotnet restore "Domionoes.GameEventConsumer.App/Domionoes.GameEventConsumer.App.csproj"
COPY . .
WORKDIR "/src/Domionoes.GameEventConsumer.App"
RUN dotnet build "Domionoes.GameEventConsumer.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Domionoes.GameEventConsumer.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Domionoes.GameEventConsumer.App.dll"]
