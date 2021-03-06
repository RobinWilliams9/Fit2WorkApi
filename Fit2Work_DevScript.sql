Use [Fit2Work]

-- Set all users & questionnaires for a given client
DECLARE @ClientName NVARCHAR(50)
SET @ClientName = 'Anvil Group'
SELECT * FROM Client WHERE [Name] LIKE '%' + @ClientName + '%'
SELECT TOP 10 * 
	FROM [User] u
WHERE ClientId = (SELECT Id FROM Client WHERE [Name] LIKE '%' + @ClientName + '%')
ORDER BY CreatedDate DESC
SELECT TOP 10 * 
	FROM UserQuestionnaire uq
WHERE ClientId = (SELECT Id FROM Client WHERE [Name] LIKE '%' + @ClientName + '%')
ORDER BY CreatedDate DESC

-- Recent questionnaires
SELECT TOP 10 '-' AS ALL_RecentQuestionnaires, * FROM UserQuestionnaire uq ORDER BY CreatedDate DESC

-- Recent log messages
-- DELETE FROM [Fit2Work].[dbo].[Log]
SELECT TOP 10  '-' AS ALL_RecentLogMessages, * FROM [Fit2Work].[dbo].[Log] Order by CreatedDate DESC

-- See email templates
SELECT '-' AS Resources, * FROM [Fit2Work].[dbo].[Resources]

/*
-- Create dummy users for clients
SELECT * FROM [Client]
INSERT INTO [dbo].[User]([ClientId],[FirstName],[LastName],[PhoneNumber],[CreatedDate])
VALUES
(1,'Andrew','Larkin','07967698637',GETDATE()),
(1,'Diego','Cardoso','07762130866',GETDATE()),
(1,'Tim','Pollard','07432099713',GETDATE())
*/

-- Change client emails for testing
/*
DECLARE @Email NVARCHAR(50)
SET @Email = 'dcardoso@anvilgroup.com;alarkin@anvilgroup.com'
UPDATE Client SET PrimaryEmailAddress = @Email, SecondaryEmailAddress = @Email WHERE Id = 1
*/