from .Listener import Listener
from signalrcore.hub.base_hub_connection import BaseHubConnection
import sys
class ListenerStream:
      def __init__(self, listener: BaseHubConnection, stream):
        self.listener = listener
        self.stream = stream

      def write(self, data):
        self.stream.write(data);
        if(self.listener.transport.is_running()):
            self.listener.send("StatusMessage", [data])
#        self.listener.publish_message(data)
        
      def flush(self):
        self.stream.flush();
        return
