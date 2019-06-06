import time
import Vjoy

from Vjoy import joyState, SetButton, SetPOV, UpdateJoyState
from threading import Thread
from copy import copy
from TwitchBot import sendmessage
import pyautogui

INPUTS = {
    "left":         0, "c":             0,
    "right":        1, "z":             1,
    "up":           2, "tleft":         2,
    "down":         3, "tright":        3,
    "a":            4,
    "b":            5,
    "l":            6, "one":           6,
    "r":            7, "two":           7,
    "select":       8, "minus":         8,
    "start":        9, "plus":          9,
    "cleft":        10, "pleft":        10,
    "cright":       11, "pright":       11,
    "cup":          12, "pup":          12,
    "cdown":        13, "pdown":        13,
    "dleft":        14,
    "dright":       15,
    "dup":          16,
    "ddown":        17,
    "SAVESTATE1":   18, "tforward":     18,
    "SAVESTATE2":   19, "tback":        19,
    "SAVESTATE3":   20,
    "SAVESTATE4":   21,
    "SAVESTATE5":   22,
    "SAVESTATE6":   23,
    "LOADSTATE1":   24,
    "LOADSTATE2":   25,
    "LOADSTATE3":   26,
    "LOADSTATE4":   27,
    "LOADSTATE5":   28,
    "LOADSTATE6":   29,
    "x":            30, "shake":        30,
    "y":            31, "point":         31
}

def reset():

    pyautogui.press("f1")

def savestate(num, user):

    global Controllers

    if user["level"] < 3 and num <= 3 and num > 0:
        return "Access denied"
    elif num > 0 and num <= 6:
        Controllers[1].execute_input("SAVESTATE" + str(num), 200)
        #pyautogui.press(str(num))
        #pyautogui.press("f5")

        f = open("last_save.txt", "w")
        f.write(str(num))
        f.close()

        return "/me Saving state " + str(num)
    else: return "Invalid number."

def loadstate(num, user):

    global Controllers

    if num > 0 and num <= 6:
        Controllers[1].execute_input("LOADSTATE" + str(num), 200)
        #pyautogui.press(str(num))
        #pyautogui.press("f7")

        f = open("last_load.txt", "w")
        f.write(str(num))
        f.close()

        return "/me Loading state " + str(num)
    else: return "Invalid number."

