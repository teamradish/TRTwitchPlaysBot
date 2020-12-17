This page serves to document important database settings for TRBot. All these settings are in the Settings table of the database file, **TRBotData.db**.

Table of Contents
=================

* [main_thread_sleep](#main_thread_sleep)
* [credits_name](#credits_name)
* [credits_give_time](#credits_give_time)
* [credits_give_amount](#credits_give_amount)
* [duel_timeout](#duel_timeout)
* [group_bet_total_time](#group_bet_total_time)
* [group_bet_min_participants](#group_bet_min_participants)
* [chatbot_enabled](#chatbot_enabled)
* [chatbot_socket_path](#chatbot_socket_path)
* [chatbot_socket_path_is_relative](#chatbot_socket_path_is_relative)
* [bingo_enabled](#bingo_enabled)
* [bingo_pipe_path](#bingo_pipe_path)
* [bingo_pipe_path_is_relative](#bingo_pipe_path_is_relative)
* [client_service_type](#client_service_type)
* [auto_promote_enabled](#auto_promote_enabled)
* [auto_promote_level](#auto_promote_level)
* [auto_promote_input_req](#auto_promote_input_req)
* [bot_message_char_limit](#bot_message_char_limit)
* [periodic_message_time](#periodic_message_time)
* [message_cooldown](#message_cooldown)
* [connect_message](#connect_message)
* [reconnected_message](#reconnected_message)
* [periodic_message](#periodic_message)
* [auto_promote_message](#auto_promote_message)
* [new_user_message](#new_user_message)
* [being_hosted_message](#being_hosted_message)
* [new_subscriber_message](#new_subscriber_message)
* [resubscriber_message](#resubscriber_message)
* [source_code_message](#source_code_message)
* [game_message](#game_message)
* [game_message_path](#game_message_path)
* [game_message_path_is_relative](#game_message_path_is_relative)
* [info_message](#info_message)
* [tutorial_message](#tutorial_message)
* [documentation_message](#documentation_message)
* [periodic_input_enabled](#periodic_input_enabled)
* [periodic_input_time](#periodic_input_time)
* [periodic_input_port](#periodic_input_port)
* [periodic_input_value](#periodic_input_value)
* [teams_mode_enabled](#teams_mode_enabled)
* [teams_mode_max_port](#teams_mode_max_port)
* [teams_mode_next_port](#teams_mode_next_port)
* [default_input_duration](#default_input_duration)
* [max_input_duration](#max_input_duration)
* [global_mid_input_delay_enabled](#global_mid_input_delay_enabled)
* [global_mid_input_delay_time](#global_mid_input_delay_time)
* [last_console](#last_console)
* [last_vcontroller_type](#last_vcontroller_type)
* [joystick_count](#joystick_count)
* [global_input_level](#global_input_level)
* [first_launch](#first_launch)
* [force_init_defaults](#force_init_defaults)
* [data_version](#data_version)

### main_thread_sleep
Indicates how much time, in milliseconds, to sleep the main thread. This is used to throttle TRBot's main loop so it doesn't use up as much CPU time on your machine. Values too high may noticeably delay the execution of bot routines and messages.

### credits_name
The name to use for user credits.

### credits_give_time
The interval, in milliseconds, to give credits to users who have participated in chat within the time frame. If this value is less than 0, users won't be given credits.

### credits_give_amount
How many credits to give to users participating in chat within the time interval.

### duel_timeout
The time, in milliseconds, a user has to respond to a duel request before it is invalidated.

### group_bet_total_time
The total time, in milliseconds, a group bet takes place in.

### group_bet_min_participants
The minimum number of participants to start a group bet.

### chatbot_enabled
Whether users can talk with the chatbot.

### chatbot_socket_path
The path to the socket for the chatbot, which TRBot uses to communicate.

### chatbot_socket_path_is_relative
If 1, [chatbot_socket_path](#chatbot_socket_path) is a path relative to the Data folder, otherwise it's an absolute path.

### bingo_enabled
Whether users can participate in the external bingo application.

### bingo_pipe_path
The path to the socket for the bingo application, which TRBot uses to communicate.

### bingo_pipe_path_is_relative
If 1, [bingo_pipe_path](#bingo_pipe_path) is a path relative to the Data folder, otherwise it's an absolute path.

### client_service_type
The type of client service connection to use. 0 = Terminal, 1 = Twitch. **Requires restarting TRBot to apply.**

### auto_promote_enabled
If 1, users who reach a threshold of valid inputs will be automatically promoted to a given access level, if they haven't already. A value of 0 disables this.

### auto_promote_level
The access level to automatically promote users to.
### auto_promote_input_req
The number of valid inputs required for a user to be automatically promoted to.

### bot_message_char_limit
The character limit for the bot. This should often be set to the limit for the service you're deploying TRBot to. For example, this value should be 500 for Twitch. Bot messages longer than this value will often be split into separate messages.

### message_cooldown
Indicates how much time, in milliseconds, each message can be sent in max. This acts as a message throttler for platforms with rate-limiting on bots.

### periodic_message_time
The interval, in milliseconds, for TRBot to output the [periodic_message](#periodic_message).

### periodic_message
A message TRBot sends occasionally. The interval is determined by * [periodic_message_time](#periodic_message_time).

### connect_message
The message TRBot sends upon connecting to the service.

### reconnected_message
The message TRBot sends upon reconnecting from a disconnect.

### auto_promote_message
The message TRBot sends when a user gets autopromoted. Arguments: "{0}" = Username, "{1}" = Access level promoted to.

### new_user_message
The message TRBot sends when a user talks in chat for the first time. Arguments: "{0}" = Username.

### being_hosted_message
The message TRBot sends when another channel hosts yours. Used on Twitch and other services. Arguments: "{0}" = Channel name hosting yours.

### new_subscriber_message
The message TRBot sends when someone subscribes to your channel. Used on Twitch and other services. Arguments: "{0}" = Username.

### resubscriber_message
The message TRBot sends when someone re-subscribes to your channel. Used on Twitch and other services. Arguments: "{0}" = Username, "{1}" = Number of months subscribed.

### source_code_message
The message showing where users can obtain the source code of this instance and their rights under the AGPL 3.0+. If you have a custom fork of TRBot's source code, you must modify this message to link to your fork.

### game_message
An internal value for the game message users set while playing. This isn't modified manually.

### game_message_path
The path to the game message file used to display a message on stream.

### game_message_path_is_relative
If 1, [game_message_path](#game_message_path) is a path relative to the Data folder, otherwise it's an absolute path.

### info_message
An informational message about the current game or stream.

### tutorial_message
A message linking to the syntax tutorial on how to play.

### documentation_message
A message linking to the documentation for the bot.

### periodic_input_enabled
Whether to enable an input sequence that is performed periodically by TRBot. Periodic input sequences are useful for newer game consoles that go to sleep after some time of inactivity. 0 = disabled, 1 = enabled. This defaults to 0, disabled.

### periodic_input_time
The interval to perform the periodic input sequence, in milliseconds. This defaults to 300000 milliseconds, or 5 minutes.

### periodic_input_port
The controller port to perform the periodic input sequence on. You can avoid interfering with the game if you set this to another controller port that is otherwise unused. This defaults to 0 (port 1). 

### periodic_input_value
The input sequence to perform (Ex. "a"). This can also be an input macro or an input synonym. This defaults to an empty string, or no input.

### teams_mode_enabled
If 1 or greater, will enable teams mode, automatically assigning the value of [teams_mode_next_port](#teams_mode_next_port) as the controller port to new users, then incrementing it and wrapping it between 0 and [teams_mode_max_port](#teams_mode_max_port).

For example, if this is enabled and [teams_mode_max_port](#teams_mode_max_port) is 1, user1 will be assigned controller port 0, user2 will be port 1, user3 will be 0, and so on.

This defaults to 0 (disabled).

### teams_mode_max_port
The zero-based maximum controller port number to assign to new users if [teams_mode_enabled](#teams_mode_enabled) is 1 or greater. This defaults to 3 (port 4).

### teams_mode_next_port
Contains the zero-based controller port number to assign to new users if [teams_mode_enabled](#teams_mode_enabled) is 1 or greater. For example, 0 = controller port 1, and 1 = controller port 2.

Once this is assigned, its value is incremented and then wrapped to be between 0 and [teams_mode_max_port](#teams_mode_max_port) so the next new user is assigned a different controller port.

This defaults to 0 (port 1).

### default_input_duration
The global default duration of inputs with unspecified durations, in milliseconds. This defaults to 200 milliseconds.

### max_input_duration
The global max duration of any given input sequence, in milliseconds. This defaults to 60000 milliseconds. 

### global_mid_input_delay_enabled
If 1 or greater, will insert a blank input with a delay, in milliseconds, between each input. This delay is inserted only between inputs that either do not have any blank inputs or the blank input is not the longest in the subsequence. For example, if the delay is 200 milliseconds:
- "a200ms b200ms" will be modified to "a200ms #200ms b200ms".
- "a200ms #100ms b200ms" will not be modified because there is already a blank input in between.
- "a200ms+#100ms b200ms" will be modified to "a200ms+#100ms #200ms b200ms" because the blank input is not the longest in the "a200ms+#100ms" subsequence.
- "a200ms+#300ms b200ms" will not be modified because the blank input is the longest in the "a200ms+#300ms" subsequence.

This defaults to 0. The delay inserted is determined by [global_mid_input_delay_time](#global_mid_input_delay_time).

### global_mid_input_delay_time
The global time, in milliseconds, of the blank inputs inserted between each input. This does not apply if [global_mid_input_delay_enabled](#global_mid_input_delay_enabled) is 0 or lower. This defaults to 34 milliseconds.

### max_user_recent_inputs
The max number of recent input sequences to store per user. If the user is opted out of stats, it won't store any inputs. This defaults to 5.

### last_console
The game console to use.

### last_vcontroller_type
The type of virtual controller to use. If the one specified is not available on your platform, it will be switched to the default available one automatically. Defaults: Windows (vJoy), GNU/Linux (uinput)

### joystick_count
The number of virtual controllers to use. Values greater than 1 enable playing multiplayer games. This value will be automatically capped by the minimum and maximum values supported by the current virtual controller implementation.

### global_input_level
The global access level required to perform inputs. Users not at or above this level are refrained from making inputs.

### first_launch
Indicates the first ever launch of TRBot. This sets up all the default game consoles. Starts at 0 then gets set to 1.

### force_init_defaults
If 1, initializes all default values, including default commands, permissions, and settings, if the don't already exist.

### data_version
The version for TRBot's data. If this is behind the bot version being used, it will automatically set force_init_defaults to 1 and add missing data. Data versions are in the format "x.y.z", with the initial 2.0 release being "2.0.0".

