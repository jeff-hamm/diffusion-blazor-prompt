#
# Dockerfile for ffserver
#

FROM alpine:3.8
RUN apk update && apk add curl bash ffmpeg && \
    rm -rf /var/cache/apk/*

WORKDIR /ffserver
COPY entrypoint.sh .
COPY ffserver.conf .
EXPOSE 554 8090
ENTRYPOINT ["/bin/bash"]
CMD ["/ffserver/entrypoint.sh"]