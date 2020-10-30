-- Create wsa-account
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'anvilgroup\wsa-fit2work')
	CREATE LOGIN [anvilgroup\wsa-fit2work] 
	FROM WINDOWS 
	WITH DEFAULT_DATABASE=[Master], 
		 DEFAULT_LANGUAGE=[British]
		 
GO

-- Create WebUser role with permissions
CREATE ROLE [WebUser]
GO
GRANT SELECT ON SCHEMA::[dbo] TO [WebUser];
GRANT INSERT ON SCHEMA::[dbo] TO [WebUser];
GRANT UPDATE ON SCHEMA::[dbo] TO [WebUser];
GO

-- Add wsa user and put them in the WebUser Role
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.database_principals WHERE name = N'anvilgroup\wsa-fit2work')
	CREATE USER [anvilgroup\wsa-fit2work] FOR LOGIN [anvilgroup\wsa-fit2work] WITH DEFAULT_SCHEMA=[dbo]
GO
GRANT CONNECT TO [anvilgroup\wsa-fit2work];
GO
EXECUTE sp_addrolemember @rolename = N'WebUser', @membername = N'anvilgroup\wsa-fit2work';
GO

-- Add sa user and put them in the WebUser Role
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.database_principals WHERE name = N'anvilgroup\sa-fit2work')
	CREATE USER [anvilgroup\sa-fit2work] FOR LOGIN [anvilgroup\sa-fit2work] WITH DEFAULT_SCHEMA=[dbo]
GO
GRANT CONNECT TO [anvilgroup\sa-fit2work];
GO
EXECUTE sp_addrolemember @rolename = N'WebUser', @membername = N'anvilgroup\sa-fit2work';
GO

