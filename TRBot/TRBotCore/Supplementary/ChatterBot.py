# This is a Python source file to run a local ChatterBot for chatbot conversations on your stream
# Run this program separately with "python ChatterBot.py"
# It's recommended to run this in the TRBot Data folder at first
# You will need to install Python and ChatterBot as prerequisites (https://chatterbot.readthedocs.io/en/stable/setup.html)

# Overview: This looks for changes in a text file that is used as a bot prompt
# If it detects a change in the file, it'll send the prompt through ChatterBot and write the response to a response file
# If "UseChatBot" is true in TRBot's Settings.txt file, a routine will look for changes in the response file and output that response to the chat
# This essentially lets users talk with a bot

# Don't hesitate to change any of this to get your desired chatbot behavior - this is just a sample setup
# See more information on the official documentation (https://chatterbot.readthedocs.io/en/stable/index.html)

from chatterbot import ChatBot
from chatterbot.trainers import ChatterBotCorpusTrainer
from chatterbot.trainers import UbuntuCorpusTrainer
import time
import os
import traceback

# This is the root path to the prompt and response files
# Change this to the TRBot data folder where you want to store the bot's files
# This allows running this program anywhere
# Keep it as "." if you want to run the bot in the data folder
Path = "."

SleepTime = 1
PromptFileName = "ChatBotPrompt.txt"
ResponseFileName = "ChatBotResponse.txt"

PromptDir = os.path.join(Path, PromptFileName)
ResponseDir = os.path.join(Path, ResponseFileName)

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

# Train with Ubuntu Corpus Trainer
# This can take a VERY long time to download, unpack, and train
# Uncomment it only if you want your bot to be trained with a lot of data
# Make sure to comment out the above trainer if you're using this
# ubuntuCorpusTrainer = UbuntuCorpusTrainer(chatbot)
# ubuntuCorpusTrainer.train()

# response = chatbot.get_response("Hi, how are you?")

#print(response)

print("\nChatBot trained! Listening for responses...");

LastMTime = time.time()

# Keep listening for changes
while True:
    # Sleep this amount of time
    time.sleep(SleepTime)
    
    # print("1 second passed")
    
    # Get the time the prompt file was modified
    mTime = 0
    try:
        mTime = os.path.getmtime(PromptDir)
        #print(mTime)
    except:
        print("Exception when obtaining file timestamp: ", traceback.format_exc())
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
        with open(PromptDir, "r") as File_obj:
            fileTxt = File_obj.read()
    except:
        print("Exception opening or reading prompt file: ", traceback.format_exc())
        continue

    # Get the response from the chat bot
    response = str(chatbot.get_response(fileTxt))

    # Write to the response file
    try:
        with open(ResponseDir, "w") as Response_File:
            Response_File.write(response)
    except:
        print("Exception writing to response file: ", traceback.format_exc())




     
