﻿CREATE TABLE [dbo].[ResourceUrls]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [CultureCode] CHAR(5) NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Url] NVARCHAR(128) NOT NULL,
	CONSTRAINT [PK_ResourceUrl] PRIMARY KEY CLUSTERED ([Id] ASC)
)
