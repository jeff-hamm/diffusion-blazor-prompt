
while ( true ) do \
  ffmpeg -i http://192.168.122.1:8080/liveview/liveviewstream -vcodec libvpx -fflags nobuffer -an http://127.0.0.1:8090/feed1.ffm; \
  sleep 1 ; \
done &
sleep 5
ffserver -f /ffserver/ffserver.conf

