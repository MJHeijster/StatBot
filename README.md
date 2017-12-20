# StatBot
A Discord Bot which logs in the HydraIRC complient log format so that mIRCStats will work. Slightly modified parser can be found in the mIRCStats parser folder.

# Setup
Compile and place in a location you want the software in. Modify the token in the App.config file. More information can be found here about the token: https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token

# Tips
If a channel is renamed, you can use a script to move the old and new log into a folder. If you specify the channel name as such in the first log (for this example, 1.log): "*** Now talking in #channelname", mIRCStats will recognize it as that name.
Example batch script for combining multiple logs:
mkdir C:\Discord\StatBot\277418737798479872\general
copy "C:\Discord\StatBot\277418737798479872\general.log" "C:\Discord\StatBot\277418737798479872\general\1.log" /Y
copy "C:\Discord\StatBot\277418737798479872\the-toy-factory.log" "C:\Discord\StatBot\277418737798479872\general\2.log" /Y
START /WAIT C:\Discord\StatBot\mircstats\mircstats.exe -cfg general.cfg -log "C:\Discord\StatBot\277418737798479872\general\*.log" -html Html\general.html

Font used for the icon: Archicoco