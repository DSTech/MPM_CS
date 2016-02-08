FROM mono:latest
MAINTAINER Dessix <Dessix@Dessix.net>

RUN mkdir -p /usr/src/appbuild && mkdir -p /usr/src/app
VOLUME /usr/src/app/data

COPY ./ /usr/src/appbuild
WORKDIR /usr/src/appbuild
RUN nuget restore MPM.sln
RUN xbuild /p:Configuration=Release /tv:12.0 MPM.sln

RUN cp -r ./Repository/bin/Release/. /usr/src/app/

WORKDIR /usr/src/app

EXPOSE 8950
ENTRYPOINT ["mono", "./Repository.exe"]
