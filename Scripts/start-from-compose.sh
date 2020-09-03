#!/bin/bash
PROJECT_NAME=Dottalk
CSPROJ_PATH=./$PROJECT_NAME

echo "[START]: Staring dotnet watch..."
dotnet run --project $CSPROJ_PATH --urls $ASPNETCORE_URLS