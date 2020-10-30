CREATE TABLE [dbo].[Client]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [Name] NVARCHAR(128) NOT NULL, 
	[MemberCode] NVARCHAR(50) NOT NULL,
    [IsDeleted] BIT NOT NULL, 
    [PrimaryEmailAddress] NVARCHAR(128) NOT NULL, 
    [SecondaryEmailAddress] NVARCHAR(128) NOT NULL, 
    [CreatedDate] DATETIME NOT NULL, 
    [UpdatedDate] DATETIME NULL,	
	CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED ([Id] ASC)
)
