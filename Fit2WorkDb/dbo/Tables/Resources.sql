CREATE TABLE [dbo].[Resources]
(
	[Id] INT IDENTITY(1,1) NOT NULL, 
    [CultureCode] CHAR(5)						COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [EmailHeader] NVARCHAR(MAX)					COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [EmailFooter] NVARCHAR(MAX)					COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL, 
    [PrimaryEmailSubject] NVARCHAR(256)			COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL, 
    [PrimaryEmailBody] NVARCHAR(MAX)			COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL, 
    [SecondaryEmailSubject] NVARCHAR(256)		COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL, 
    [SecondaryEmailBody] NVARCHAR(MAX)			COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [UserMessageText] NVARCHAR(160)				COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[UserPrimaryMessageText] NVARCHAR(256)		COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [UserSecondaryMessageText] NVARCHAR(256)	COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL, 
    [DownloadMessageText] NVARCHAR(160)     	COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL , 
    CONSTRAINT [PK_Resources] PRIMARY KEY CLUSTERED ([Id] ASC)
)
