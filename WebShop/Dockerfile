FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebShop/WebShop.csproj", "WebShop/"]
RUN dotnet restore "WebShop/WebShop.csproj"
COPY . .
WORKDIR "/src/WebShop"
RUN dotnet build "WebShop.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebShop.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebShop.dll"]