-- Create resource data
INSERT INTO [dbo].[Resources](
CultureCode, UserMessageText, EmailHeader, EmailFooter, 
PrimaryEmailSubject, PrimaryEmailBody, SecondaryEmailSubject, SecondaryEmailBody,
UserPrimaryMessageText, UserSecondaryMessageText)
VALUES(
'en-GB',
'Your company {clientName} MEMBER CODE is {memberCode}. Please download the Fit to Work App and register to start using the service. Thank you.',
'<table border="0" cellpadding="0" cellspacing="0" width="600" style="border-collapse: collapse;"><tr><td><img src="https://tag.anvilgroup.com/FitToWork/images/email-header.png" style="border:0;padding:0;margin:0;"/></td></tr></table>',
'<p><br/></p><table style="font-family:''Open Sans'', sans-serif;font-size:14px; padding:30px;" border="0" cellpadding="0" cellspacing="0" width="600" style="border-collapse: collapse;"><tr><td style="color:#00b5e3;font-weight:600;"> Confidentiality note:</td></tr><tr><td> The information contained in this message is legally privileged and confidential information intended only for the use of the individual or the entity named above. If the reader of this message is not the intended recipient, you are hereby notified that any use, dissemination, distribution or copying of this message is strictly prohibited. If you have received this message in error, please notify us immediately on +44 (0)20 7938 4221 and return the original message to us by e-mail to <a style="color:#00b5e3;" href="mailto:FitToWork@anvilgroup.com">FitToWork@anvilgroup.com</a> Thank you. The ANVIL Group (International) Limited. Registered Office: Vicarage House, 58-60 Kensington Church Street, London, W8 4DB. Registered in England & Wales No. 05429335.</td></tr></table><p><br/></p><table border="0" cellpadding="0" cellspacing="0" width="600" style="border-collapse: collapse;"><tr><td><img src="https://tag.anvilgroup.com/fittowork/images/email-footer.png" style="border:0;padding:0;margin:0;"/></td></tr></table>',
'{firstName} {lastName} will be coming into work today',
'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html xmlns="http://www.w3.org/1999/xhtml"><head><meta http-equiv="Content-Type" content="text/html; charset=UTF-8" /><title>Fit to work notification</title><meta name="viewport" content="width=device-width, initial-scale=1.0"/><link href="https://fonts.googleapis.com/css?family=Open+Sans" rel="stylesheet" type="text/css"></head><body style="font-family:''Open Sans'', sans-serif; margin: 0; padding: 0;"> {header}<table style="font-family:''Open Sans'', sans-serif;font-size:12pt; padding:30px;" border="0" cellpadding="0" cellspacing="0" width="600" style="border-collapse: collapse;"><tr><td align="center" style="text-align:center;"><img src="https://tag.anvilgroup.com/FitToWork/images/email-fittowork-icon.png" style="border:0;padding:0;margin:0;"/></td></tr><tr><td><p>This notification confirms that <strong style="font-family:''Open Sans'', sans-serif;font-size:12pt;">{firstName} {lastName}</strong> has completed today’s Fit to Work self-assessment and <strong style="font-family:''Open Sans'', sans-serif;font-size:12pt;">will be</strong> coming into work today.</p><p>Kind regards,<br/><br/>Anvil</p></td></tr></table>{footer}</body></html>',
'{firstName} {lastName} will not be coming into work today',
'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html xmlns="http://www.w3.org/1999/xhtml"><head><meta http-equiv="Content-Type" content="text/html; charset=UTF-8" /><title>Fit to work notification</title><meta name="viewport" content="width=device-width, initial-scale=1.0"/><link href="https://fonts.googleapis.com/css?family=Open+Sans" rel="stylesheet" type="text/css"></head><body style="font-family:''Open Sans'', sans-serif; margin: 0; padding: 0;"> {header}<table style="font-family:''Open Sans'', sans-serif;font-size:12pt; padding:30px;" border="0" cellpadding="0" cellspacing="0" width="600" style="border-collapse: collapse;"><tr><td align="center" style="text-align:center;"><img src="https://tag.anvilgroup.com/FitToWork/images/email-not-fittowork-icon.png" style="border:0;padding:0;margin:0;"/></td></tr><tr><td><p>This notification confirms that <strong style="font-family:''Open Sans'', sans-serif;font-size:12pt;">{firstName} {lastName}</strong> has completed today’s Fit to Work self-assessment and <strong style="font-family:''Open Sans'', sans-serif;font-size:12pt;">will not</strong> be coming into work today.</p><p>Kind regards<br/><br/>Anvil</p></td></tr></table>{footer}</body></html>',
'Based on the answers you have provided, you do not currently have any recognised COVID-19 symptoms. Please proceed with work today if fit to do so. Please inform us if at any point you feel unwell or have any concerns about your health.',
'Based on the answers you have provided, you may have one or more symptoms related to COVID-19. Please remain at home and someone at work will contact you. Please also consider contacting your doctor for medical advice concerning your condition.'
)

-- Create resource URLs
INSERT INTO [dbo].ResourceUrls(
CultureCode, [Name], [Url])
VALUES
('en-GB','NhsUrl','https://www.nhs.uk/conditions/coronavirus-covid-19/'),
('en-GB','CdcUrl','https://www.cdc.gov/coronavirus/2019-ncov/'),
('en-GB','WhoUrl','https://www.who.int/emergencies/diseases/novel-coronavirus-2019')

-- Create test anvil client
INSERT INTO [dbo].Client([Name],[MemberCode],[IsDeleted],[PrimaryEmailAddress],[SecondaryEmailAddress],[CreatedDate])
VALUES('Anvil Group','anvilgroup',0,'alarkin@anvilgroup.com','alarkin@anvilgroup.com',GETDATE())
-- Create user for anvil client
INSERT INTO [dbo].[User]([ClientId],[FirstName],[LastName],[PhoneNumber],[CreatedDate],[IsDeleted])
VALUES
(1,'Test','User','447003002001',GETDATE(),0),
(1,'Andrew','Larkin','447967698637',GETDATE(),0),
(1,'Diego','Cardoso','447762130866',GETDATE(),0),
(1,'Tim','Pollard','447432099713',GETDATE(),0)
