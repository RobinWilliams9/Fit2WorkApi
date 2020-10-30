CREATE ROLE [WebUser]
    AUTHORIZATION [dbo];


GO
ALTER ROLE [WebUser] ADD MEMBER [anvilgroup\sa-fit2work];


GO
ALTER ROLE [WebUser] ADD MEMBER [anvilgroup\wsa-fit2work];

