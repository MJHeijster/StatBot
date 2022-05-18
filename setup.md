## Setup
# Setup
Compile (or download) and place in a location you want the software in. Configure the application through the appsettings.json file. More information can be found here about the token: https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token

You can either let the bot log to channelname.log or channelid.log. You can figure out what channel belongs to a channelid by opening the log and checking the first line, or by writing \#channelname in Discord. It will show something like <#296923759988703232> where 296923759988703232 is the id.

If you use StatBot to generate the stats through GeneratorFile, make sure to run StatBot through your own account or one that has the FTP credentials uploaded if you use FTP. The FTP credentials are encrypted in mIRCStats, so other accounts will not be able to upload.
