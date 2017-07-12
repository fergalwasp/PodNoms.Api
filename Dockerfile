FROM microsoft/aspnetcore:2.0.0-preview2
ENV ASPNETCORE_URLS=http://+:5000
RUN echo "deb http://http.debian.net/debian jessie-backports main contrib non-free" >> /etc/apt/sources.list

RUN apt-get update \
    && apt-get install -y  \
        python-pip python-dev build-essential ffmpeg

RUN pip install --upgrade pip
RUN pip install --upgrade youtube_dl

COPY bin/Release/netcoreapp2.0/publish/ /app
EXPOSE 5000/tcp
WORKDIR /app/

CMD ["dotnet", "PodNoms.Api.dll"]
