USE RssBotDb
GO

DECLARE @username varchar(255);
SET @username = 'Nick';

INSERT dbo.Persons(Name, Subscriptions)
VALUES(@username, '')