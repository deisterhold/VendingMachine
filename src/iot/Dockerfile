FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build

WORKDIR /usr/src/app

COPY ./Vending.Iot.csproj ./

RUN dotnet restore

COPY ./ ./

RUN dotnet publish -c Release -o /usr/bin/app -r linux-arm

FROM balenalib/raspberrypi3-debian

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        ca-certificates \
        \
# .NET Core dependencies
        libc6 \
        libgcc1 \
        libgssapi-krb5-2 \
        libicu63 \
        libssl1.1 \
        libstdc++6 \
        zlib1g \
    && rm -rf /var/lib/apt/lists/*

# Configure web servers to bind to port 80 when present
ENV ASPNETCORE_URLS=http://+:8080

# Enable detection of running in a container
ENV DOTNET_RUNNING_IN_CONTAINER=true

WORKDIR /usr/bin/app

COPY --from=build /usr/bin/app ./

CMD ["/usr/bin/app/Vending.Iot"]
#CMD ["/bin/bash"]