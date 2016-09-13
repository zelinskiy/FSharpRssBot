USE RssBotDb;
GO

IF EXISTS(
	SELECT * from RssBotDb.INFORMATION_SCHEMA.TABLES 
	WHERE TABLE_NAME = 'Persons')
BEGIN
    DROP TABLE Persons;
END

CREATE TABLE Persons
(
	Name varchar(255) NOT NULL,
	Subscriptions varchar(255) NOT NULL,
	PRIMARY KEY (Name)
);