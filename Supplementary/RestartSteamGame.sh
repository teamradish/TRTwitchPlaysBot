# This script will launch Steam with a given app ID and timeout, launching the game
# NOTE: It's advised to run this in a terminal window
# Once the game is closed, it'll completely kill Steam and restart the loop
# To completely kill this, you can spam Ctrl + C in the terminal window or find the process manually and kill it

# Credit goes to ejthill for the bulk of the script:
# https://github.com/ValveSoftware/steam-for-linux/issues/1721#issuecomment-282413139

# This script requires xdotool (`sudo apt install xdotool`)
# Usage `./RestartSteamGame.sh appid launchtimeout`

app=$1
launchtimeout=$2
#If launchtimeout not specified, use default of 5
if [[ -z "$launchtimeout" ]]; then
  launchtimeout=5
fi

# Keep relaunching Steam and the game
(while true; do
    #Lookup current active window
    launchwindow=$(xdotool getwindowfocus getwindowname)
    #Launch Application
    steam -silent -applaunch $app &
    #Give steam time to start and launch a new window
    sleep $launchtimeout
    #Wait until the focus returns to the original window
    while [ "$focus" != "$launchwindow" ]
    do
      focus=$(xdotool getwindowfocus getwindowname)
      sleep 1
    done

    # Uncomment to completely kill Steam
    pgrep steam | while read pid; do kill $pid; done

    # Sleep for a little after killing Steam to ensure it's gone    
    sleep 3
done)
