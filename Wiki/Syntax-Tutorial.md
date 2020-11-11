This tutorial serves to demonstrate all the features of TRBot's input syntax.

#### Table of Contents
* [Overview](#overview)
* [Input Sequences](#input-sequences)
* [Simultaneous Inputs](#simultaneous-inputs)
* [Holding and Releasing Inputs](#holding-and-releasing-inputs)
* [Delaying Inputs](#delaying-inputs)
* [Repeated Inputs](#repeated-inputs)
* [Multi-Controller Inputs](#multi-controller-inputs)
* [Macros](#macros)
* [Dynamic Macros](#dynamic-macros)
* [Input Synonyms](#input-synonyms)

## Overview

TRBot's syntax takes the following form:

(&#) (- or _) (input) (#%) (#ms or #s) 
1. &# - This is the controller port number the input should apply to. | Optional. Example: "&1"
2. _ or - - This is a hold/release modifier; - means to release the input, and _ means to hold the input until either the end of the sequence or a corresponding release input. | Optional. Examples: "_a", "-a"
3. input - This is the actual input; think of the "A" or "B" buttons on many game controllers. | **Required.** Example: "start"
4. #% - This is the percentage to press the input; this is commonly used on analog sticks. | Optional. Example: "50%" 
5. #ms or #s - This is how long to hold the input for, in milliseconds or seconds, but not both. | Optional. Examples: "450ms", "2s"

Inputs are *not* case-sensitive and vary by the console in use. The valid input list can be viewed by running the `InputInfoCommand` (default: "!inputs"). A blank input, also known as a wait input, is usually available, with "#" as the default. Sometimes, "." will be a synonym for "#" (more on synonyms later below).

If a duration isn't specified, it defaults to the [default_input_duration](./Settings-Documentation.md#default_input_duration) setting, which can be viewed with the `DefaultInputDurCommand` (default: "!defaultinputdur"). The default on a clean TRBot installation is 200 milliseconds.

Complete input example:
* "&1right70%500ms" - this holds "right" 70% for 500 milliseconds on player 1's controller

#### Parsing Order

Inputs are parsed from left to right with no delay in between. Ex. "a b" will press "a" first then "b". "a #500ms b" will press "a", wait 500 milliseconds, then press "b".

## Input Sequences

It's possible to perform as many inputs as you want in a message. Each string of inputs is known as an input sequence. Input sequences can be performed with or without whitespace. For example, "a b" is equivalent to "ab".

Example: "left500ms b500ms right500ms b500ms" ([Demonstration](https://clips.twitch.tv/InterestingBoldKathyEleGiggle))

The maximum duration of an entire input sequence is determined by the [max_input_duration](./Settings-Documentation.md#max_input_duration) setting. No input sequence can surpass this duration. For example, if the max duration is 60 seconds, typing "a61s" or "a30s b30001ms" will be equal to 60 seconds, so the input will fail.

The max input duration can be viewed with the `MaxInputDurCommand` (default: "!maxinputdur"), and the bot will often tell you when an input sequence exceeds the max duration.

Inputs may also fail depending on the client service. For instance, messages on Twitch cannot exceed 500 characters.

## Simultaneous Inputs

Append "+" at the end of an input to chain another input. "a+b" will press both "a" and "b" at the same time. "a+b+c" will press "a", "b", and "c" at the same time. There is no limit to how many inputs can be chained together, and each input can have its own duration. For example, "a200ms+b1s" will press both "a" and "b" at the same time, but "a" will be held for 200 milliseconds while "b" will be held for one second.

Example: "b+right1s down500ms left500ms+b50ms" ([Demonstration](https://clips.twitch.tv/DepressedHonestAntelopeKeyboardCat))

This can be used for more precise inputs. For example, "_a+right1s" may be used to jump and move a character to the right at the same time in a platformer.

For inputs on the same controller axis, such as an analog stick, the lattermost input takes precedence. For example, if "left" and "right" modify the same axis, "-right+left" will move the axis left, while "left+-right" will let go of the axis.

## Holding and Releasing Inputs

Release inputs by prepending "-" before the input name. If an input is held or ongoing, this will stop it early.

Hold inputs by prepending "_" before the input name. Inputs held this way stay held until either the end of the input sequence or until it sees a matching release input. "_a1s" will release the "a" input after 1 second, while "_a1s #400ms -a b" will release the "a" input after 1400 milliseconds.

Example: "_down plus . . . _left40% -left+_right #200ms -down _up300ms -right+_left -up" ([Demonstration](https://clips.twitch.tv/ThirstyNimbleDeerYouWHY))

Tip: Combine this with simultaneous inputs to hold or release multiple inputs at the same time.

## Delaying Inputs

As mentioned earlier, a blank, or wait input is available, with the default being "#" for pre-configured consoles. Since this input isn't used for anything, it can effectively be used to delay any other input or set of inputs. "right #100ms up" will press "right", wait 100 milliseconds, then press "up". 

Example: "#p right+up z #100ms cleft . . cleft #1000ms b z down400ms . a" ([Demonstration](https://clips.twitch.tv/CuriousPluckyCarrotKappaWealth))

**Note:** In the example above, "#p" is a macro, which will be covered later.

## Repeated Inputs

Inputs can be repeated a number of times in the form of [...]*#, with "#" as the number of repetitions. For example, "[a #100ms]*2" will be equivalent to "a #100ms a #100ms". The max number of repetitions is currently 999, but this may be changed in the future.

Example: "z _a #800ms left550ms _l -a . . [y #800ms]\*2 a . . [a1650ms]\*10" ([Demonstration](https://clips.twitch.tv/ResilientLuckyDurianGOWSkull))

**Note:** In the example above, "." is a synonym for "#", the wait input.

Repeated inputs can also be nested. For example, "[up [b]\*2]\*2" expands out to "up [b]\*2 up [b\*2]", which then expands to "up b b up b b". 

Repeated inputs can help mitigate the client service's character limit and make your inputs more concise and readable.

## Multi-Controller Inputs

TRBot's syntax supports passing inputs to different controllers in the same input string. For example, "&1right &2left" will press "right" for player 1 then "left" for player 2. This is useful in co-op or competitive games with multiple controllers in use.

Example: "start . &2_right &2b500ms+left2s b1s" ([Demonstration](https://clips.twitch.tv/ColdbloodedDarkDelicataItsBoshyTime))

**Note:** In the example above, the input was performed while the player was on controller port 1, hence the omission of "&1" for player 1 inputs.

Specifying an invalid controller port will cause the input to fail. The number of controllers in use can be viewed with the `ControllerCountCommand` (default: "!controllercount"), and a user can view or set their controller port with the `ControllerPortCommand` (default: "!port"). 

## Macros

Macros are a collection of inputs that can be assigned to the bot with the `AddMacroCommand` (default: "!addmacro"). Macros are stored in the **Macros** table in the database and can be viewed with the `ListMacrosCommand` (default: "!macros").

Macros start with "#" and have a name, which may be used to describe what the macro does. For example, "#masha" can be a macro for "[a34ms #34ms]\*20", which will press "a" many times very rapidly.

Example: "#farmstar" ([Demonstration](https://clips.twitch.tv/TenuousEasyOilKevinTurtle))

The example above demonstrates how powerful and complex a macro can be for accomplishing a task.

Macros can even contain other macros. For example, a macro named "#mashatwice" can be "#masha #masha".

Macros can be a very powerful asset, especially in more complex games such as 3D platformers, where the character may have moves that can be performed only via a sequence of inputs. Macros can be used to eliminate the tedium involved in remembering and performing such inputs.

## Dynamic Macros

Further expanding on macros are dynamic macros, which can accept variable inputs and use them as they see fit. They come in the generic form of "#macroname(\*,...)" with "\*" representing the arguments.

While being defined, the input arguments come in the form of "<#>" with "#" as the argument number, starting from 0. For example, "#mash(\*)" accepts one argument and can be "[<0>34ms #34ms]\*20". If using this macro, you can type "#mash(a)", which will replace "<0>" with "a", resulting in "[a34ms #34ms]\*20". Substituting "a" for "b" or any other input is also valid in this context.

Examples: "#mash(y)", "#mash(b)", "mashalternate(left, right)" ([Demonstration](https://clips.twitch.tv/VainFurtiveCoffeeStoneLightning))

It's possible to expand the earlier example further with an additional argument: "#mash(\*,\*)". If the new macro was then "[<0>34ms #34ms]\*<1>", calling the macro with "#mash(y,30)" will result in "[y34ms #34ms]\*30".

Dynamic macros are referred to by their generic form when used for other purposes, such as when adding or removing them, or viewing them with the `ShowMacroCommand` (default: "!showmacro"). For the example above, the generic form is "#mash(\*,\*)".

**Note that due to the nature of dynamic macros and the liberties with their syntax, the parser cannot validate them upon creation.** It is up to the players to verify that the dynamic macro works. Inputting a dynamic macro that doesn't parse will simply result in the input failing with no special error message. **Watch out for spaces between arguments while calling dynamic macros inside other macros!**

## Input Synonyms

While not part of TRBot's syntax, input synonyms can be used to define alternatives for existing inputs. They can be added with the `AddInputSynonymCommand` (default: "!addsyn") and are stored in the **InputSynonyms** table in the database. Input synonyms are defined on a per-console basis. A common input synonym is ".", which is a synonym for "#", the default wait input.

For example, in a game where the "a" button makes the character jump, one can make "jump" a synonym of "a". As a result, the parser will replace "jump" with "a", and the character will jump.

It's also possible to make more complex synonyms: for example, "slide" can be "_down a". This gives input synonyms a similar quality to macros, but they are not designed for such a purpose; thus, it's recommended to keep input synonyms as simple as possible.

Input synonyms will directly replace any match, which can break inputs. For example, if "triangle" is an input, with "a" being an input synonym, the final output would be "tritrianglengle", which is likely not a valid input. In these instances, it's recommended to instead add a new input with the same button and/or axis value so it parses correctly. You can view all input synonyms with the `ListInputSynonymsCommand` (default: "!listsyn").
