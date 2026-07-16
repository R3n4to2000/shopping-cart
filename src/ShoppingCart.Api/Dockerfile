FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["src/ShoppingCart.Api/ShoppingCart.Api.csproj", "src/ShoppingCart.Api/"]
COPY ["src/ShoppingCart.Application/ShoppingCart.Application.csproj", "src/ShoppingCart.Application/"]
COPY ["src/ShoppingCart.Domain/ShoppingCart.Domain.csproj", "src/ShoppingCart.Domain/"]
COPY ["src/ShoppingCart.Infrastructure/ShoppingCart.Infrastructure.csproj", "src/ShoppingCart.Infrastructure/"]

RUN dotnet restore "src/ShoppingCart.Api/ShoppingCart.Api.csproj"

COPY . .

WORKDIR "/src/src/ShoppingCart.Api"

RUN dotnet publish "ShoppingCart.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ShoppingCart.Api.dll"]