FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build

# Protocol Buffer Compiler Dependencies
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        autoconf \
        automake \
        curl \
        git \
        g++ \
        libtool \
        make \
        unzip \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /tmp

RUN git clone https://github.com/protocolbuffers/protobuf.git

WORKDIR /tmp/protobuf

RUN git submodule update --init --recursive
RUN ./autogen.sh

RUN ./configure
RUN make
RUN make check
RUN make install
RUN ldconfig

WORKDIR /usr/src/app

COPY ./VendingMachine.Iot.csproj ./

RUN dotnet restore

COPY ./ ./

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_RUNNING_IN_CONTAINER=true

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

# Enable detection of running in a container
ENV DOTNET_RUNNING_IN_CONTAINER=true

WORKDIR /usr/bin/app

COPY --from=build /usr/bin/app ./

CMD ["/bin/bash"]