class Controller:

    def __init__(self, id=0):

        self.id = id
        self.buttons ={
        "left":         False, "tleft":         False,
        "right":        False, "tright":        False,
        "up":           False,
        "down":         False,
        "a":            False,
        "b":            False,
        "l":            False, "one":           False,
        "r":            False, "two":           False,
        "select":       False, "minus":         False, "z": False,
        "start":        False, "plus":          False,
        "cleft":        False, "pleft":         False,
        "cright":       False, "pright":        False,
        "cup":          False, "pup":           False,
        "cdown":        False, "pdown":         False,
        "dleft":        False, "c":             False,
        "dright":       False, "z":             False,
        "dup":          False,
        "ddown":        False,
        "SAVESTATE1":   False, "tforward":      False,
        "SAVESTATE2":   False, "tback":         False,
        "SAVESTATE3":   False,
        "SAVESTATE4":   False,
        "SAVESTATE5":   False,
        "SAVESTATE6":   False,
        "LOADSTATE1":   False,
        "LOADSTATE2":   False,
        "LOADSTATE3":   False,
        "LOADSTATE4":   False,
        "LOADSTATE5":   False,
        "LOADSTATE6":   False,
        "x":            False, "shake":         False,
        "y":            False, "point":          False,
        "#":            False,
        ".":            False
        }

        self.paused = False
        self.thread_count = 0

    def execute_input_array(self, input_array):

        self.thread_count += 1

        # The list of buttons this particular instance cares about
        instance_buttons = []

        #test1 = time.clock()

        # For each string of simultaneous buttons
        for simultaneous_buttons in input_array:

            # Determine the delay time
            max_delay = 0
            for button in simultaneous_buttons:

                if button.duration > max_delay: max_delay = button.duration

            # Press each button in a separate thread, as they are simultaneous
            for button in simultaneous_buttons:

                b = Thread(target=self.execute_input, args=[button.name, button.duration, button.percent, button.hold, button.release])
                b.start()

                instance_buttons.append(button.name)
            # Wait the maximum time
            delay_time = int(max_delay)/1000
            #print(str(delay_time))
            if delay_time > 0:
                time.sleep(delay_time)
            else:
                pass

        #test2 = time.clock()
        #print((test2-test1)*1000)
        # Release any buttons this instance started holding
        for button in instance_buttons:
            if self.buttons[button]:
                # TEMP to add l
                if button == "l" or button == "left" or button == "right" or button == "up" or button == "down" or button == "pleft" or button == "pright" or button == "pup" or button == "pdown":
                    self.release_analog(button)
                elif button != "." and button != "#":
                    self.release_digital(button)

        self.thread_count -= 1

    def hold_digital_duration(self, val="", duration=0):

        global Vjoy
        global joyState

        score = 0
        if self.buttons["a"] or val == "a": score += 1
        if self.buttons["b"] or val == "b": score += 1
        if self.buttons["select"] or val == "select": score += 1
        if self.buttons["start"] or val == "start": score += 1
        if score >= 4:
            self.release_digital("a")
            self.release_digital("b")
            self.release_digital("select")
            sendmessage("No resetting ;P")
        else:
            if val != "point":
                SetButton(joyState[self.id], INPUTS[val], Vjoy.BUTTON_DOWN)
            else:
                SetButton(joyState[self.id], INPUTS[val], Vjoy.BUTTON_UP)
            self.buttons[val] = True
            UpdateJoyState(self.id, joyState[self.id])

            time.sleep(duration/1000)
            if val != "point":
                SetButton(joyState[self.id], INPUTS[val], Vjoy.BUTTON_UP)
            else:
                SetButton(joyState[self.id], INPUTS[val], Vjoy.BUTTON_DOWN)
            UpdateJoyState(self.id, joyState[self.id])
            self.buttons[val] = False


    def hold_digital_indefinite(self, val=""):

        global Vjoy
        global joyState

        score = 0
        if self.buttons["a"] or val == "a": score += 1
        if self.buttons["b"] or val == "b": score += 1
        if self.buttons["select"] or val == "select": score += 1
        if self.buttons["start"] or val == "start": score += 1
        if score >= 4:
            self.release_digital("a")
            self.release_digital("b")
            self.release_digital("select")
            sendmessage("No resetting ;P")
        else:
            if val != "point":
                SetButton(joyState[self.id], INPUTS[val], Vjoy.BUTTON_DOWN)
            else:
                SetButton(joyState[self.id], INPUTS[val], Vjoy.BUTTON_UP)
            UpdateJoyState(self.id, joyState[self.id])

            self.buttons[val] = True

    def release_digital(self, val=""):

        global Vjoy
        global joyState

        if val != "point":
            SetButton(joyState[self.id], INPUTS[val], Vjoy.BUTTON_UP)
        else:
            SetButton(joyState[self.id], INPUTS[val], Vjoy.BUTTON_DOWN)
        UpdateJoyState(self.id, joyState[self.id])
        self.buttons[val] = False

    def hold_analog_duration(self, val="", duration=0, percent=100):

        global Vjoy
        global joyState

        # TEMP
        if val == "l": joyState[self.id].XRotation = int(Vjoy.AXIS_MIN * percent/100)

        if val == "left": joyState[self.id].XAxis = int(Vjoy.AXIS_MIN * percent/100)
        if val == "right": joyState[self.id].XAxis = int(Vjoy.AXIS_MAX * percent/100)
        if val == "up": joyState[self.id].YAxis = int(Vjoy.AXIS_MAX * percent/100)
        if val == "down": joyState[self.id].YAxis = int(Vjoy.AXIS_MIN * percent/100)

        if val == "pleft": joyState[self.id].XRotation = int(Vjoy.AXIS_MIN * percent/100)
        if val == "pright": joyState[self.id].XRotation = int(Vjoy.AXIS_MAX * percent/100)
        if val == "pup": joyState[self.id].YRotation = int(Vjoy.AXIS_MIN * percent/100)
        if val == "pdown": joyState[self.id].YRotation = int(Vjoy.AXIS_MAX * percent/100)

        UpdateJoyState(self.id, joyState[self.id])
        self.buttons[val] = True

        time.sleep(duration/1000)

        # TEMP
        if val == "l": joyState[self.id].XRotation = Vjoy.AXIS_NIL

        if val == "left": joyState[self.id].XAxis = Vjoy.AXIS_NIL
        if val == "right": joyState[self.id].XAxis = Vjoy.AXIS_NIL
        if val == "up": joyState[self.id].YAxis = Vjoy.AXIS_NIL
        if val == "down": joyState[self.id].YAxis = Vjoy.AXIS_NIL

        if val == "pleft": joyState[self.id].XRotation = Vjoy.AXIS_NIL
        if val == "pright": joyState[self.id].XRotation = Vjoy.AXIS_NIL
        if val == "pup": joyState[self.id].YRotation = Vjoy.AXIS_NIL
        if val == "pdown": joyState[self.id].YRotation = Vjoy.AXIS_NIL

        UpdateJoyState(self.id, joyState[self.id])
        self.buttons[val] = False

    def hold_analog_indefinite(self, val="", percent=100):

        global Vjoy
        global joyState

        # TEMP
        if val == "l": joyState[self.id].XRotation = int(Vjoy.AXIS_MIN * percent/100)

        if val == "left": joyState[self.id].XAxis = int(Vjoy.AXIS_MIN * percent/100)
        if val == "right": joyState[self.id].XAxis = int(Vjoy.AXIS_MAX * percent/100)
        if val == "up": joyState[self.id].YAxis = int(Vjoy.AXIS_MAX * percent/100)
        if val == "down": joyState[self.id].YAxis = int(Vjoy.AXIS_MIN * percent/100)

        if val == "pleft": joyState[self.id].XRotation = int(Vjoy.AXIS_MIN * percent/100)
        if val == "pright": joyState[self.id].XRotation = int(Vjoy.AXIS_MAX * percent/100)
        if val == "pup": joyState[self.id].YRotation = int(Vjoy.AXIS_MIN * percent/100)
        if val == "pdown": joyState[self.id].YRotation = int(Vjoy.AXIS_MAX * percent/100)

        UpdateJoyState(self.id, joyState[self.id])
        self.buttons[val] = True

    def release_analog(self, val=""):

        global Vjoy
        global joyState

        if val == "l":
            joyState[self.id].XRotation = Vjoy.AXIS_NIL
            self.release_digital("l")

        if val == "left": joyState[self.id].XAxis = Vjoy.AXIS_NIL
        if val == "right": joyState[self.id].XAxis = Vjoy.AXIS_NIL
        if val == "up": joyState[self.id].YAxis = Vjoy.AXIS_NIL
        if val == "down": joyState[self.id].YAxis = Vjoy.AXIS_NIL

        if val == "pleft": joyState[self.id].XRotation = Vjoy.AXIS_NIL
        if val == "pright": joyState[self.id].XRotation = Vjoy.AXIS_NIL
        if val == "pup": joyState[self.id].YRotation = Vjoy.AXIS_NIL
        if val == "pdown": joyState[self.id].YRotation = Vjoy.AXIS_NIL

        UpdateJoyState(self.id, joyState[self.id])
        self.buttons[val] = False

    def execute_input(self, val="", duration=0, percent=100, hold=False, release=False):

        # TEMP to add l
        if (val == "l" and percent != 100) or val == "left" or val == "right" or val == "up" or val == "down" or val == "pleft" or val == "pright" or val == "pup" or val == "pdown":
            if hold:
                self.hold_analog_indefinite(val, percent)
            elif release:
                self.release_analog(val)
            else:
                self.hold_analog_duration(val, duration, percent)
        elif val != "." and val != "#" and not (val.startswith("SAVE") or val.startswith("LOAD")):
            if hold:
                self.hold_digital_indefinite(val)
            elif release:
                self.release_digital(val)
            else:
                self.hold_digital_duration(val, duration)
        else:
            if val == ".":
                self.buttons[val] = True
                time.sleep(0.2)
                self.buttons[val] = False
            elif val == "#":
                self.buttons[val] = True
                time.sleep(duration/1000)
                self.buttons[val] = False
            else:
                self.hold_digital_duration(val, duration)

Controllers = [Controller(0), Controller(1)]
