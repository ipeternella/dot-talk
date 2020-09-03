# Main variables required for the Docker layers
ARG MAIN_PROJECT_NAME=Dottalk
ARG DOTNETCORE_VERSION=3.1

# Starting layer point using build image with dotnet SDK (very heavy image ~ 2GB)
FROM mcr.microsoft.com/dotnet/core/sdk:$DOTNETCORE_VERSION AS build-env
ARG MAIN_PROJECT_NAME
ARG DOTNETCORE_VERSION

WORKDIR /app

# Copy .csproj and performs an explicit Nuget restore - smart dependencies caching
COPY ./$MAIN_PROJECT_NAME/$MAIN_PROJECT_NAME.csproj ./$MAIN_PROJECT_NAME/
RUN dotnet restore ./$MAIN_PROJECT_NAME/$MAIN_PROJECT_NAME.csproj

# Copy all files
COPY . ./

# Compiles project as a single binary
WORKDIR ./$MAIN_PROJECT_NAME
RUN dotnet publish --runtime alpine-x64 --configuration Release --output out \
    -p:PublishSingleFile=true -p:PublishTrimmed=true

# Final layer based on Alpine Linux (ultra light-weight ~ 5MB)
FROM alpine:3.9.4 AS runtime-env
ARG MAIN_PROJECT_NAME
ARG DOTNETCORE_VERSION

# Installing some libraries required by .NET Core on Alpine Linux
RUN apk add --no-cache libstdc++ libintl icu

# Copies from the build environment the compiled files of the out folder
WORKDIR /app

# Creates an app and its group and make it own the final /app folder
RUN addgroup --system --gid 777 appgroup && \
    adduser --system --uid 777 --ingroup appgroup appuser && \
    chown -R appuser:appgroup /app

# Copies the compiled program with CHOWN to the app user/group to avoid a heavy RUN chown layer
COPY --chown=appuser:appgroup --from=build-env /app/$MAIN_PROJECT_NAME/out /app

# Switch to the app (non-root user)
USER appuser

# Requires ASPNETCORE_URLS env var such as: http://0.0.0.0:5005
ENTRYPOINT ["./Dottalk"]