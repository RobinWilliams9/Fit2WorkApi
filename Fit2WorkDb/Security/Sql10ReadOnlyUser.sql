
-- added this code
CREATE LOGIN [Sql10ReadOnlyUser]
    WITH PASSWORD = N'1iw9pe|3khhqflnbdvhtnzm{msFT7_&#$!~<b8kFSkkvtDer', SID = 0x00E64D5BB1470B4EAD64BD83D961F0B5, DEFAULT_LANGUAGE = [british], CHECK_POLICY = OFF;
GO

-- original file from TFS (breaks build on its own)
CREATE USER [Sql10ReadOnlyUser] FOR LOGIN [Sql10ReadOnlyUser];

