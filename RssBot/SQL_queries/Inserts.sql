USE RssBotDb
GO

/*
INSERT dbo.Persons(Name, Subscriptions)
VALUES('Nick', 'Sub1,Sub2')
*/

DECLARE @subname varchar(255);
SET @subname = 'Sub230';

DECLARE @username varchar(255);
SET @username = 'Nick';

IF EXISTS
(
	SELECT * from dbo.Persons
	WHERE Name = @username
	AND NOT Subscriptions LIKE CONCAT('%', @subname, '%')
)
BEGIN
    UPDATE dbo.Persons
	SET Subscriptions = CONCAT(Subscriptions, CONCAT(',', @subname))
	WHERE Name = @username
END
