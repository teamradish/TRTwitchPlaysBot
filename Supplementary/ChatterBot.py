# This is a Python source file to run a local ChatterBot for chatbot conversations on your stream
# Run this program separately with "python ChatterBot.py"
# It's recommended to run this in the TRBot Data folder at first
# You will need to install Python and ChatterBot as prerequisites (https://chatterbot.readthedocs.io/en/stable/setup.html)

# Overview: This opens a socket with a file used for the data
# After a client sends data through the socket, it feeds that into the bot as a prompt and sends back a response
# This essentially lets users talk with a bot

# Don't hesitate to change any of this to get your desired chatbot behavior - this is just a sample setup
# See more information on the official documentation (https://chatterbot.readthedocs.io/en/stable/index.html)

from chatterbot import ChatBot
from chatterbot.trainers import ChatterBotCorpusTrainer, ListTrainer
import time
import os
import socket
import struct
import traceback

# This is the root path to the file
# Change this to the TRBot data folder where you want to store the bot's files
# This allows running this program anywhere
# Keep it as "." if you want to run the bot in the data folder
BaseDir = "."

SleepTime = 1
FileName = "ChatterBotSocket"

SocketPath = os.path.join(BaseDir, FileName)

chatbot = ChatBot(
        "ChatBot",
        logic_adapters=[
            'chatterbot.logic.MathematicalEvaluation',
            'chatterbot.logic.BestMatch'
        ],
        preprocessors=[
            'chatterbot.preprocessors.unescape_html',
        ]
)

trainer = ChatterBotCorpusTrainer(chatbot)

# Train the chatbot with common english
trainer.train(
    "chatterbot.corpus.english"
)

# Train with custom responses to questions
conversationList = [
    "Hey, that's...",
    "That blue hedgehog again, of all places.",
    "I found you, faker!",
    "Faker? I think you're the fake hedgehog around here. You're comparing yourself to me? Ha! You're not even good enough to be my fake!",
    "Mama",
    "That's Mama Luigi to you, Mario!",
    "Nice of the princess to invite us over for a picnic, eh Luigi?",
    "I hope she made lotsa spaghetti!",
    "Gee, it sure is boring around here...",
    "My boy, this peace is what all true warriors strive for.",
]

listTrainer = ListTrainer(chatbot)

numTrainLoops = 5

# Train it a bunch with this list so it keeps it in its digital head
for i in range(0, numTrainLoops):
    listTrainer.train(conversationList)

# Do anything that needs to be done before getting responses
# chatbot.initialize()

print("\nChatBot trained! Setting up socket and listening for responses...")

BufferSize = 4

# Setup socket
with socket.socket(socket.AF_UNIX, socket.SOCK_STREAM) as chatterBotSocket:

    # Try to replace the socket file if this was run before
    try:
        os.remove(SocketPath)
    except OSError:
        pass

    # Bind socket to address and start listening to accept connections
    chatterBotSocket.bind(SocketPath)
    chatterBotSocket.listen()

    # Keep listening for changes
    while True:
        # conn is the socket object used to send and receive data once it accepts a connection
        # addr is the address bound to the socket on the other end of the connection
        (conn, addr) = chatterBotSocket.accept()

        with conn:
            try:
                while True:
                    # Unpack the native socket data into something python can read (byte strings from C code in this case)
                    # The return value of recv is how many bytes were received, the parameter taking the # of bytes in the buffer
                    # The return value of unpack is a tuple
                    dataReceived = conn.recv(BufferSize)
                    
                    # If no data is received, end the connection with this client
                    if dataReceived is None or len(dataReceived) == 0:
                        break

                    #print(dataReceived)
                    #print(len(dataReceived))

                    bytesReceived = struct.unpack('I', dataReceived)[0]
                    
                    #print("bytesReceived: " + bytesReceived)
                    
                    # Get the data for the number of bytes we actually received
                    # Decode the string as UTF-8
                    promptData = conn.recv(bytesReceived)
                    
                    promptTxt = promptData.decode()
                    #print("Received prompt :" + promptTxt)

                    # Get the response from the chat bot
                    responseTxt = str(chatbot.get_response(promptTxt))

                    #print("Response: " + responseTxt)
                    
                    # Repack the string as well as the encoding data
                    sentData = struct.pack('I', len(responseTxt)) + responseTxt.encode('utf-8')
    
                    # Repack the response and send it off
                    conn.sendall(sentData)

            except (struct.error, KeyboardInterrupt) as e:
                print(e)
            # finally:
            #     print("Closing socket connection for this client and listening for another client.")


     
