FROM microsoft/aspnetcore:2.0

RUN apt-get update \
    && apt-get install -y  \
        youtube-dl

COPY bin/Debug/netcoreapp2.0/publish/ /app
EXPOSE 5000/tcp
WORKDIR /app/

CMD ["dotnet", "PodNoms.Api.dll"]
