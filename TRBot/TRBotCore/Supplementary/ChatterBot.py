# This is a Python source file to run a local ChatterBot for chatbot conversations in your stream
# Place this file in your TRBot instance's Data folder and run this script
# You will need to install Python and ChatterBot first (https://chatterbot.readthedocs.io/en/stable/setup.html)

# Overview: This looks for changes in a text file that is used as a bot prompt
# If it detects a change in the file, it'll send the prompt through ChatterBot and write the response to a response file
# If "UseChatBot" is true in TRBot's Settings.txt file, a routine will look for changes in the response file and output that response to the chat
# This essentially lets users talk with a bot

from chatterbot import ChatBot
from chatterbot.trainers import ChatterBotCorpusTrainer
import time
import os
import traceback

SleepTime = 1
PromptFileName = "ChatBotPrompt.txt"
ResponseFileName = "ChatBotResponse.txt"

chatbot = ChatBot(
        "ChatBot",
        #logic_adapters=[
        #    'chatterbot.logic.MathematicalEvaluation',
        #    'chatterbot.logic.TimeLogicAdapter',
        #    'chatterbot.logic.BestMatch'
        #]
)

trainer = ChatterBotCorpusTrainer(chatbot)

# Train the chatbot with common english
trainer.train(
    "chatterbot.corpus.english"
)

# response = chatbot.get_response("Hi, how are you?")

#print(response)

LastMTime = time.time()

# Keep listening for changes
while True:
    # Sleep this amount of time
    time.sleep(SleepTime)
    
    # print("1 second passed")
    
    # Get the time the prompt file was modified
    mTime = 0
    try:
        mTime = os.path.getmtime(PromptFileName)
        #print(mTime)
    except:
        print("Exception when obtaining file timestamp")
        continue

    # If the checked time is less than or equal to the stored time, it hasn't been changed
    if mTime <= LastMTime:
        continue
    
    #print("File newly modified!")

    # Update modified time
    LastMTime = mTime
    
    fileTxt = ""
    
    # Read the prompt file
    try:    
        with open(PromptFileName, "r") as File_obj:
            fileTxt = File_obj.read()
    except:
        print("Exception opening or reading prompt file: ", traceback.format_exc())
        continue

    # Get the response from the chat bot
    response = str(chatbot.get_response(fileTxt))

    # Write to the response file
    try:
        with open(ResponseFileName, "w") as Response_File:
            Response_File.write(response)
    except:
        print("Exception writing to response file: ", traceback.format_exc())




     
