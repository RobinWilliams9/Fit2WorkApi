CREATE TABLE [dbo].[User]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [ClientId] INT NOT NULL, 
    [FirstName] NVARCHAR(128) NOT NULL, 
    [LastName] NVARCHAR(128) NOT NULL, 
    [PhoneNumber] NVARCHAR(50) NOT NULL, 
    [CreatedDate] DATETIME NOT NULL, 
    [UpdatedDate] DATETIME NULL,	
	[RegisteredDate] DATETIME NULL, 
    [SmsSentDate] DATETIME NULL, 
    [IsDeleted] BIT NOT NULL DEFAULT 0, 
    [ReminderSettings] VARCHAR(200) NULL, 
    [ReminderSettingsUpdatedDate] DATETIME NULL, 
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_User_Client] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Client] ([Id])
)
