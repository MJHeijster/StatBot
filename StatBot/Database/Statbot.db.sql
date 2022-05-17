BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Database" (
	"Version"	INTEGER
);
CREATE TABLE IF NOT EXISTS "EmbedType" (
	"Id"	INTEGER,
	"Type"	TEXT,
	PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "Servers" (
	"Id"	INTEGER,
	"Name"	TEXT
);
CREATE TABLE IF NOT EXISTS "Channels" (
	"Id"	INTEGER,
	"Name"	TEXT,
	"ServerId"	INTEGER,
	FOREIGN KEY("ServerId") REFERENCES "Servers"("Id"),
	PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "Embeds" (
	"Id"	INTEGER,
	"MessageId"	INTEGER,
	"Uri"	TEXT,
	"EmbedType"	INTEGER,
	"Thumbnail"	TEXT,
	FOREIGN KEY("EmbedType") REFERENCES "EmbedType"("Id"),
	FOREIGN KEY("MessageId") REFERENCES "Messages"("Id"),
	PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "MentionedUsers" (
	"MessageId"	INTEGER,
	"UserId"	INTEGER,
	FOREIGN KEY("UserId") REFERENCES "Users"("Id"),
	FOREIGN KEY("MessageId") REFERENCES "Messages"("Id")
);
CREATE TABLE IF NOT EXISTS "Attachments" (
	"Id"	INTEGER,
	"Filename"	INTEGER,
	"Uri"	INTEGER,
	"MessageId"	INTEGER,
	FOREIGN KEY("MessageId") REFERENCES "Messages"("Id"),
	PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "Emojis" (
	"Id"	INTEGER,
	"Text"	TEXT
);
CREATE TABLE IF NOT EXISTS "EmojisUsed" (
	"MessageId"	INTEGER,
	"EmojiId"	INTEGER,
	FOREIGN KEY("EmojiId") REFERENCES "Emojis"("Id"),
	FOREIGN KEY("MessageId") REFERENCES "Messages"("Id")
);
CREATE TABLE IF NOT EXISTS "Messages" (
	"Id"	INTEGER,
	"Text"	TEXT,
	"UserId"	INTEGER,
	FOREIGN KEY("UserId") REFERENCES "Users"("Id"),
	PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "Users" (
	"Id"	INTEGER UNIQUE,
	"Username"	TEXT NOT NULL,
	"Discrim"	TEXT NOT NULL,
	"AvatarUri"	TEXT,
	"IsBot"	INTEGER DEFAULT 0,
	"IsExcludedFromStats"	INTEGER DEFAULT 0,
	PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "OldUsers" (
	"Id"	INTEGER,
	"Username"	TEXT,
	"Discrim"	TEXT,
	"DateTimeChanged"	TEXT,
	FOREIGN KEY("Id") REFERENCES "Users"("Id"),
	UNIQUE("Id","Username","Discrim")
);
CREATE TRIGGER User_Renamed
AFTER Insert ON Users
BEGIN
	insert into OldUsers (Id, Username, Discrim, DateTimeChanged) 
	values(new.Id, new.Username, new.Discrim, DATETIME('now'));
END;
COMMIT;
