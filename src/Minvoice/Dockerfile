## Build Minvoice
FROM mcr.microsoft.com/dotnet/sdk:6.0.100-rc.1-focal-amd64 as build

WORKDIR /source

# copy csproj and restore as distinct layers
COPY src/Minvoice/Minvoice.csproj .
COPY nuget.config .
RUN dotnet restore --runtime ubuntu.20.04-x64

# copy everything else and build app
COPY ./src/Minvoice .
RUN dotnet publish --runtime ubuntu.20.04-x64 -c Release -o /publish --self-contained true --no-restore

FROM ubuntu:focal

# === INSTALL Node.js ===

# Install node14
RUN apt-get update && apt-get install -y curl && \
    curl -sL https://deb.nodesource.com/setup_14.x | bash - && \
    apt-get install -y nodejs

# Feature-parity with node.js base images.
RUN apt-get update && apt-get install -y --no-install-recommends git ssh && \
    npm install -g yarn

# Install Python 3.8

RUN apt-get update && apt-get install -y python3.8 python3-pip && \
    update-alternatives --install /usr/bin/pip pip /usr/bin/pip3 1 && \
    update-alternatives --install /usr/bin/python python /usr/bin/python3 1 && \
    update-alternatives --install /usr/bin/python3 python3 /usr/bin/python3.8 1

# === BAKE BROWSERS INTO IMAGE ===

ENV PLAYWRIGHT_BROWSERS_PATH=/ms-playwright

# 2. Install playwright and then delete the installation.
#    Browsers will remain downloaded in `/ms-playwright`.
#    Note: make sure to set 777 to the registry so that any user can access
#    registry.
RUN mkdir /ms-playwright && \
    mkdir /tmp/pw && cd /tmp/pw && npm init -y && \
    # Install chromium specific package from npm
    npm i playwright-chromium && \
    DEBIAN_FRONTEND=noninteractive npx playwright install-deps chromium && \
    rm -rf /tmp/pw && \
    chmod -R 777 /ms-playwright

# Use non-root user
RUN addgroup --group dotnetuser --gid 2000 \
    && adduser \    
        --uid 1000 \
        --gid 2000 \
        --disabled-password \
        --home /app \
        --gecos '' \
        "dotnetuser" 

# Add Minvoice
COPY --from=build --chown=dotnetuser:dotnetuser /publish/ /app/

USER dotnetuser:dotnetuser 
EXPOSE 5000
ENV ASPNETCORE_URLS=http://*:5000
WORKDIR /app
ENTRYPOINT [ "./Minvoice" ]
