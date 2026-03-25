# ---------- BUILD STAGE ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# copy solution first
COPY *.slnx .

# copy project files (for restore caching)
COPY Catalog.API/*.csproj Catalog.API/
COPY Basket.API/*.csproj Basket.API/
COPY Discount.API/*.csproj Discount.API/
COPY Discount.Grpc/*.csproj Discount.Grpc/
COPY Ordering.API/*.csproj Ordering.API/
COPY Ordering.Application/*.csproj Ordering.Application/
COPY Ordering.Domain/*.csproj Ordering.Domain/
COPY Ordering.Infrastructure/*.csproj Ordering.Infrastructure/
COPY EventBus.Messages/*.csproj EventBus.Messages/
COPY BuildingBlocks/BuildingBlocks.csproj BuildingBlocks/
COPY Shopping.Aggregator/*.csproj Shopping.Aggregator/
COPY Identity.API/*.csproj Identity.API/
COPY ApiGateway/*.csproj ApiGateway/

RUN dotnet restore

# copy remaining files
COPY . .

# project argument
ARG PROJECT

WORKDIR /src/${PROJECT}

RUN dotnet publish -c Release -o /app/publish


# ---------- RUNTIME STAGE ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ARG PROJECT
ENV PROJECT=$PROJECT

ENTRYPOINT dotnet ${PROJECT}.dll