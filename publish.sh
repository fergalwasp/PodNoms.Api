#!/bin/bash
unset DOCKER_HOST
unset DOCKER_TLS_VERIFY
dotnet build -c Release
# dotnet ef database update --environment Production
dotnet publish -c Release
docker build -t fergalmoran/podnoms.api .

