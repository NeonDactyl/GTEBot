CREATE TABLE elosubmissionDatabase.EloSubmissions (
	Id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
	GameUrl VARCHAR(255),
	Color INT,
	Pgn TEXT,
	IsValid BIT,
	ErrorMessage VARCHAR(511),
	SourceDiscordUserId BIGINT UNSIGNED,
	DiscordUserName VARCHAR(255),
	SourceDiscordChannelId BIGINT UNSIGNED,
	SubmissionTime DATETIME,
	IsActive BIT
	);

CREATE TABLE elosubmissionDatabase.AllowedChannels (
	ChannelId BIGINT UNSIGNED PRIMARY KEY,
	Allowed BIT,
	DefaultRole BIGINT UNSIGNED
	);

INSERT INTO elosubmissionDatabase.AllowedChannels (ChannelId, Allowed) VALUES (881581709756338259, 1, 889685958121914478)