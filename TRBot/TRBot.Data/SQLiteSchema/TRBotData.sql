BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "settings" (
	"id"	INTEGER DEFAULT 0,
	"key"	TEXT DEFAULT "",
	"value_str"	TEXT DEFAULT "",
	"value_int"	INTEGER DEFAULT 0,
	PRIMARY KEY("id")
);
CREATE TABLE IF NOT EXISTS "users" (
	"name"	TEXT DEFAULT "",
	"level"	INTEGER DEFAULT 0,
	"total_messages"	INTEGER DEFAULT 0,
	"valid_inputs"	INTEGER DEFAULT 0,
	"silenced"	INTEGER DEFAULT 0,
	"port"	INTEGER DEFAULT 0,
	"subscriber"	INTEGER DEFAULT 0,
	"auto_promoted"	INTEGER DEFAULT 0,
	"opted_out"	INTEGER DEFAULT 0,
	"id"	INTEGER PRIMARY KEY AUTOINCREMENT
);
CREATE TABLE IF NOT EXISTS "memes" (
	"meme_name"	TEXT DEFAULT "",
	"meme_value"	TEXT DEFAULT "",
	"id"	INTEGER DEFAULT 0 PRIMARY KEY AUTOINCREMENT
);
CREATE TABLE IF NOT EXISTS "input_synonyms" (
	"synonym_name"	TEXT DEFAULT "",
	"synonym_value"	TEXT DEFAULT "",
	"id"	INTEGER DEFAULT 0 PRIMARY KEY AUTOINCREMENT
);
CREATE TABLE IF NOT EXISTS "input_macros" (
	"macro_name"	TEXT DEFAULT "",
	"macro_value"	TEXT DEFAULT "",
	"id"	INTEGER DEFAULT 0 PRIMARY KEY AUTOINCREMENT
);
CREATE TABLE IF NOT EXISTS "gamelogs" (
	"log_time"	TEXT DEFAULT "",
	"username"	TEXT DEFAULT "",
	"message"	TEXT DEFAULT "",
	"gamelog_id"	INTEGER DEFAULT 0 PRIMARY KEY AUTOINCREMENT
);
CREATE TABLE IF NOT EXISTS "consoles_axis_maps" (
	"console_id"	INTEGER DEFAULT 0,
	"valid_input_id"	INTEGER DEFAULT 0,
	"axis_min_val"	INTEGER DEFAULT 0,
	"axis_max_val"	INTEGER DEFAULT 1,
	"axis_max_percent"	INTEGER DEFAULT 100,
	"axis_value"	INTEGER DEFAULT 0
);
CREATE TABLE IF NOT EXISTS "consoles_button_maps" (
	"console_id"	INTEGER DEFAULT 0,
	"valid_input_id"	INTEGER DEFAULT 0,
	"button_value"	INTEGER DEFAULT 0
);
CREATE TABLE IF NOT EXISTS "consoles" (
	"id"	INTEGER DEFAULT 0 PRIMARY KEY AUTOINCREMENT,
	"name"	TEXT DEFAULT "GameConsole"
);
CREATE TABLE IF NOT EXISTS "consoles_valid_inputs" (
	"console_id"	INTEGER DEFAULT 0,
	"valid_input_id"	INTEGER DEFAULT 0
);
CREATE TABLE IF NOT EXISTS "valid_inputs" (
	"id"	INTEGER PRIMARY KEY AUTOINCREMENT,
	"value"	TEXT DEFAULT ""
);
INSERT INTO "settings" VALUES (0,'','',0);
INSERT INTO "settings" VALUES (1,'main_thread_sleep','',100);
INSERT INTO "settings" VALUES (2,'credits_name','Credits',0);
INSERT INTO "settings" VALUES (3,'credits_give_time','',120000);
INSERT INTO "settings" VALUES (4,'credits_give_amount','',100);
INSERT INTO "settings" VALUES (5,'chatbot_enabled','',0);
INSERT INTO "settings" VALUES (6,'chatbot_socket_path','',0);
INSERT INTO "settings" VALUES (7,'bingo_enabled','',0);
INSERT INTO "settings" VALUES (8,'bingo_pipe_path','',0);
INSERT INTO "settings" VALUES (9,'client_service_type','',0);
INSERT INTO "settings" VALUES (10,'auto_promote_enabled','',0);
INSERT INTO "settings" VALUES (11,'auto_promote_level','',1);
INSERT INTO "settings" VALUES (12,'auto_promote_input_req','',30);
COMMIT;
