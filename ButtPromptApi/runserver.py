"""
This script runs the ButtPromptApi application using a development server.
"""

from os import environ
from ButtPromptApi import PromptRunner
# import uvicorn

#if __name__ == '__main__':
#     HOST = environ.get('SERVER_HOST', 'localhost')
# #    try:
# #        PORT = int(environ.get('SERVER_PORT', '5555'))
# #    except ValueError:
# #        PORT = 5555
#     PORT = 5555
#     uvicorn.run(app, host="0.0.0.0", port=PORT)
    
if __name__ == '__main__':
    
    server_url = environ.get("ButtsUrl")
    if not server_url:
       server_url = "ws://localhost:5023/prompt"
    prompt_runner = PromptRunner(server_url)
    prompt_runner.run()