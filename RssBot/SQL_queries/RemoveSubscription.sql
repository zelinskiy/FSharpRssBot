USE RssBotDb
GO

DECLARE @subname varchar(255);
SET @subname = 'Sub231';

DECLARE @username varchar(255);
SET @username = 'Nick';

IF EXISTS
(
	SELECT * from dbo.Persons
	WHERE Name = @username
	AND Subscriptions LIKE CONCAT('%', @subname, '%')
)
BEGIN
    UPDATE dbo.Persons
	SET Subscriptions = REPLACE(Subscriptions, CONCAT(',', @subname), '')
	WHERE Name = @username
END