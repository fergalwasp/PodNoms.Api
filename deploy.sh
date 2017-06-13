#!/bin/bash
dotnet build
dotnet ef database update --environment Production
dotnet publish
docker build -t fergalmoran/podnoms.api .

