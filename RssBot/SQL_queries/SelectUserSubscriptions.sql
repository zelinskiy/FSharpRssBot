USE RssBotDb
GO

DECLARE @username varchar(255);
SET @username = 'Nick';

SELECT Subscriptions from dbo.Persons
WHERE Name = @username