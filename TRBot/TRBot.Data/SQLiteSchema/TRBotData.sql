CREATE TABLE sqlite_sequence(name,seq);
CREATE TABLE IF NOT EXISTS "valid_inputs" (
	"id"	INTEGER PRIMARY KEY AUTOINCREMENT,
	"value"	TEXT DEFAULT ""
);
CREATE TABLE IF NOT EXISTS "consoles_valid_inputs" (
	"console_id"	INTEGER DEFAULT 0,
	"valid_input_id"	INTEGER DEFAULT 0
);
CREATE TABLE IF NOT EXISTS "auto_promote_settings" (
	"auto_promote_enabled"	INTEGER DEFAULT 0,
	"auto_promote_level"	INTEGER DEFAULT 1,
	"auto_promote_input_req"	INTEGER DEFAULT 20,
	PRIMARY KEY("auto_promote_enabled")
);
CREATE TABLE IF NOT EXISTS "bingo_settings" (
	"bingo_enabled"	INTEGER DEFAULT 0,
	"bingo_pipe_path"	TEXT DEFAULT "",
	PRIMARY KEY("bingo_enabled")
);
CREATE TABLE IF NOT EXISTS "chatbot_settings" (
	"chatbot_enabled"	INTEGER DEFAULT 0,
	"chatbot_socket_path"	TEXT DEFAULT "",
	PRIMARY KEY("chatbot_enabled")
);
CREATE TABLE IF NOT EXISTS "client_service_settings" (
	"client_service_type"	INTEGER DEFAULT 0,
	PRIMARY KEY("client_service_type")
);
CREATE TABLE IF NOT EXISTS "consoles" (
	"id"	INTEGER DEFAULT 0 PRIMARY KEY AUTOINCREMENT,
	"name"	TEXT DEFAULT "GameConsole"
);
CREATE TABLE IF NOT EXISTS "consoles_button_maps" (
	"console_id"	INTEGER DEFAULT 0,
	"valid_input_id"	INTEGER DEFAULT 0,
	"button_value"	INTEGER DEFAULT 0
);
CREATE TABLE IF NOT EXISTS "consoles_axis_maps" (
	"console_id"	INTEGER DEFAULT 0,
	"valid_input_id"	INTEGER DEFAULT 0,
	"axis_min_val"	INTEGER DEFAULT 0,
	"axis_max_val"	INTEGER DEFAULT 1,
	"axis_max_percent"	INTEGER DEFAULT 100,
	"axis_value"	INTEGER DEFAULT 0
);
CREATE TABLE IF NOT EXISTS "credits_settings" (
	"credits_name"	TEXT DEFAULT "Credits",
	"credits_give_time"	INTEGER DEFAULT 120000,
	"credits_give_amount"	INTEGER DEFAULT 100,
	PRIMARY KEY("credits_name")
);
CREATE TABLE IF NOT EXISTS "gamelogs" (
	"log_time"	TEXT DEFAULT "",
	"username"	TEXT DEFAULT "",
	"message"	TEXT DEFAULT "",
	"gamelog_id"	INTEGER DEFAULT 0 PRIMARY KEY AUTOINCREMENT
);
CREATE TABLE IF NOT EXISTS "input_macros" (
	"macro_name"	TEXT DEFAULT "",
	"macro_value"	TEXT DEFAULT "",
	"id"	INTEGER DEFAULT 0 PRIMARY KEY AUTOINCREMENT
);
CREATE TABLE IF NOT EXISTS "input_synonyms" (
	"synonym_name"	TEXT DEFAULT "",
	"synonym_value"	TEXT DEFAULT "",
	"id"	INTEGER DEFAULT 0 PRIMARY KEY AUTOINCREMENT
);
CREATE TABLE IF NOT EXISTS "memes" (
	"meme_name"	TEXT DEFAULT "",
	"meme_value"	TEXT DEFAULT "",
	"id"	INTEGER DEFAULT 0 PRIMARY KEY AUTOINCREMENT
);
CREATE TABLE IF NOT EXISTS "performance_settings" (
	"main_thread_sleep"	INTEGER DEFAULT 100,
	PRIMARY KEY("main_thread_sleep")
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
CREATE TABLE IF NOT EXISTS "message_settings" (
	"id"	INTEGER,
	"key"	TEXT,
	"value_str"	INTEGER,
	"value_int"	INTEGER,
	PRIMARY KEY("id")
);
