#!/usr/bin/env python

# This is a Python source file that sets up a test WebSocket, which you can connect to via TRBot
# Run this program separately with "python websocketexample.py" or "python3 websocketexample.py" depending on your platform
#
# Overview: This sets up a WebSocket on localhost (127.0.0.1) on port 8765 by default
# It then sends "up500ms" and states the user is "terminaluser" - this goes through the WebSocket as JSON
#
# This requires Python 3+ and the websockets package to be installed
# Install the package with "python -m pip install websockets" or "python3 -m pip install websockets" depending on your platform
#
# The websockets package is licensed under BSD 3-Clause: https://github.com/aaugustin/websockets/blob/main/LICENSE
# This specific file is licensed under the same terms as websockets itself

import asyncio
import websockets
import json

async def websocket_handler(websocket, path):
    print("Called websocket_handler!")    

    # Every 3 seconds, send an input over the socket
    while True:
        await asyncio.sleep(3)
        
        obj = {
            "user": {
                "Name": "terminaluser"
            },
            "message": {
                "Text": "up500ms"
            }
        }

        objJSON = json.dumps(obj)

        print(objJSON)

        # If configured correctly, this should cause TRBot to press inputs on its virtual controllers
        await websocket.send(objJSON)

# Main Code

# To connect to this WebSocket via TRBot, set the "client_service_type" to 2 then open WebSocketConnectionSettings.txt (if not available, run TRBot once to generate it)
# Set the ConnectURL to "ws://(hosthere):(porthere)/" - for instance, the default should be "ws://127.0.0.1:8765/"
host = "127.0.0.1"
port = 8765

start_server = websockets.serve(websocket_handler, host, port)

asyncio.get_event_loop().run_until_complete(start_server)
asyncio.get_event_loop().run_forever()
