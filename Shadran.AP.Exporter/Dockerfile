FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:b27b1354af00b7d4c922d74084f5c8a5cbf51f96de9ab855812bf17cbf176dd2 AS build
WORKDIR /Shadran.AP.Exporter

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0@sha256:312b67256e903c2cc175e5390245dd77c427527a20e83c6e0389e6e1e8e5a033
WORKDIR /Shadran.AP.Exporter
COPY --from=build /Shadran.AP.Exporter/out .
ENTRYPOINT ["dotnet", "Shadran.AP.Exporter.dll"]