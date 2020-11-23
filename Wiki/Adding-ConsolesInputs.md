# Adding, Removing, Modifying Consoles and Inputs
TRBot utilizes a `GameConsole` model that defines the valid inputs available for users to type to control the game. There are several [pre-configured consoles](../TRBot/TRBot.Consoles/Consoles) added to the database upon first starting up TRBot, which you can use for any game you want.

What if you want to add a new console specific to a single game, add a new input (Ex. a "toggleaudio" input for 2 linked-game GBA instances), or remove some inputs? This guide will cover all three!

## Adding a new console
First let's start with adding a new console. All we need for a new console is a name, so for this example, let's go with the name "newconsole".

We'll use TRBot's [`AddConsoleCommand`](../TRBot/TRBot.Commands/Commands/AddConsoleCommand.cs) ("!addconsole" by default) to add a new console to the database.

Type the following without quotes: "!addconsole newconsole"

Now, we should have a new game console named "newconsole". We can verify this through the [`GetSetConsoleCommand`](../TRBot/TRBot.Commands/Commands/GetSetConsoleCommand.cs) ("!console" by default), which will list all available consoles. Simply type "!console" without quotes and we can see that "newconsole" is in list. Type "!console newconsole" to switch the active console to our new console.

After switching, use the [`InputInfoCommand`](../TRBot/TRBot.Commands/Commands/GetSetConsoleCommand.cs) ("!inputs" by default) without any arguments to see...uh oh, there are no inputs! Newly created consoles do not come with any inputs - think of a game console without a controller. We now have to define the buttons on the controller so we can actually play the console.

## Adding new inputs
Let's add some inputs to the "newconsole" console through the [`AddInputCommand`](../TRBot/TRBot.Commands/Commands/AddInputCommand.cs) ("!addinput" by default). It's possible to add new inputs directly through the database, but this command greatly simplifies the process by automatically linking the input to the console (adding directly to the database requires looking up the console ID and specifying it in the record in the **Inputs** table).

The `AddInputCommand` takes the following arguments:

1. **console name** - The name of the console to add the input for. In our case, it will be "newconsole".
2. **input name** - The name of the input.
3. **buttonVal** - An integer representing the button value of the input on the virtual controller.
4. **axisVal** - An integer representing the axis value of the input on the virtual controller.
5. **inputType** - An integer representing the type of the input. 0 = Blank input, 1 = Button (Ex. "a" button), 2 = Axis (Ex. analog stick on the N64 controller), 3 = Button+Axis (Ex. "l" and "r" triggers on GameCube controllers, which function as both)
6. **minAxis** - An integer from -1 to 1 representing the minimum axis range if the input is an axis. 0 indicates the center of the analog stick. This will often be 0.
7. **maxAxis** - An integer from -1 to 1 representing the maximum axis range if the input is an axis. 1 indicates the center of the analog stick. This will often be -1 (up/left on the analog stick) or 1 (down/right on the analog stick).
8. **maxAxis percent** - An integer from 0 to 100 representing the maximum percent the axis can be pressed - any value afterwards is considered a button press. As an example, the GameCube's "l" and "r" triggers would have this at 99 since they function as axes until they're pressed down 100%, after which they're the L and R buttons.

That was a lot to absorb, so let's jump right into adding a simple "a" button with a button value of 1 to "newconsole".

Type the following: "!addinput newconsole a 0 0 1 0 0 0"

Oops! If you noticed, the 3rd argument, the button value, is 0 instead of 1. Let's fix that. Fortunately, we don't need to do anything different aside from supply a different value.

"!addinput newconsole a 1 0 1 0 0 0"

That's right, the `AddInputCommand` will update an input if it already exists for the given console. You can modify inputs the same way as you add them! Since the axis value for our "a" button isn't used, we didn't need to think about the data we supplied for the axis values.

Now let's add a full analog stick to this console. The X axis will be axis value 0, and the Y axis will be axis value 1. This time, we have to specify an input type of **2** to indicate that this input is an axis.

### X axis (Left/Right)
"!addinput newconsole left 0 0 2 0 -1 100"
"!addinput newconsole right 0 0 2 0 1 100"

### Y axis (Up/Down)
"!addinput newconsole up 0 1 2 0 -1 100"
"!addinput newconsole down 0 1 2 0 1 100"

If you noticed, the only difference between the opposing directions on each axis is the maximum axis value. If a user inputs "up50%", the normalized value will be -0.5 (halfway up), whereas "down75%" will have a normalized value of 0.75 (3/4 down). You don't have to strictly follow this model for defining axes, but it's highly recommended, as most game controllers work this way.

Similarly to the "a" button, we don't need to care about the button value since the input is an axis and won't use it. If the max axis percent was less than 100, we would, since the axis can act as a button above that threshold, and we don't want it to interfere with existing buttons. 

## Removing inputs
Our "newconsole" is coming along! It's got an "a" button and a full analog stick with "up", "down", "left", and "right" inputs available. However, what if we no longer want the "a" button so we can replace it with something else?

Simply use the [`RemoveInputCommand`](../TRBot/TRBot.Commands/Commands/RemoveInputCommand.cs) ("!removeinput" by default).

Type the following: "!removeinput newconsole a"

That's it! The `RemoveInputCommand` simply takes in the console and input name as arguments. If you view all inputs with "!inputs", the "a" button should now be removed.

# Uses
There are many uses for custom consoles and inputs:

* Add a new console TRBot doesn't come with out of the box.
* Create buttons for hotkeys, such as a "toggleaudio" input that mutes/unmutes audio on an emulator.
* Implement a "split" input, used in a script through software such as JoyToKey, that activates the next split in software such as LiveSplit.
* ...and much more! Keep exploring and share your findings!
