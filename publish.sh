#!/bin/bash
dotnet build
# dotnet ef database update --environment Production
dotnet publish -c Release
docker build -t fergalmoran/podnoms.api .

