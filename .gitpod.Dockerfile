FROM gitpod/workspace-full-vnc:latest

USER root
# Install .NET runtime dependencies and the git gui
RUN apt-get update \
    && apt-get install -y \
        libc6 \
        libgcc1 \
        libgssapi-krb5-2 \
        libicu66 \
        libssl1.1 \
        libstdc++6 \
        zlib1g \
        git-gui \
        unzip \
    && rm -rf /var/lib/apt/lists/*

# install protocol buffer compiler
RUN wget -O protoc.zip https://github.com/protocolbuffers/protobuf/releases/download/v3.15.8/protoc-3.15.8-linux-x86_64.zip \
    && unzip protoc.zip -d $HOME/.local \
    && rm protoc.zip


USER gitpod

# messy handling for https://github.com/gitpod-io/gitpod/issues/5090
ENV DOTNET_ROOT="/workspace/.dotnet"
ENV PATH=$PATH:$DOTNET_ROOT
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true
