CREATE TABLE [dbo].[UserQuestionnaire]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [ClientId] INT NOT NULL, 
    [UserId] INT NOT NULL, 
    [QuestionsAndAnswersData] NVARCHAR(MAX) NOT NULL, 
    [CreatedDate] DATETIME NOT NULL,	
	CONSTRAINT [PK_UserQuestionnaire] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_UserQuestionnaire_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]),
	CONSTRAINT [FK_UserQuestionnaire_Client] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Client] ([Id])
)
