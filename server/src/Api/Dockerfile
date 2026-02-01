FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:d68a5e260330b659f7eae596a255bddbdc4e406e3579eb2d85d718ac58dd7dcb AS build
WORKDIR /app

COPY . ./
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0@sha256:1aacc8154bc3071349907dae26849df301188be1a2e1f4560b903fb6275e481a
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 5180
ENTRYPOINT ["dotnet", "Api.dll"]