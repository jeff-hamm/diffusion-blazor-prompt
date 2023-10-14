from re import L
import string
from typing import Annotated
from .PromptArgs import PromptArgs
from.Listener import Listener
from diffusers import utils
import logging
import time
import sys
import threading
from signalrcore.hub_connection_builder import HubConnectionBuilder
from .PromptPipeline import PromptPipeline
from .ListenerStream import ListenerStream
from os import environ

class NextPromptResponse:
    def __init__(self):
        self.lock = threading.Lock();
    def acquire(self, timeout:float|None=None):
        return self.lock.acquire(timeout)
            
    def release(self):
        self.lock.release();




    
class PromptRunner:
    def __init__(self, server_url:str):
        self.hub_connection = HubConnectionBuilder() \
            .with_url(server_url, options={
                "verify_ssl": False,
            }) \
            .configure_logging(logging.INFO) \
            .with_automatic_reconnect({ 
                "type": "interval",
                "keep_alive_interval": 10,
                "intervals": [1, 3, 5, 6, 7, 87, 3]
            }).build();
        self.newPromptEvent = threading.Event()
        self.stream = ListenerStream(self.hub_connection,sys.stderr)
        self.pipeline = PromptPipeline(self.stream)
        self.pipeline.load()
        sys.stderr = self.stream
        stream_handler = logging.StreamHandler(self.stream)
        utils.logging.add_handler(stream_handler)
        self.end = False


    def reconnect(self):
        print("try reconnect")
        while not self.end:
            try:
                if(self.hub_connection.start()):
                    time.sleep(10)
                    self.hub_connection.send("StartProcessing", []);
                    return
            except:
                print("Connection Failed, retrying")
                time.sleep(2)

    def newPrompt(self,id):
        self.newPromptEvent.set()
        
    def processNext(self):
        prompt_response = NextPromptResponse()
        prompt_response.prompt = None
        prompt_response.lock.acquire()
        self.hub_connection.send(
            "ProcessNext", # Method
            [], # Params
            lambda prompt: self.processNextCallback(prompt_response, prompt)) # Callback
        print("Response Lock")
        if not prompt_response.lock.acquire(timeout=30) or prompt_response.prompt is None:
            print("Prompt not found")
            return False
        
        prompt = prompt_response.prompt
        print(f"Got prompt {prompt.id}: {prompt}")
        generated = self.pipeline.generatebutt(prompt)
        self.hub_connection.send("ProcessComplete", [prompt.id, generated])
        return True
    
    def processNextCallback(self, lock:NextPromptResponse, prompt_dict):
        print("Got response", prompt_dict)
        if(prompt_dict.result):
            lock.prompt = PromptArgs(prompt_dict.result)
        lock.release();
        print("Unlocked");
        
        


    def run(self):
        self.hub_connection.on_open(lambda: print("connection opened and handshake received ready to send messages"))
        self.hub_connection.on_close(lambda: self.reconnect())
        self.hub_connection.on_error(lambda data: print(f"An exception was thrown closed{data.error}"))
        self.hub_connection.on("NewPrompt", lambda id: self.newPrompt(id))
        self.reconnect()
        while not self.end:
            try:
                while not self.end:
                    self.newPromptEvent = threading.Event()
                    if(not self.processNext()):
                        self.newPromptEvent.wait(timeout=60)
                    else:
                        time.sleep(1)
            except Exception as ex:
                print("Exception" + str(ex))
                time.sleep(2)
        self.hub_connection.send("StopProcessing", []);
        self.hub_connection.stop()
        
