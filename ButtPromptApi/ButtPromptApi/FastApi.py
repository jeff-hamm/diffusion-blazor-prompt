import string
from typing import Annotated
from .PromptFile import PromptFile
from.Listener import Listener
from fastapi import FastAPI,Depends,BackgroundTasks,UploadFile,WebSocket
from diffusers import utils
import logging
import sys
import asyncio
from .PromptPipeline import PromptPipeline
from .ListenerStream import ListenerStream
app = FastAPI()

global_listener = Listener()
global_pipeline = PromptPipeline(global_listener)
global_stream = ListenerStream(global_listener,sys.stderr)
sys.stderr = global_stream
global_handler = logging.StreamHandler(global_stream)
utils.logging.add_handler(global_handler)


@app.on_event("startup")
async def startup_event():
    await global_listener.start_listening()
    global_pipeline.load()
    return

@app.on_event("shutdown")
async def shutdown_event():
    await global_listener.stop_listening()
    return

@app.get("/prompt")
async def get_prompt():
     return global_state

@app.websocket("/status")
async def websocket_endpoint(websocket: WebSocket):
    await websocket.accept()
    q: asyncio.Queue = asyncio.Queue()
    await global_listener.subscribe(q=q)
    try:
        while True:
            data = await q.get()
            await websocket.send_text(data)
    except Exception as e:
        sys.stderr.write(str(e))
        await global_listener.unsubscribe(q);
        return
    # await websocket.accept()
    # while True:
    #     data = await websocket.receive_text()
    #     await websocket.send_text(data)

@app.post("/prompt")
async def create_upload_file(file:PromptFile, background_tasks: BackgroundTasks):
    if(global_pipeline.state.running):
        return global_pipeline.state;
    global_stream.write("Recieved prompt");
    global_pipeline.state.running = True
    background_tasks.add_task(global_pipeline.generatebutt, file)
    return {"filename": file.outputfile}