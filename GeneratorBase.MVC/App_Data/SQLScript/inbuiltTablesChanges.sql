SET ANSI_WARNINGS OFF


IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_JournalEntry')
                    AND name = 'PropertyName' ) 
ALTER TABLE tbl_JournalEntry ADD PropertyName nvarchar(MAX) NULL

GO

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_JournalEntry')
                    AND name = 'BrowserInfo' ) 
ALTER TABLE tbl_JournalEntry ADD BrowserInfo nvarchar(MAX) NULL

GO

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_JournalEntry')
                    AND name = 'OldValue' ) 
ALTER TABLE tbl_JournalEntry ADD OldValue nvarchar(MAX) NULL

GO

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_JournalEntry')
                    AND name = 'NewValue' ) 
ALTER TABLE tbl_JournalEntry ADD NewValue nvarchar(MAX) NULL

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_JournalEntry')
                    AND name = 'DisplayValue' ) 
ALTER TABLE tbl_JournalEntry ADD DisplayValue nvarchar(MAX) NULL
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_JournalEntry')
                    AND name = 'ConcurrencyKey' ) 
ALTER TABLE tbl_JournalEntry ADD ConcurrencyKey timestamp NOT NULL
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_JournalEntry')
                    AND name = 'RoleName' ) 
ALTER TABLE tbl_JournalEntry ADD RoleName nvarchar(MAX) NULL
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_JournalEntry')
                    AND name = 'Source' ) 
ALTER TABLE tbl_JournalEntry ADD Source nvarchar(MAX) NULL
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_JournalEntry')
                    AND name = 'Reason' ) 
ALTER TABLE tbl_JournalEntry ADD Reason nvarchar(MAX) NULL
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_JournalEntry')
                    AND name = 'Tenant' ) 
ALTER TABLE tbl_JournalEntry ADD Tenant [bigint] NULL
GO

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ActionType')
                    AND name = 'Template' ) 
ALTER TABLE tbl_ActionType ADD Template nvarchar(MAX) NULL
GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ActionType')
                    AND name = 'OriginalTemplate' ) 
ALTER TABLE tbl_ActionType ADD OriginalTemplate nvarchar(MAX) NULL
GO

if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '1') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (1,'Lock Record','Make Record Non-Editable if rule condition satisfies','Lock Record','<div id=''DisplayMessage''><div id=''trRuleRLockRecord'' class=''DisplayMessageTitle'' ><span class=''fa fa-exclamation-triangle''></span> Record Locked: </div><label id=''ErrmsgLockRecord''> ###Message### </label></div><script>$(''#trRuleRLockRecord'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);}); </script>','<div id=''DisplayMessage''><div id=''trRuleRLockRecord'' class=''DisplayMessageTitle'' ><span class=''fa fa-exclamation-triangle''></span> Record Locked: </div><label id=''ErrmsgLockRecord''> ###Message### </label></div><script>$(''#trRuleRLockRecord'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);}); </script>')
end
else
begin
UPDATE tbl_ActionType SET Template='<div id=''DisplayMessage''><div id=''trRuleRLockRecord'' class=''DisplayMessageTitle'' ><span class=''fa fa-exclamation-triangle''></span> Record Locked: </div><label id=''ErrmsgLockRecord''> ###Message### </label></div><script>$(''#trRuleRLockRecord'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);}); </script>'
WHERE TypeNo=1 and Template is null
end
begin
UPDATE tbl_ActionType SET OriginalTemplate='<div id=''DisplayMessage''><div id=''trRuleRLockRecord'' class=''DisplayMessageTitle'' ><span class=''fa fa-exclamation-triangle''></span> Record Locked: </div><label id=''ErrmsgLockRecord''> ###Message### </label></div><script>$(''#trRuleRLockRecord'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);}); </script>'
WHERE TypeNo=1 and OriginalTemplate is null
end
GO
if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '2') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (2,'Mandatory Properties','Make Some Properties Mandatory if rule condition satisfies','Mandatory Properties','<div id=''DisplayMessage''><div id=''trRuleMandatory'' class=''DisplayMessageTitle''><span class=''fa fa-asterisk''></span> Mandatory Fields: </div><label id=''ErrMsgMandatory''> ###Message### </label></div> <script>$(''#trRuleMandatory'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);});</script>','<div id=''DisplayMessage''><div id=''trRuleMandatory'' class=''DisplayMessageTitle''><span class=''fa fa-asterisk''></span> Mandatory Fields: </div><label id=''ErrMsgMandatory''> ###Message### </label></div> <script>$(''#trRuleMandatory'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);});</script>')
end
else
begin
UPDATE tbl_ActionType SET Template='<div id=''DisplayMessage''><div id=''trRuleMandatory'' class=''DisplayMessageTitle''><span class=''fa fa-asterisk''></span> Mandatory Fields: </div><label id=''ErrMsgMandatory''> ###Message### </label></div> <script>$(''#trRuleMandatory'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);});</script>'
WHERE TypeNo=2 and Template is null
end
begin
UPDATE tbl_ActionType SET OriginalTemplate='<div id=''DisplayMessage''><div id=''trRuleMandatory'' class=''DisplayMessageTitle''><span class=''fa fa-asterisk''></span> Mandatory Fields: </div><label id=''ErrMsgMandatory''> ###Message### </label></div> <script>$(''#trRuleMandatory'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);});</script>'
WHERE TypeNo=2 and OriginalTemplate is null
end
GO
if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '3') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (3,'Send Email','Send Email Notification after save if rule condition satisfies','Send Email',NULL,NULL)
end

GO
if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '4') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (4,'ReadOnly Properties','Make some properties readonly if rule condition satisfies','ReadOnly Properties',NULL,NULL)
end
else
begin
UPDATE tbl_ActionType SET TypeNo=4,ActionTypeName='ReadOnly Properties',Description='Make some properties readonly if rule condition satisfies',DisplayValue='ReadOnly Properties'
WHERE TypeNo=4
end
GO

if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '5') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (5,'Filter Dropdown','Filter Dropdown','Filter Dropdown',NULL,NULL)
end
GO

if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '6') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (6,'Hidden Properties','Make Some Properties Hidden if rule condition satisfies','Hidden Properties',NULL,NULL)
end

if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '7') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (7,'Set Value','Set Value','Set Value',NULL,NULL)
end

if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '8') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (8,'Invoke Action','Invoke Action','Invoke Action',NULL,NULL)
end

if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '9') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (9,'Scheduled','Scheduled Task','Scheduled',NULL,NULL)
end

if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '10') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (10,'ValidateBeforeSave','Validate Before Save','ValidateBeforeSave','<div id=''DisplayMessage''><div id=''trRuleBeforeSaveProp'' class=''DisplayMessageTitle'' ><span class=''fa fa-exclamation-triangle''></span> Alert: </div><label id=''ErrMsgRuleBeforeSaveProp''> ###Message### </label></div><script>$(''#trRuleBeforeSaveProp'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);});</script>','<div id=''DisplayMessage''><div id=''trRuleBeforeSaveProp'' class=''DisplayMessageTitle'' ><span class=''fa fa-exclamation-triangle''></span> Alert: </div><label id=''ErrMsgRuleBeforeSaveProp''> ###Message### </label></div><script>$(''#trRuleBeforeSaveProp'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);});</script>')
end
else
begin
UPDATE tbl_ActionType SET Template='<div id=''DisplayMessage''><div id=''trRuleBeforeSaveProp'' class=''DisplayMessageTitle'' ><span class=''fa fa-exclamation-triangle''></span> Alert: </div><label id=''ErrMsgRuleBeforeSaveProp''> ###Message### </label></div><script>$(''#trRuleBeforeSaveProp'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);});</script>'
WHERE TypeNo=10 and Template is null
end
begin
UPDATE tbl_ActionType SET OriginalTemplate='<div id=''DisplayMessage''><div id=''trRuleBeforeSaveProp'' class=''DisplayMessageTitle'' ><span class=''fa fa-exclamation-triangle''></span> Alert: </div><label id=''ErrMsgRuleBeforeSaveProp''> ###Message### </label></div><script>$(''#trRuleBeforeSaveProp'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);});</script>'
WHERE TypeNo=10 and OriginalTemplate is null
end

if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '11') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (11,'Make Record & Associations Readonly','Make Record & Associations Readonly','Make Record & Associations Readonly','<div id=''DisplayMessage''><div id=''trRuleRLockRecord'' class=''DisplayMessageTitle'' ><span class=''fa fa-rocket''></span> Record Locked: </div><label id=''ErrmsgLockRecord''> ###Message### </label></div><script>$(''#trRuleRLockRecord'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);}); </script>','<div id=''DisplayMessage''><div id=''trRuleRLockRecord'' class=''DisplayMessageTitle'' ><span class=''fa fa-rocket''></span> Record Locked: </div><label id=''ErrmsgLockRecord''> ###Message### </label></div><script>$(''#trRuleRLockRecord'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);}); </script>')
end
else
begin
UPDATE tbl_ActionType SET Template='<div id=''DisplayMessage''><div id=''trRuleRLockRecord'' class=''DisplayMessageTitle'' ><span class=''fa fa-rocket''></span> Record Locked: </div><label id=''ErrmsgLockRecord''> ###Message### </label></div><script>$(''#trRuleRLockRecord'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);}); </script>'
WHERE TypeNo=11 and Template is null
end
begin
UPDATE tbl_ActionType SET OriginalTemplate='<div id=''DisplayMessage''><div id=''trRuleRLockRecord'' class=''DisplayMessageTitle'' ><span class=''fa fa-rocket''></span> Record Locked: </div><label id=''ErrmsgLockRecord''> ###Message### </label></div><script>$(''#trRuleRLockRecord'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);}); </script>'
WHERE TypeNo=11 and OriginalTemplate is null
end
if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '12') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (12,'Hidden Groups','Make Some Groups Hidden if rule condition satisfies','Hidden Groups',NULL,NULL)
end

if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '13') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (13,'UIAlert','Shows Alert if rule condition satisfies.','UIAlert','<div id=''DisplayMessage''><div id=''trRuleBeforeSaveProp'' class=''DisplayMessageTitle'' ><span class=''fa fa-exclamation-triangle''></span> Alert: </div><label id=''ErrMsgRuleBeforeSaveProp''>###Message###</label></div><script>$(''#trRuleBeforeSaveProp'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);});</script>','<div id=''DisplayMessage''><div id=''trRuleBeforeSaveProp'' class=''DisplayMessageTitle'' ><span class=''fa fa-exclamation-triangle''></span> Alert: </div><label id=''ErrMsgRuleBeforeSaveProp''>###Message###</label></div><script>$(''#trRuleBeforeSaveProp'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);});</script>')
end
else
begin
UPDATE tbl_ActionType SET Template='<div id=''DisplayMessage''><div id=''trRuleBeforeSaveProp'' class=''DisplayMessageTitle'' ><span class=''fa fa-exclamation-triangle''></span> Alert: </div><label id=''ErrMsgRuleBeforeSaveProp''>###Message###</label></div><script>$(''#trRuleBeforeSaveProp'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);});</script>'
WHERE TypeNo=13 and Template is null
end
begin
UPDATE tbl_ActionType SET OriginalTemplate='<div id=''DisplayMessage''><div id=''trRuleBeforeSaveProp'' class=''DisplayMessageTitle'' ><span class=''fa fa-exclamation-triangle''></span> Alert: </div><label id=''ErrMsgRuleBeforeSaveProp''>###Message###</label></div><script>$(''#trRuleBeforeSaveProp'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);});</script>'
WHERE TypeNo=13 and OriginalTemplate is null
end
if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '14') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (14,'Invoke Webhook','Invoke Webhook (REST Call)','Invoke Webhook',NULL,NULL)
end
if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '15') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (15,'ConfirmationBeforeSave','Confirmation Before Save is a alert pop up when save update','Confirmation Before Save','<div id=''DisplayMessage''><div id=''trRuleBeforeSaveProp'' class=''DisplayMessageTitle'' ><span class=''fa fa-exclamation-triangle''></span> Alert: </div><label id=''ErrMsgRuleBeforeSaveProp''> ###Message### </label></div><script>$(''#trRuleBeforeSaveProp'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);});</script>','<div id=''DisplayMessage''><div id=''trRuleBeforeSaveProp'' class=''DisplayMessageTitle'' ><span class=''fa fa-exclamation-triangle''></span> Alert: </div><label id=''ErrMsgRuleBeforeSaveProp''> ###Message### </label></div><script>$(''#trRuleBeforeSaveProp'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);});</script>')
end
else
begin
UPDATE tbl_ActionType SET Template='<div id=''DisplayMessage''><div id=''trRuleBeforeSaveProp'' class=''DisplayMessageTitle'' ><span class=''fa fa-exclamation-triangle''></span> Alert: </div><label id=''ErrMsgRuleBeforeSaveProp''> ###Message### </label></div><script>$(''#trRuleBeforeSaveProp'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);});</script>'
WHERE TypeNo=15 and Template is null
end
begin
UPDATE tbl_ActionType SET OriginalTemplate='<div id=''DisplayMessage''><div id=''trRuleBeforeSaveProp'' class=''DisplayMessageTitle'' ><span class=''fa fa-exclamation-triangle''></span> Alert: </div><label id=''ErrMsgRuleBeforeSaveProp''> ###Message### </label></div><script>$(''#trRuleBeforeSaveProp'').click(function () {$(this).toggleClass(''expand'').nextUntil(''tr.header'').slideToggle(500);});</script>'
WHERE TypeNo=15 and OriginalTemplate is null
end
GO
--
if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '16') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (16,'MakeVerbsHidden','Hide Verb(s) Single/Bulk on add/update','Make Verbs  Hidden',NULL,NULL)
end
GO

if exists (select * from sysobjects where name='tbl_ActionType' and xtype='U') 
if(Select count(*) from tbl_ActionType Where [TypeNo] = '17') <= '0'
begin
INSERT INTO tbl_ActionType (TypeNo,ActionTypeName,Description,DisplayValue,Template,OriginalTemplate)
VALUES (17,'RestrictDropdown','Restrict Dropdown Items On Entity Page','Restrict Dropdown',NULL,NULL)
end
--

if not exists (select * from sysobjects where name='tbl_Document' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_Document](
      [Id] [bigint] IDENTITY(1,1) NOT NULL,
      [ConcurrencyKey] [timestamp] NOT NULL,
      [DocumentName] [nvarchar](255) NULL,
      [DateCreated] [datetime] NULL,
      [DateLastUpdated] [datetime] NULL,
      [Description] [nvarchar](255) NULL,
      [DisplayValue] [nvarchar](255) NULL,
      [FileExtension] [nvarchar](255) NULL,
      [FileName] [nvarchar](255) NULL,
      [FileSize] [bigint] NULL,
      [MIMEType] [nvarchar](255) NULL,
      [SearchableText] [nvarchar](255) NULL,
      [Byte] [varbinary](max) NULL,
	  [EntityName] [nvarchar](255) NULL,
      [FileType] [nvarchar](255) NULL,
	  [IsDeleted] [bit] NULL,
	  [DeleteDateTime] [datetime] NULL,
	  [ExportDataLogId] [bigint] NULL
CONSTRAINT [PK_tbl_Document] PRIMARY KEY CLUSTERED 
(
      [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end

GO
-- tbl_Document
	IF NOT EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_Document') AND name = 'EntityName' ) 
	BEGIN
		ALTER TABLE tbl_Document ADD EntityName [nvarchar](255) NULL
	END

	IF NOT EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_Document') AND name = 'FileType' ) 
	BEGIN
		ALTER TABLE tbl_Document ADD FileType [nvarchar](255) NULL
	END

	IF NOT EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_Document') AND name = 'IsDeleted' ) 
	BEGIN
		ALTER TABLE tbl_Document ADD IsDeleted [bit] NULL
	END

	IF NOT EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_Document') AND name = 'DeleteDateTime' ) 
	BEGIN
		ALTER TABLE tbl_Document ADD DeleteDateTime [datetime] NULL
	END

	IF NOT EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_Document') AND name = 'ExportDataLogId' ) 
	BEGIN
		ALTER TABLE tbl_Document ADD ExportDataLogId [bigint] NULL
	END


IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('AspNetUsers') AND name = 'EmailConfirmed')
ALTER TABLE AspNetUsers ADD EmailConfirmed bit NOT NULL DEFAULT '0'
IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('AspNetUsers') AND name = 'PhoneNumber')
ALTER TABLE AspNetUsers ADD PhoneNumber NVARCHAR(50) NULL 
IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('AspNetUsers') AND name = 'PhoneNumberConfirmed')
ALTER TABLE AspNetUsers ADD PhoneNumberConfirmed bit NOT NULL DEFAULT '0'
IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('AspNetUsers') AND name = 'TwoFactorEnabled')
ALTER TABLE AspNetUsers ADD TwoFactorEnabled bit NOT NULL DEFAULT '0'
IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('AspNetUsers') AND name = 'LockoutEnabled')
ALTER TABLE AspNetUsers ADD LockoutEnabled bit NOT NULL DEFAULT '0'
IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('AspNetUsers') AND name = 'LockoutEndDateUtc')
ALTER TABLE AspNetUsers ADD LockoutEndDateUtc datetime NULL 
IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('AspNetUsers') AND name = 'AccessFailedCount')
ALTER TABLE AspNetUsers ADD AccessFailedCount int NOT NULL DEFAULT 0

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('AspNetUsers') AND name = 'NotifyForEmail')
ALTER TABLE AspNetUsers ADD NotifyForEmail bit NOT NULL DEFAULT '0'

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('AspNetUserClaims') AND name = 'UserId')
EXEC sp_RENAME 'AspNetUserClaims.User_Id', 'UserId', 'COLUMN'
if exists (select * from sysobjects where name='__MigrationHistory' and xtype='U')
begin
delete from __MigrationHistory where ContextKey != 'GeneratorBase.MVC.Migrations.Configuration'
end
--if exists (select * from sysobjects where name='tbl_UserBasedSecurity' and xtype='U')
--delete from tbl_UserBasedSecurity

GO

if not exists (select * from sysobjects where name='tbl_UserBasedSecurity' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_UserBasedSecurity](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[EntityName] [nvarchar](max) NOT NULL,
	[TargetEntityName] [nvarchar](max) NOT NULL,
	[AssociationName] [nvarchar](max) NOT NULL,
	[IsMainEntity] [bit] NOT NULL,
	[RolesToIgnore] [nvarchar](max) NULL,
	[Other1] [nvarchar](max) NULL,
	[Other2] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.tbl_UserBasedSecurity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO

if not exists (select * from sysobjects where name='tbl_FavoriteItem' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_FavoriteItem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[LinkAddress] [nvarchar](max) NOT NULL,
	[LastUpdatedBy] [datetime] NULL,
	[LastUpdatedByUser] [nvarchar](255) NOT NULL,
	[DisplayValue] [nvarchar](255) NOT NULL
) ON [PRIMARY]
end

GO

if not exists (select * from sysobjects where name='tbl_FacetedSearch' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_FacetedSearch](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Disable] [bit] NULL,
	[LinkAddress] [nvarchar](max) NULL,
	[EntityName] [nvarchar](255) NULL,
	[TargetEntity] [nvarchar](255) NULL,
	[AssociationName] [nvarchar](255) NULL,
	[OtherInfo] [nvarchar](255) NULL,
	[Flag] [bit] NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[Roles] [nvarchar](max) NULL,
	[TabName] [nvarchar](255) NULL,
) ON [PRIMARY]
end

GO

if not exists (select * from sysobjects where name='tbl_ImportConfiguration' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_ImportConfiguration](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[TableColumn] [nvarchar](255) NULL,
	[SheetColumn] [nvarchar](255) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUpdateUser] [nvarchar](255) NULL,
	[UniqueColumn] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[MappingName] [nvarchar](max) NULL,
	[IsDefaultMapping] [bit] NULL,
	[UpdateColumn] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_ImportConfiguration] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('tbl_ImportConfiguration') AND name = 'IsDefaultMapping')
ALTER TABLE tbl_ImportConfiguration ADD IsDefaultMapping bit NULL

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('tbl_ImportConfiguration') AND name = 'MappingName')
ALTER TABLE tbl_ImportConfiguration ADD MappingName NVARCHAR(MAX) NULL

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('tbl_ImportConfiguration') AND name = 'UpdateColumn')
ALTER TABLE tbl_ImportConfiguration ADD UpdateColumn NVARCHAR(MAX) NULL

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('tbl_FavoriteItem') AND name = 'EntityName')
ALTER TABLE tbl_FavoriteItem ADD EntityName NVARCHAR(255) NULL

GO

if not exists (select * from sysobjects where name='tbl_DefaultEntityPage' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_DefaultEntityPage](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[EntityName] [nvarchar](255) NULL,
	[Roles] [nvarchar](max) NULL,
	[PageType] [nvarchar](255) NULL,
	[PageUrl] [nvarchar](max) NULL,
	[Other] [nvarchar](255) NULL,
	[Flag] [bit] NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[ViewEntityPage] [nvarchar](max) NULL,
	[ListEntityPage] [nvarchar](max) NULL,
	[EditEntityPage] [nvarchar](max) NULL,
	[SearchEntityPage] [nvarchar](max) NULL,
	[CreateEntityPage] [nvarchar](max) NULL,

	[CreateQuickEntityPage] [nvarchar](max) NULL,
	[EditQuickEntityPage] [nvarchar](max) NULL,
	[CreateWizardEntityPage] [nvarchar](max) NULL,
	[EditWizardEntityPage]  [nvarchar](max) NULL,
	[HomePage] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_DefaultEntityPage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('tbl_DefaultEntityPage') AND name = 'CreateEntityPage')
ALTER TABLE tbl_DefaultEntityPage ADD CreateEntityPage [nvarchar](max) NULL

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('tbl_DefaultEntityPage') AND name = 'CreateQuickEntityPage')
ALTER TABLE tbl_DefaultEntityPage ADD CreateQuickEntityPage [nvarchar](max) NULL

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('tbl_DefaultEntityPage') AND name = 'EditQuickEntityPage')
ALTER TABLE tbl_DefaultEntityPage ADD EditQuickEntityPage [nvarchar](max) NULL

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('tbl_DefaultEntityPage') AND name = 'CreateWizardEntityPage')
ALTER TABLE tbl_DefaultEntityPage ADD CreateWizardEntityPage [nvarchar](max) NULL

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('tbl_DefaultEntityPage') AND name = 'EditWizardEntityPage')
ALTER TABLE tbl_DefaultEntityPage ADD EditWizardEntityPage [nvarchar](max) NULL

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('tbl_DefaultEntityPage') AND name = 'HomePage')
ALTER TABLE tbl_DefaultEntityPage ADD HomePage [nvarchar](max) NULL

GO

if not exists (select * from sysobjects where name='tbl_DynamicRoleMapping' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_DynamicRoleMapping](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[EntityName] [nvarchar](255) NULL,
	[RoleId] [nvarchar](255) NULL,
	[Condition] [nvarchar](max) NULL,
	[Value] [nvarchar](255) NULL,
	[UserRelation] [nvarchar](255) NULL,
	[Other] [nvarchar](255) NULL,
	[Flag] [bit] NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_DynamicRoleMapping] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end


if not exists (select * from sysobjects where name='tbl_FeedbackSeverity' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_FeedbackSeverity](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_FeedbackSeverity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO tbl_FeedbackSeverity (Name,Description,DisplayValue)
VALUES ('Blocker','','Blocker')
INSERT INTO tbl_FeedbackSeverity (Name,Description,DisplayValue)
VALUES ('Critical','','Critical')
INSERT INTO tbl_FeedbackSeverity (Name,Description,DisplayValue)
VALUES ('Major','','Major')
INSERT INTO tbl_FeedbackSeverity (Name,Description,DisplayValue)
VALUES ('Normal','','Normal')
INSERT INTO tbl_FeedbackSeverity (Name,Description,DisplayValue)
VALUES ('Minor','','Minor')
INSERT INTO tbl_FeedbackSeverity (Name,Description,DisplayValue)
VALUES ('Trivial','','Trivial')
end

GO

if not exists (select * from sysobjects where name='tbl_FeedbackResource' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_FeedbackResource](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[FirstName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL,
	[PhoneNo] [nvarchar](255) NULL,
	[ResourceId] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_FeedbackResource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end

GO

if not exists (select * from sysobjects where name='tbl_FeedbackPriority' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_FeedbackPriority](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_FeedbackPriority] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO tbl_FeedbackPriority (Name,Description,DisplayValue)
VALUES ('High','','High')
INSERT INTO tbl_FeedbackPriority (Name,Description,DisplayValue)
VALUES ('Normal','','Normal')
INSERT INTO tbl_FeedbackPriority (Name,Description,DisplayValue)
VALUES ('Low','','Low')
INSERT INTO tbl_FeedbackPriority (Name,Description,DisplayValue)
VALUES ('Very-Low','','Very-Low')

end

GO

if not exists (select * from sysobjects where name='tbl_ApplicationFeedbackType' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_ApplicationFeedbackType](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_ApplicationFeedbackType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO tbl_ApplicationFeedbackType (Name,Description,DisplayValue)
VALUES ('UI','','UI')
INSERT INTO tbl_ApplicationFeedbackType (Name,Description,DisplayValue)
VALUES ('Validation','','Validation')
INSERT INTO tbl_ApplicationFeedbackType (Name,Description,DisplayValue)
VALUES ('Funtional','','Funtional')
INSERT INTO tbl_ApplicationFeedbackType (Name,Description,DisplayValue)
VALUES ('Enhancement','','Enhancement')
INSERT INTO tbl_ApplicationFeedbackType (Name,Description,DisplayValue)
VALUES ('Defect','','Defect')
INSERT INTO tbl_ApplicationFeedbackType (Name,Description,DisplayValue)
VALUES ('Feature','','Feature')
INSERT INTO tbl_ApplicationFeedbackType (Name,Description,DisplayValue)
VALUES ('Suggestion','','Suggestion')

end

GO

if not exists (select * from sysobjects where name='tbl_ApplicationFeedbackStatus' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_ApplicationFeedbackStatus](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_ApplicationFeedbackStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
INSERT INTO tbl_ApplicationFeedbackStatus (Name,Description,DisplayValue)
VALUES ('New','','New')
INSERT INTO tbl_ApplicationFeedbackStatus (Name,Description,DisplayValue)
VALUES ('InProgress','','InProgress')
INSERT INTO tbl_ApplicationFeedbackStatus (Name,Description,DisplayValue)
VALUES ('Assigned','','Assigned')
INSERT INTO tbl_ApplicationFeedbackStatus (Name,Description,DisplayValue)
VALUES ('Resolved','','Resolved')
INSERT INTO tbl_ApplicationFeedbackStatus (Name,Description,DisplayValue)
VALUES ('Verified','','Verified')
INSERT INTO tbl_ApplicationFeedbackStatus (Name,Description,DisplayValue)
VALUES ('Duplicate','','Duplicate')
INSERT INTO tbl_ApplicationFeedbackStatus (Name,Description,DisplayValue)
VALUES ('Waiting for Info','','Waiting for Info')
end

GO

if not exists (select * from sysobjects where name='tbl_ApplicationFeedback' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_ApplicationFeedback](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[CommentId] [bigint] NULL,
	[Summary] [nvarchar](max) NULL,
	[EntityName] [nvarchar](255) NULL,
	[PropertyName] [nvarchar](255) NULL,
	[PageName] [nvarchar](255) NULL,
	[PageUrlTitle] [nvarchar](255) NULL,
	[PageUrl] [nvarchar](255) NULL,
	[UIControlName] [nvarchar](255) NULL,
	[AttachImage] [nvarchar](max) NULL,
	[AttachDocument] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[ReportedBy] [datetime] NULL,
	[ReportedByUser] [nvarchar](255) NULL,
	[EntitySubscribe] [bit] NULL,
	[AssociatedApplicationFeedbackTypeID] [bigint] NULL,
	[AssociatedApplicationFeedbackStatusID] [bigint] NULL,
	[ApplicationFeedbackPriorityID] [bigint] NULL,
	[ApplicationFeedbackSeverityID] [bigint] NULL,
	[ApplicationFeedbackResourceID] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_ApplicationFeedback] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[tbl_ApplicationFeedback]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_ApplicationFeedback_tbl_ApplicationFeedbackStatus_AssociatedApplicationFeedbackStatus] FOREIGN KEY([AssociatedApplicationFeedbackStatusID])
REFERENCES [dbo].[tbl_ApplicationFeedbackStatus] ([Id])


ALTER TABLE [dbo].[tbl_ApplicationFeedback] NOCHECK CONSTRAINT [FK_tbl_ApplicationFeedback_tbl_ApplicationFeedbackStatus_AssociatedApplicationFeedbackStatus]


ALTER TABLE [dbo].[tbl_ApplicationFeedback]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_ApplicationFeedback_tbl_ApplicationFeedbackType_AssociatedApplicationFeedbackType] FOREIGN KEY([AssociatedApplicationFeedbackTypeID])
REFERENCES [dbo].[tbl_ApplicationFeedbackType] ([Id])


ALTER TABLE [dbo].[tbl_ApplicationFeedback] NOCHECK CONSTRAINT [FK_tbl_ApplicationFeedback_tbl_ApplicationFeedbackType_AssociatedApplicationFeedbackType]


ALTER TABLE [dbo].[tbl_ApplicationFeedback]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_ApplicationFeedback_tbl_FeedbackPriority_ApplicationFeedbackPriority] FOREIGN KEY([ApplicationFeedbackPriorityID])
REFERENCES [dbo].[tbl_FeedbackPriority] ([Id])


ALTER TABLE [dbo].[tbl_ApplicationFeedback] NOCHECK CONSTRAINT [FK_tbl_ApplicationFeedback_tbl_FeedbackPriority_ApplicationFeedbackPriority]


ALTER TABLE [dbo].[tbl_ApplicationFeedback]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_ApplicationFeedback_tbl_FeedbackResource_ApplicationFeedbackResource] FOREIGN KEY([ApplicationFeedbackResourceID])
REFERENCES [dbo].[tbl_FeedbackResource] ([Id])


ALTER TABLE [dbo].[tbl_ApplicationFeedback] NOCHECK CONSTRAINT [FK_tbl_ApplicationFeedback_tbl_FeedbackResource_ApplicationFeedbackResource]


ALTER TABLE [dbo].[tbl_ApplicationFeedback]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_ApplicationFeedback_tbl_FeedbackSeverity_ApplicationFeedbackSeverity] FOREIGN KEY([ApplicationFeedbackSeverityID])
REFERENCES [dbo].[tbl_FeedbackSeverity] ([Id])

ALTER TABLE [dbo].[tbl_ApplicationFeedback] NOCHECK CONSTRAINT [FK_tbl_ApplicationFeedback_tbl_FeedbackSeverity_ApplicationFeedbackSeverity]

end


if not exists (select * from sysobjects where name='tbl_AppSettingGroup' and xtype='U')
begin

CREATE TABLE [dbo].[tbl_AppSettingGroup](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[LastUpdatedBy] [datetime] NOT NULL,
	[LastUpdatedByUser] [nvarchar](255) NOT NULL,
	[DisplayValue] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tbl_AppSettingGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
ALTER TABLE [dbo].[tbl_AppSettingGroup] ADD  DEFAULT ((0)) FOR [IsDefault]
end
GO

if(Select count(*) from tbl_AppSettingGroup Where [Name] = 'SSRS Report Setting') <= '0'
begin
INSERT [dbo].[tbl_AppSettingGroup] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'SSRS Report Setting', 1, CAST(getdate() AS DateTime), N'Admin', N'SSRS Report Setting')
end

if(Select count(*) from tbl_AppSettingGroup Where [Name] = 'Active Directory Setting') <= '0'
begin
INSERT [dbo].[tbl_AppSettingGroup] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'Active Directory Setting', 1, CAST(getdate() AS DateTime), N'Admin', N'Active Directory Setting')
end

if(Select count(*) from tbl_AppSettingGroup Where [Name] = 'Security Compliance Settings') <= '0'
begin
INSERT [dbo].[tbl_AppSettingGroup] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'Security Compliance Settings', 1, CAST(getdate() AS DateTime), N'Admin', N'Security Compliance Settings')
end

if(Select count(*) from tbl_AppSettingGroup Where [Name] = 'Google Analytics Settings') <= '0'
begin
DBCC CHECKIDENT ([tbl_AppSettingGroup], reseed, 3)
INSERT [dbo].[tbl_AppSettingGroup] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue])
VALUES (N'Google Analytics Settings', 1, CAST(getdate() AS DateTime), N'Admin', N'Google Analytics Settings')
end

if(Select count(*) from tbl_AppSettingGroup Where [Name] = 'Maintenance Mode') <= '0'
begin
INSERT [dbo].[tbl_AppSettingGroup] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue])
VALUES (N'Maintenance Mode', 1, CAST(getdate() AS DateTime), N'Admin', N'Maintenance Mode')
end

if(Select count(*) from tbl_AppSettingGroup Where [Name] = 'DateTime Settings') <= '0'
begin
INSERT [dbo].[tbl_AppSettingGroup] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue])
VALUES (N'DateTime Settings', 1, CAST(getdate() AS DateTime), N'Admin', N'DateTime Settings')
end

if(Select count(*) from tbl_AppSettingGroup Where [Name] = 'JSON Web Token Settings') <= '0'
begin
INSERT [dbo].[tbl_AppSettingGroup] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue])
VALUES (N'JSON Web Token Settings', 1, CAST(getdate() AS DateTime), N'Admin', N'JSON Web Token Settings')
end

if(Select count(*) from tbl_AppSettingGroup Where [Name] = 'Two-Factor Authentication Setting') <= '0'
begin
INSERT [dbo].[tbl_AppSettingGroup] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue])
VALUES (N'Two-Factor Authentication Setting', 1, CAST(getdate() AS DateTime), N'Admin', N'Two-Factor Authentication Setting')
end
--Create Group For Front Door Setting
if(Select count(*) from tbl_AppSettingGroup Where [Name] = 'Front Door Setting') <= '0'
begin
INSERT [dbo].[tbl_AppSettingGroup] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'Front Door Setting', 1, CAST(getdate() AS DateTime), N'Admin', N'Front Door Setting')
end
--
--Create Group For Google Map Setting
if(Select count(*) from tbl_AppSettingGroup Where [Name] = 'Google Map Setting') <= '0'
begin
INSERT [dbo].[tbl_AppSettingGroup] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'Google Map Setting', 1, CAST(getdate() AS DateTime), N'Admin', N'Google Map Setting')
end
--
--Create Group For Third Party
if(Select count(*) from tbl_AppSettingGroup Where [Name] = 'Third Party Login (Social Media)') <= '0'
begin
INSERT [dbo].[tbl_AppSettingGroup] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'Third Party Login (Social Media)', 1, CAST(getdate() AS DateTime), N'Admin', N'Third Party Login (Social Media)')
end
--

if not exists (select * from sysobjects where name='tbl_AppSetting' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_AppSetting](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Key] [nvarchar](255) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[AssociatedAppSettingGroupID] [bigint] NULL,
	[IsDefault] [bit] NOT NULL,
	[LastUpdatedBy] [datetime] NOT NULL,
	[LastUpdatedByUser] [nvarchar](255) NOT NULL,
	[DisplayValue] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tbl_AppSetting] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[tbl_AppSetting] ADD  DEFAULT ((0)) FOR [IsDefault]
end
GO

if(Select count(*) from tbl_AppSetting Where [Key] = 'ReportPath') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'ReportPath', N'http://10.0.1.252/ReportServer_SQLEXPRESS', N'SSRS Report Server Url', 1, 1, CAST(getdate() AS DateTime), N'Admin', N'ReportPath')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'ReportUser') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'ReportUser', N'Administrator', N'SSRS Report Server User Name', 1, 1, CAST(getdate() AS DateTime), N'Admin', N'ReportUser')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'ReportPass') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'ReportPass', N'xiubUjL4MusRejgbg/VpzA==', N'SSRS Report Server Password', 1, 1, CAST(getdate() AS DateTime), N'Admin', N'ReportPass')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'ReportFolder') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'ReportFolder', N'', N'SSRS Report Folder', 1, 1, CAST(getdate() AS DateTime), N'Admin', N'ReportFolder')
end 

if(Select count(*) from tbl_AppSetting Where [Key] = 'DomainName') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'DomainName', N'Your Domain Name', N'Domain Name Required if using AD Authentication', 2, 1, CAST(getdate() AS DateTime), N'Admin', N'DomainName')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'UseActiveDirectory') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'UseActiveDirectory', N'false', N'Set value as "true" incase of AD authentication', 2, 1, CAST(getdate() AS DateTime), N'Admin', N'UseActiveDirectory')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'UseActiveDirectoryRole') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'UseActiveDirectoryRole', N'false', N'Set value as "true" When you used AD authentication and you want to get users role from AD', 2, 1, CAST(getdate() AS DateTime), N'Admin', N'UseActiveDirectoryRole')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'AdministratorRoles') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'AdministratorRoles', N'Admin', N'Set admin role incase of AD authentication', 2, 1, CAST(getdate() AS DateTime), N'Admin', N'AdministratorRoles')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'NeedSharedUserSystem') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'NeedSharedUserSystem', N'', NULL, NULL, 1, CAST(getdate() AS DateTime), N'Admin', N'NeedSharedUserSystem')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'server') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'server', N'mvc.turanto.com', NULL, NULL, 1, CAST(getdate() AS DateTime), N'Admin', N'server')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'AppURL') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'AppURL', N'', NULL, NULL, 1, CAST(getdate() AS DateTime), N'Admin', N'AppURL')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'AppName') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'AppName', N'', NULL, NULL, 1, CAST(getdate() AS DateTime), N'Admin', N'AppName')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'MultipleRoleSelection') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'MultipleRoleSelection', N'true', N'For multiple role selection', NULL, 1, CAST(getdate() AS DateTime), N'Admin', N'MultipleRoleSelection')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'EnablePrototypingTool') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'EnablePrototypingTool', N'false', NULL, NULL, 1, CAST(getdate() AS DateTime), N'Admin', N'EnablePrototypingTool')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'UseActiveDirectoryRole') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'UseActiveDirectoryRole', N'false', N'Set value as "true" When you used AD authentication and you want to get users role from AD', 2, 1, CAST(getdate() AS DateTime), N'Admin', N'UseActiveDirectoryRole')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'GPSEnabled') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'GPSEnabled', N'true', NULL, NULL, 1, CAST(getdate() AS DateTime), N'Admin', N'GPSEnabled')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'FileTypes') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'FileTypes', N'xlsx,xls,doc,docx,txt,png,gif,jpg,jpeg,bmp,pptx,ppt,pdf', N'Define file types to be allowed in app', NULL, 1, CAST(getdate() AS DateTime), N'Admin', N'FileTypes')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'FileSize') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'FileSize', N'5', N'Define file size in MB to be allowed in app', NULL, 1, CAST(getdate() AS DateTime), N'Admin', N'FileSize')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'RegExValidationForFileName') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'RegExValidationForFileName', N'<No Regular Expression>', N'RegEx Validation For File Name', NULL, 1, CAST(getdate() AS DateTime), N'Admin', N'RegExValidationForFileName')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'ApplySecurityPolicy') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'ApplySecurityPolicy', N'No', N'Select Yes/No to apply security policy. Not applicable for session time out.', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'ApplySecurityPolicy')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'DefaultAccountLockoutTimeSpan') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'DefaultAccountLockoutTimeSpan', N'24', N'Set default lockout time span (In Hours)', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'DefaultAccountLockoutTimeSpan')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'MaxFailedAccessAttemptsBeforeLockout') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'MaxFailedAccessAttemptsBeforeLockout', N'5', N'Set maximum allowed failed attempt before account lockout', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'MaxFailedAccessAttemptsBeforeLockout')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'PasswordExpirationInDays') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'PasswordExpirationInDays', N'60', N'Enforce password change every (value) days.', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'PasswordExpirationInDays')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'ApplicationSessionTimeOut') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'ApplicationSessionTimeOut', N'20', N'Set time limit for active but idle session (in minutes). Set ''0'' to disable timeout.', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'ApplicationSessionTimeOut')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'ApplicationSessionTimeOutAlert') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'ApplicationSessionTimeOutAlert', N'false', N'Set true for showing alert message before 5 min when application session timeout.', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'ApplicationSessionTimeOutAlert')
end


if(Select count(*) from tbl_AppSetting Where [Key] = 'Glimpse') > '0'
begin
DELETE FROM [dbo].[tbl_AppSetting] WHERE [Key] = 'Glimpse'
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'CreateAnAccount') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'CreateAnAccount', N'false', N'Set true for active Create an account link on login page.', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'CreateAnAccount')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'OldPasswordGenerationCount') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'OldPasswordGenerationCount', N'3', N'User will not be able to change password from last (value) passwords.', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'OldPasswordGenerationCount')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'PasswordMinimumLength') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'PasswordMinimumLength', N'6', N'Minimum (value) character required to set new password.', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'PasswordMinimumLength')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'PasswordMaximumLength') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'PasswordMaximumLength', N'256', N'Maximum characters allowed to set password.', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'PasswordMaximumLength')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'PasswordRequireSpecialCharacter') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'PasswordRequireSpecialCharacter', N'Yes', N'Password requires at least one special character.', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'PasswordRequireSpecialCharacter')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'PasswordRequireDigit') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'PasswordRequireDigit', N'Yes', N'Password require at least one digit.', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'PasswordRequireDigit')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'PasswordRequireUpperCase') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'PasswordRequireUpperCase', N'Yes', N'Password require at least one upper case alphabet.', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'PasswordRequireUpperCase')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'PasswordRequireLowerCase') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'PasswordRequireLowerCase', N'Yes', N'Password require at least one lower case alphabet.', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'PasswordRequireLowerCase')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'EnforceChangePassword') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'EnforceChangePassword', N'Yes', N'Set value as Yes to enforce user to change password on first login else No.', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'EnforceChangePassword')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'PreventMultipleLogin') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'PreventMultipleLogin', N'No', N'Set value as Yes to prevent multiple login (to disconnect all active user''s session on login) else No.', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'PreventMultipleLogin')
end


if(Select count(*) from tbl_AppSetting Where [Key] = 'JournalEverything') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'JournalEverything', N'Yes', N'Set value as Yes to journal every action else only model trackable entities', NULL, 1, CAST(getdate() AS DateTime), N'Admin', N'JournalEverything')
end
--Recodrs for google analytics
if(Select count(*) from tbl_AppSetting Where [Key] = 'Enable google analytics') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'Enable google analytics', N'false', N'Enable google analytics for application.', 4, 1, CAST(getdate() AS DateTime), N'Admin', N'Enable google analytics')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'Tracking ID') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'Tracking ID', N'UA-XXXXXX-Y', N'If google analytics is enable then set Tracking ID generated form google analytics.', 4, 1, CAST(getdate() AS DateTime), N'Admin', N'Tracking ID')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'Custom Dimension Name') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'Custom Dimension Name', N'none', N'If google analytics is enable.In google analytics Account Admin>Property>User-ID then Custom Definitions>Custom Dimension Name active with User Scope that name set in this field.none for disable.', 4, 1, CAST(getdate() AS DateTime), N'Admin', N'Custom Dimension Name')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'MaintenanceMode') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'MaintenanceMode', N'false', N'Offline mode for application, only System-Admin can access the application.',5, 1, CAST(getdate() AS DateTime), N'Admin', N'MaintenanceMode')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'MaintenanceModeRoles') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'MaintenanceModeRoles', N'Admin', N'Allow roles to access in maintenance mode.',5, 1, CAST(getdate() AS DateTime), N'Admin', N'MaintenanceModeRoles')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'MaintenanceModeAlertMessage') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'MaintenanceModeAlertMessage', N'The application is undergoing maintenance. Please try again later.', N'Alert message to show during maintenance.',5, 1, CAST(getdate() AS DateTime), N'Admin', N'MaintenanceModeAlertMessage')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'SoftDeleteEnabled') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'SoftDeleteEnabled', N'false', N'Enabling Soft Delete will move deleted items to Recycle Bin, later archived items can be restored or purge.',NULL, 1, CAST(getdate() AS DateTime), N'Admin', N'Soft-Delete Enabled')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'PromptForRoleSelection') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'PromptForRoleSelection', N'true', N'Prompt for multiple role selection, if disabled then user will be logged-in with all assigned roles.',NULL, 1, CAST(getdate() AS DateTime), N'Admin', N'PromptForRoleSelection')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'DateFormat') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'DateFormat', N'Default', N'Global date format.',6, 1, CAST(getdate() AS DateTime), N'Admin', N'DateFormat')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'TimeFormat') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'TimeFormat', N'Default', N'Global time format.',6, 1, CAST(getdate() AS DateTime), N'Admin', N'TimeFormat')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'TimeZone') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'TimeZone', N'Default', N'Application global time zone.',6, 1, CAST(getdate() AS DateTime), N'Admin', N'TimeZone')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'ExternalValidationKey') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'ExternalValidationKey', N'', N'JWT token Validation Key.',7, 1, CAST(getdate() AS DateTime), N'Admin', N'External Validation Key')
end
if(Select count(*) from tbl_AppSetting Where [Key] = 'ExternalValidationKeySize') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'ExternalValidationKeySize', N'2048', N'JWT Validation Key Size.',7, 1, CAST(getdate() AS DateTime), N'Admin', N'External Validation Key Size')
end
if(Select count(*) from tbl_AppSetting Where [Key] = 'ExternalIssuerName') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'ExternalIssuerName', N'', N'JWT token Issuer Name.',7, 1, CAST(getdate() AS DateTime), N'Admin', N'Token Issuer Name')
end
if(Select count(*) from tbl_AppSetting Where [Key] = 'ExternalSecurityAlgorithm') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'ExternalSecurityAlgorithm', N'http://www.w3.org/2000/09/xmldsig#rsa-sha1', N'Security Algorithm to validate, .',7, 1, CAST(getdate() AS DateTime), N'Admin', N'Security Algorithm')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'QueryCacheTimeOut') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'QueryCacheTimeOut', N'60', N'Database query caching expiration(In Mins), Individual model entities can be enabled from Data Caching section in Admin section.',null, 1, CAST(getdate() AS DateTime), N'Admin', N'Query Cache Expiration')
end

Update[dbo].[tbl_AppSetting]
set [Value] = '60',  [Description] ='Database query caching expiration(In Mins), Individual model entities can be enabled from Data Caching section in Admin section.' 
Where [Key]='QueryCacheTimeOut' AND [Description]='Database query caching expiration(In hours), 0 to disable caching'


DELETE FROM [dbo].[tbl_AppSetting] WHERE [Key]='EnableQueryCache'

if(Select count(*) from tbl_AppSetting Where [Key] = 'AlwaysUseEmailAsUsername') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'AlwaysUseEmailAsUsername', N'No', N'Set value as Yes to always use email address as username.', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'Always use email as username')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'EnableNotification') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'EnableNotification', N'Yes', N'Set value as Yes to enable email notification', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'EnableNotification')
end

--Two factor change
if(Select count(*) from tbl_AppSetting Where [Key] = 'TwoFactorAuthenticationEnable') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'TwoFactorAuthenticationEnable', N'No', N'Set value as Yes to enable two factor authentication(Make sure to correct all user''s existing email ids before enabling this) ', 8, 1, CAST(getdate() AS DateTime), N'Admin', N'TwoFactorAuthenticationEnable')
end
if(Select count(*) from tbl_AppSetting Where [Key] = 'TwoFactorAuthenticationEnablePhoneCode') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'TwoFactorAuthenticationEnablePhoneCode', N'No', N'Set value as Yes to enable two factor authentication for Phone Code Authentication(Make sure to correct all user''s existing phone numbers before enabling this). Also, provide valid Twilio Account settings. ', 8, 1, CAST(getdate() AS DateTime), N'Admin', N'TwoFactorAuthenticationEnablePhoneCode')
end
if(Select count(*) from tbl_AppSetting Where [Key] = 'TwilioAccountSID') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'TwilioAccountSID', N'<Twilio Account SID>', N'Your Twilio Account SID', 8, 1, CAST(getdate() AS DateTime), N'Admin', N'TwilioAccountSID')
end
if(Select count(*) from tbl_AppSetting Where [Key] = 'TwilioAuthToken') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'TwilioAuthToken', N'<Twilio Auth  Token>', N'Your Twilio Auth token', 8, 1, CAST(getdate() AS DateTime), N'Admin', N'TwilioAuthToken')
end
if(Select count(*) from tbl_AppSetting Where [Key] = 'TwilioFromNumber') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'TwilioFromNumber', N'<Twilio From Number>', N'Your Twilio Phone Number', 8, 1, CAST(getdate() AS DateTime), N'Admin', N'TwilioFromNumber')
end
if(Select count(*) from tbl_AppSetting Where [Key] = 'TwilioDefaultCountryCode') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'TwilioDefaultCountryCode', N'+1', N'Twilio Default Country Code', 8, 1, CAST(getdate() AS DateTime), N'Admin', N'TwilioDefaultCountryCode')
end
--Front Door Setting parameter
if(Select count(*) from tbl_AppSetting Where [Key] = 'FrontDoorUrl') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'FrontDoorUrl', N'<Your Front Door Url>', N'Your Front Door Url', 9, 1, CAST(getdate() AS DateTime), N'Admin', N'FrontDoorUrl')
end
if(Select count(*) from tbl_AppSetting Where [Key] = 'FrontDoorEnable') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'FrontDoorEnable', N'No', N'Your Front Door Status ', 9, 1, CAST(getdate() AS DateTime), N'Admin', N'FrontDoorEnable')
end
if(Select count(*) from tbl_AppSetting Where [Key] = 'FrontDoorId') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'FrontDoorId', N'<Your Front Door Id>', N'Your Front Door Id', 9, 1, CAST(getdate() AS DateTime), N'Admin', N'FrontDoorId')
end
--
--Google Map Api Key Setting parameter
if(Select count(*) from tbl_AppSetting Where [Key] = 'GoogleMapAPIKey') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'GoogleMapAPIKey', N'QUl6YVN5QWhxdXV0YklfWk1sM1JFN0JVeWcwd3RxcG5YOUMwVEh3', N'A Google Maps API key is a personal code provided by Google to access Google Maps on this site. Your API key provides you with a free quota of Google Map queries.', 10, 1, CAST(getdate() AS DateTime), N'Admin', N'GoogleMapAPIKey')
end
if(Select count(*) from tbl_AppSetting Where [Key] = 'Unit') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'Unit', N'km', N'The unit of distance is a kilometer(km) or miles(mi) (kilometer(km) is the default).', 10, 1, CAST(getdate() AS DateTime), N'Admin', N'Unit')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'UseRevalee') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'UseRevalee', N'No', N'Enable to run scheduled business rules.',null, 1, CAST(getdate() AS DateTime), N'Admin', N'Use Revalee')
end
if(Select count(*) from tbl_AppSetting Where [Key] = 'ScheduledTaskCallbackTime') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'ScheduledTaskCallbackTime', N'30', N'Call scheduled tasks in every minutess.',null, 1, CAST(getdate() AS DateTime), N'Admin', N'Scheduled Task Callback Time')
end
--Third Party Login
if(Select count(*) from tbl_AppSetting Where [Key] = 'ThirdPartyLoginEnabled') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'ThirdPartyLoginEnabled', N'false', N'If true then we can Enabled Google, FaceBook Login That button will appear in the login page ', 11, 1, CAST(getdate() AS DateTime), N'Admin', N'ThirdPartyLoginEnabled')
end
if(Select count(*) from tbl_AppSetting Where [Key] = 'GoogleAuthenticationId') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'GoogleAuthenticationId', N'NONE', N'The GoogleAuthenticationId for setting Key with is register in any gmail account we can set/get from using https://console.cloud.google.com/apis/credentials.', 11, 1, CAST(getdate() AS DateTime), N'Admin', N'GoogleAuthenticationId')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'GoogleAuthenticationSecret') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'GoogleAuthenticationSecret', N'NONE', N'The GoogleAuthenticationSecret for setting Key with is register in any gmail account we can set/get from using https://console.cloud.google.com/apis/credentials.', 11, 1, CAST(getdate() AS DateTime), N'Admin', N'GoogleAuthenticationSecret')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'FacbookAuthenticationId') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'FacbookAuthenticationId', N'NONE', N'The FacbookAuthenticationId for setting App ID with is register in any Facebook  account we can set/get from using https://developers.facebook.com/ and click on log in for Facebook.', 11, 1, CAST(getdate() AS DateTime), N'Admin', N'FacbookAuthenticationId')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'FacbookAuthenticationSecret') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'FacbookAuthenticationSecret', N'NONE', N'The FacbookAuthenticationId for setting App Secret with is register in any Facebook  account we can set/get from using https://developers.facebook.com/ and click on log in for Facebook.', 11, 1, CAST(getdate() AS DateTime), N'Admin', N'FacbookAuthenticationSecret')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'MicrosoftAuthenticationId') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'MicrosoftAuthenticationId', N'NONE', N'The MicrosoftAuthenticationId for setting App ID with is register in any Microsoft account we can set/get from using https://portal.azure.com/ .', 11, 1, CAST(getdate() AS DateTime), N'Admin', N'MicrosoftAuthenticationId')
end

if(Select count(*) from tbl_AppSetting Where [Key] = 'MicrosoftAuthenticationSecret') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'MicrosoftAuthenticationSecret', N'NONE', N'The MicrosoftAuthenticationId for setting App Secret with is register in any Microsoft account we can set/get from using https://portal.azure.com/ .', 11, 1, CAST(getdate() AS DateTime), N'Admin', N'MicrosoftAuthenticationSecret')
end

--
GO

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('tbl_Permission') AND name = 'IsOwner')
ALTER TABLE tbl_Permission ADD IsOwner bit NULL

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('tbl_Permission') AND name = 'UserAssociation')
ALTER TABLE tbl_Permission ADD UserAssociation [nvarchar](max) NULL

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('tbl_Permission') AND name = 'Verbs')
ALTER TABLE tbl_Permission ADD Verbs [nvarchar](max) NULL

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('tbl_Permission') AND name = 'SelfRegistration')
ALTER TABLE tbl_Permission ADD SelfRegistration bit NULL
GO

if not exists (select * from sysobjects where name='tbl_EmailTemplateType' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_EmailTemplateType](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[LastUpdatedBy] [datetime] NOT NULL,
	[LastUpdatedByUser] [nvarchar](255) NOT NULL,
	[DisplayValue] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_EmailTemplateType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
ALTER TABLE [dbo].[tbl_EmailTemplateType] ADD  DEFAULT ((0)) FOR [IsDefault]	
end 
GO

if(Select count(*) from tbl_EmailTemplateType Where [Name] = 'User Registration') <= '0'
begin
INSERT [dbo].[tbl_EmailTemplateType] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) VALUES (N'User Registration', 1, CAST(0x0000A51700EA2AAF AS DateTime), N'Admin', N'User Registration')
end

if(Select count(*) from tbl_EmailTemplateType Where [Name] = 'User Forgot Password') <= '0'
begin
INSERT [dbo].[tbl_EmailTemplateType] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) VALUES (N'User Forgot Password', 1, CAST(0x0000A51701289744 AS DateTime), N'Admin', N'User Forgot Password')
end

if(Select count(*) from tbl_EmailTemplateType Where [Name] = 'Workflow') <= '0'
begin
INSERT [dbo].[tbl_EmailTemplateType] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) VALUES (N'Workflow', 1, CAST(0x0000A51B00CCCEFF AS DateTime), N'Admin', N'Workflow')
end

if(Select count(*) from tbl_EmailTemplateType Where [Name] = 'Business Rule') <= '0'
begin
INSERT [dbo].[tbl_EmailTemplateType] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) VALUES (N'Business Rule', 1, CAST(0x0000A5170147FDEB AS DateTime), N'Admin', N'Business Rule')
end

if(Select count(*) from tbl_EmailTemplateType Where [Name] = 'Notification to Create Password for New Account') <= '0'
begin
INSERT [dbo].[tbl_EmailTemplateType] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) VALUES (N'Notification to Create Password for New Account', 1, CAST(0x0000A51701289456 AS DateTime), N'Admin', N'Notification to Create Password for New Account')
end

if(Select count(*) from tbl_EmailTemplateType Where [Name] = 'Verify Email') <= '0'
begin
INSERT [dbo].[tbl_EmailTemplateType] ([Name], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) VALUES (N'Verify Email', 1, CAST(0x0000AB3E00CD459B AS DateTime), N'Admin', N'Verify Email')
end

if not exists (select * from sysobjects where name='tbl_EmailTemplate' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_EmailTemplate](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[AssociatedEmailTemplateTypeID] [bigint] NULL,
	[EmailContent] [nvarchar](max) NULL,
	[LastUpdatedBy] [datetime] NOT NULL,
	[LastUpdatedByUser] [nvarchar](255) NOT NULL,
	[DisplayValue] [nvarchar](255) NOT NULL,
	
 CONSTRAINT [PK_tbl_EmailTemplate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
INSERT [dbo].[tbl_EmailTemplate] ([AssociatedEmailTemplateTypeID], [EmailContent], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) VALUES (1, N'<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title>Turanto - Rapid Application Generator</title>
<table width="600px" cellspacing="0" cellpadding="0" border="0" style="border: 2px solid #fbac1c;
    font-family: arial,helvetica,sans-serif; font-size: 12px;">
    <tbody>
        <tr>
            <td>
                <table width="100%" cellspacing="0" cellpadding="0" border="0">
                    <tbody>
                        <tr>
                            <td><br><table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-left: 8px;
                                    font-family: arial,helvetica,sans-serif; font-size: 12px;">
                                    <tbody>
                                        <tr>
                                            <td>Dear <span style="font-weight: bold;">###FullName###</span>,<br>You are registered with ###AppName###. Click ###URL### to go to your application.</td>
                                        </tr>
                                    </tbody>
                                </table>
                                <p style="margin-left: 8px; font-family: arial,helvetica,sans-serif; font-size: 12px;">
                                    <span style="text-decoration: none;"><a href="[TurantoSite]"></a></span>
                                </p>
                                <table width="100%" cellspacing="4" border="0" bgcolor="#e9e9e9" style="border-top: 1px solid #fbac1c;
                                    margin-top: 10px; font-family: arial,helvetica,sans-serif; font-size: 12px;">
                                    <tbody>
                                        <tr>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </tbody>
</table>
<table width="600px" cellspacing="0" cellpadding="2" border="0" style="font-family: arial,helvetica,sans-serif;font-size: 10px;">
    <tbody>
        <tr>
            <td>
                Disclaimer : This email communication may contain privileged and confidential information
                and is intended for the use of the addressee only.If you are not an intended recipient
                you are requested not to reproduce,copy disseminate or in any manner distribute
                this email communication as the same is strictly prohibited. If you have received
                this email in error, Please notify the sender immediately by return e-mail and delete
                the communication sent in error.Email communications cannot be guaranteed to be
                secure & error free and TURANTO is not liable for any errors in the email communication
                or for the proper, timely and complete transmission thereof.
            </td>
        </tr>
    </tbody>
</table>
', CAST(0x0000A51C00E9199A AS DateTime), N'Admin', N'User Registration')
INSERT [dbo].[tbl_EmailTemplate] ([AssociatedEmailTemplateTypeID], [EmailContent], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) VALUES (2, N'<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
	<title>Turanto - Rapid Application Generator</title>
	<table width="600px" cellspacing="0" cellpadding="0" border="0" style="border: 2px solid #fbac1c;font-family: arial,helvetica,sans-serif; font-size: 12px;">
		<tbody>
			<tr>
				<td>
					<table width="100%" cellspacing="0" cellpadding="0" border="0">
						<tbody>
							<tr>
								<td>
								  <br>
									<table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-left: 8px;font-family: arial,helvetica,sans-serif; font-size: 12px;">
										<tbody>
											<tr>
												<td>
														 Dear<span style="font-weight: bold;"> ###FullName###</span>,<br>
														 <br>Please use <span style="font-weight: bold;">###CODE###</span> to reset password.<br>
														 UserName :&nbsp;<span style="font-weight: bold;">###Username###.</span><br>
														 Application :&nbsp;<span style="font-weight: bold;">###AppName###</span><br>
														 Click <span style="font-weight: bold;">###URL###</span> to reset password.</span><br><br>
														 <span style="font-weight: bold;">Note: </span>Above activation/reset password link can be used once.
												</td>
											</tr>
										</tbody>
									</table>
									<p style="margin-left: 8px; font-family: arial,helvetica,sans-serif; font-size: 12px;">
										<span style="text-decoration: none;"><a href="[TurantoSite]"></a></span>
									</p>
									<table width="100%" cellspacing="4" border="0" bgcolor="#e9e9e9" style="border-top: 1px solid #fbac1c;margin-top: 10px; font-family: arial,helvetica,sans-serif; font-size: 12px;">
										<tbody>
											<tr></tr>
										</tbody>
									</table>
								</td>
							</tr>
						</tbody>
					</table>
				</td>
			</tr>
		</tbody>
	</table>
	<table width="600px" cellspacing="0" cellpadding="2" border="0" style="font-family: arial,helvetica,sans-serif;font-size: 10px;">
		<tbody>
			<tr>
				<td>
					Disclaimer : This email communication may contain privileged and confidential information
					and is intended for the use of the addressee only.If you are not an intended recipient
					you are requested not to reproduce,copy disseminate or in any manner distribute
					this email communication as the same is strictly prohibited. If you have received
					this email in error, Please notify the sender immediately by return e-mail and delete
					the communication sent in error.Email communications cannot be guaranteed to be
					secure & error free and TURANTO is not liable for any errors in the email communication
					or for the proper, timely and complete transmission thereof.
				</td>
			</tr>
		</tbody>
	</table>', CAST(0x0000A51C00E8203B AS DateTime), N'Admin', N'User Forgot Password')




INSERT [dbo].[tbl_EmailTemplate] ([AssociatedEmailTemplateTypeID], [EmailContent], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) VALUES (3, N'<meta http-equiv="Content-Type" content="text/html; charset=utf-8"><title>Turanto - Rapid Application Generator</title><table width="600px" cellspacing="0" cellpadding="0" border="0" style="border: 2px solid #fbac1c;font-family: arial,helvetica,sans-serif; font-size: 12px;"><tbody><tr><td><table width="100%" cellspacing="0" cellpadding="0" border="0"><tbody><tr><td> <br><table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-left: 8px;font-family: arial,helvetica,sans-serif; font-size: 12px;"><tbody><tr><td><p> Dear<span style="font-weight: bold;"> ###FullName###</span>, <br><br>Please use following code to reset password for&nbsp;<span style="font-weight: bold;">###Username###</span><br> <span style="font-weight: bolder;">###CODE###&nbsp;</span></p><p><span style="font-weight: bolder;"><br></span><br>###AppName###<br><b>Note: </b>Above code can be used once, and code will expire in 24 hours.<br><br></p></td></tr></tbody></table><p style="margin-left: 8px; font-family: arial,helvetica,sans-serif; font-size: 12px;"> <span style="text-decoration: none;"><a href="[TurantoSite]"></a></span></p><table width="100%" cellspacing="4" border="0" bgcolor="#e9e9e9" style="border-top: 1px solid #fbac1c;margin-top: 10px; font-family: arial,helvetica,sans-serif; font-size: 12px;"><tbody><tr></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table><table width="600px" cellspacing="0" cellpadding="2" border="0" style="font-family: arial,helvetica,sans-serif;font-size: 10px;"><tbody><tr><td> Disclaimer : This email communication may contain privileged and confidential information and is intended for the use of the addressee only.If you are not an intended recipient you are requested not to reproduce,copy disseminate or in any manner distribute this email communication as the same is strictly prohibited. If you have received this email in error, Please notify the sender immediately by return e-mail and delete the communication sent in error.Email communications cannot be guaranteed to be secure &amp; error free and TURANTO is not liable for any errors in the email communication or for the proper, timely and complete transmission thereof.</td></tr></tbody></table>
', CAST(0x0000A51C00DFD032 AS DateTime), N'Admin', N'Workflow')
INSERT [dbo].[tbl_EmailTemplate] ([AssociatedEmailTemplateTypeID], [EmailContent], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) VALUES (4, N'<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title>Turanto - Rapid Application Generator</title>
<table width="600px" cellspacing="0" cellpadding="0" border="0" style="border: 2px solid #fbac1c;
    font-family: arial,helvetica,sans-serif; font-size: 12px;">
    <tbody>
        <tr>
            <td>
                <table width="100%" cellspacing="0" cellpadding="0" border="0">
                    <tbody>
                        <tr>
                            <td>
                                <br>
                              
                                <table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-left: 8px;
                                    font-family: arial,helvetica,sans-serif; font-size: 12px;">
                                    <tbody>
                                        <tr>
                                            <td>###Message###</td>
                                        </tr>
                                    </tbody>
                                </table>
                                <p style="margin-left: 8px; font-family: arial,helvetica,sans-serif; font-size: 12px;">
                                    <span style="text-decoration: none;"><a href="[TurantoSite]"></a></span>
                                </p>
                                <table width="100%" cellspacing="4" border="0" bgcolor="#e9e9e9" style="border-top: 1px solid #fbac1c;
                                    margin-top: 10px; font-family: arial,helvetica,sans-serif; font-size: 12px;">
                                    <tbody>
                                        <tr>
                                            
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </tbody>
</table>
<table width="600px" cellspacing="0" cellpadding="2" border="0" style="font-family: arial,helvetica,sans-serif;
    font-size: 10px;">
    <tbody>
        <tr>
            <td>
                Disclaimer : This email communication may contain privileged and confidential information
                and is intended for the use of the addressee only.If you are not an intended recipient
                you are requested not to reproduce,copy disseminate or in any manner distribute
                this email communication as the same is strictly prohibited. If you have received
                this email in error, Please notify the sender immediately by return e-mail and delete
                the communication sent in error.Email communications cannot be guaranteed to be
                secure & error free and TURANTO is not liable for any errors in the email communication
                or for the proper, timely and complete transmission thereof.
            </td>
        </tr>
    </tbody>
</table>
', CAST(0x0000A51B010A2792 AS DateTime), N'Admin', N'Business Rule')


end 

GO

if (Select count(*) from tbl_EmailTemplateType Where [Name] = 'Notification to Create Password for New Account') = '1' and (Select count(*) from tbl_EmailTemplate Where [DisplayValue] = 'Notification to Create Password for New Account') <= '0'
begin
INSERT [dbo].[tbl_EmailTemplate] ([AssociatedEmailTemplateTypeID], [EmailContent], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) VALUES ((Select Id from tbl_EmailTemplateType Where [Name] = 'Notification to Create Password for New Account'), N'<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title>Turanto - Rapid Application Generator</title>
<table width="600px" cellspacing="0" cellpadding="0" border="0" style="border: 2px solid #fbac1c;
    font-family: arial,helvetica,sans-serif; font-size: 12px;">
    <tbody>
        <tr>
            <td>
                <table width="100%" cellspacing="0" cellpadding="0" border="0">
                    <tbody>
                        <tr>
                            <td>
                                <br>
                              
                                <table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-left: 8px;
                                    font-family: arial,helvetica,sans-serif; font-size: 12px;">
                                    <tbody>
                                        <tr>
                                            <td>
												Dear <span style="font-weight: bold;">###FullName###</span>,
												<br><br>This is an automatically generated message from the application ###AppName###.
												<br><br>A new user account has been created for you. Your username is ###UserName###.
												<br><br>Click ###URL### to create your password.
												<br><br>After your password has been created, you can login to your new account.<br>
											</td>
                                        </tr>
                                    </tbody>
                                </table>
                                <p style="margin-left: 8px; font-family: arial,helvetica,sans-serif; font-size: 12px;">
                                    <span style="text-decoration: none;"><a href="[TurantoSite]"></a></span>
                                </p>
                                <table width="100%" cellspacing="4" border="0" bgcolor="#e9e9e9" style="border-top: 1px solid #fbac1c;
                                    margin-top: 10px; font-family: arial,helvetica,sans-serif; font-size: 12px;">
                                    <tbody>
                                        <tr>
                                            
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </tbody>
</table>
<table width="600px" cellspacing="0" cellpadding="2" border="0" style="font-family: arial,helvetica,sans-serif;
    font-size: 10px;">
    <tbody>
        <tr>
            <td>
                Disclaimer : This email communication may contain privileged and confidential information
                and is intended for the use of the addressee only.If you are not an intended recipient
                you are requested not to reproduce,copy disseminate or in any manner distribute
                this email communication as the same is strictly prohibited. If you have received
                this email in error, Please notify the sender immediately by return e-mail and delete
                the communication sent in error.Email communications cannot be guaranteed to be
                secure & error free and TURANTO is not liable for any errors in the email communication
                or for the proper, timely and complete transmission thereof.
            </td>
        </tr>
    </tbody>
</table>
', CAST(0x0000A51B010A2792 AS DateTime), N'Admin', N'Notification to Create Password for New Account')
end

if (Select count(*) from tbl_EmailTemplateType Where [Name] = 'Verify Email') = '1' and (Select count(*) from tbl_EmailTemplate Where [DisplayValue] = 'Verify Email') <= '0'
begin
INSERT [dbo].[tbl_EmailTemplate] ([AssociatedEmailTemplateTypeID], [EmailContent], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) VALUES ((Select Id from tbl_EmailTemplateType Where [Name] = 'Verify Email'), N'<meta http-equiv="Content-Type" content="text/html; charset=utf-8">  <title>Turanto - Rapid Application Generator</title>  <table width="600px" cellspacing="0" cellpadding="0" border="0" style="border: 2px solid #fbac1c;      font-family: arial,helvetica,sans-serif; font-size: 12px;">      <tbody>          <tr>              <td>                  <table width="100%" cellspacing="0" cellpadding="0" border="0">                      <tbody>                          <tr>                              <td>                                  <br>                                                                  <table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-left: 8px;                                      font-family: arial,helvetica,sans-serif; font-size: 12px;">                                      <tbody>                                          <tr>                                              <td>              Dear <span style="font-weight: bold;">###FullName###</span>,              <p></p><p>You are registered with ###AppName###.</p><p><br>Please use following code&nbsp;for email verification.<br></p><p>&nbsp;<span style="font-weight: bolder;">###CODE###</span><br></p><p> </p>             </td>                                          </tr>                                      </tbody>                                  </table>                                  <p style="margin-left: 8px; font-family: arial,helvetica,sans-serif; font-size: 12px;">                                      <span style="text-decoration: none;"><a href="[TurantoSite]"></a></span>                                  </p>                                  <table width="100%" cellspacing="4" border="0" bgcolor="#e9e9e9" style="border-top: 1px solid #fbac1c;                                      margin-top: 10px; font-family: arial,helvetica,sans-serif; font-size: 12px;">                                      <tbody>                                          <tr>                                                                                        </tr>                                      </tbody>                                  </table>                              </td>                          </tr>                      </tbody>                  </table>              </td>          </tr>      </tbody>  </table>  <table width="600px" cellspacing="0" cellpadding="2" border="0" style="font-family: arial,helvetica,sans-serif;      font-size: 10px;">      <tbody>          <tr>              <td>                  Disclaimer : This email communication may contain privileged and confidential information                  and is intended for the use of the addressee only.If you are not an intended recipient                  you are requested not to reproduce,copy disseminate or in any manner distribute                  this email communication as the same is strictly prohibited. If you have received                  this email in error, Please notify the sender immediately by return e-mail and delete                  the communication sent in error.Email communications cannot be guaranteed to be                  secure &amp; error free and TURANTO is not liable for any errors in the email communication                  or for the proper, timely and complete transmission thereof.              </td>          </tr>      </tbody>  </table>  ', CAST(0x0000A51B010A2792 AS DateTime), N'Admin', N'Verify Email')
end


IF NOT EXISTS ( SELECT  * FROM    syscolumns  WHERE   id = OBJECT_ID('tbl_EmailTemplate') AND name = 'EmailSubject' ) 
ALTER TABLE tbl_EmailTemplate ADD EmailSubject [nvarchar](max) NULL
GO
begin
UPDATE tbl_EmailTemplate SET EmailContent=N'<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
	<title>Turanto - Rapid Application Generator</title>
	<table width="600px" cellspacing="0" cellpadding="0" border="0" style="border: 2px solid #fbac1c;font-family: arial,helvetica,sans-serif; font-size: 12px;">
		<tbody>
			<tr>
				<td>
					<table width="100%" cellspacing="0" cellpadding="0" border="0">
						<tbody>
							<tr>
								<td>
								  <br>
									<table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-left: 8px;font-family: arial,helvetica,sans-serif; font-size: 12px;">
										<tbody>
											<tr>
												<td>
													Dear<span style="font-weight: bold;"> ###FullName###</span>,<br>
														 <br>Please use <span style="font-weight: bold;">###CODE###</span> to reset password.<br>
														 UserName :&nbsp;<span style="font-weight: bold;">###Username###.</span><br>
														 Application :&nbsp;<span style="font-weight: bold;">###AppName###</span><br>
														 Click <span style="font-weight: bold;">###URL###</span> to reset password.</span><br><br>
														 <span style="font-weight: bold;">Note: </span>Above activation/reset password link can be used once.
												</td>
											</tr>
										</tbody>
									</table>
									<p style="margin-left: 8px; font-family: arial,helvetica,sans-serif; font-size: 12px;">
										<span style="text-decoration: none;"><a href="[TurantoSite]"></a></span>
									</p>
									<table width="100%" cellspacing="4" border="0" bgcolor="#e9e9e9" style="border-top: 1px solid #fbac1c;margin-top: 10px; font-family: arial,helvetica,sans-serif; font-size: 12px;">
										<tbody>
											<tr></tr>
										</tbody>
									</table>
								</td>
							</tr>
						</tbody>
					</table>
				</td>
			</tr>
		</tbody>
	</table>
	<table width="600px" cellspacing="0" cellpadding="2" border="0" style="font-family: arial,helvetica,sans-serif;font-size: 10px;">
		<tbody>
			<tr>
				<td>
					Disclaimer : This email communication may contain privileged and confidential information
					and is intended for the use of the addressee only.If you are not an intended recipient
					you are requested not to reproduce,copy disseminate or in any manner distribute
					this email communication as the same is strictly prohibited. If you have received
					this email in error, Please notify the sender immediately by return e-mail and delete
					the communication sent in error.Email communications cannot be guaranteed to be
					secure & error free and TURANTO is not liable for any errors in the email communication
					or for the proper, timely and complete transmission thereof.
				</td>
			</tr>
		</tbody>
	</table>'
WHERE AssociatedEmailTemplateTypeID=2 and EmailContent NOT LIKE '%###CODE###%'
end
GO
if not exists (select * from sysobjects where name='LoginAttempts' and xtype='U')
begin
CREATE TABLE [dbo].[LoginAttempts](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[IsSuccessfull] [bit] NOT NULL,
	[Date] [datetime] NOT NULL,
	[IPAddress] [nvarchar](128) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end
GO

if not exists (select * from sysobjects where name='PasswordHistory' and xtype='U')
begin
CREATE TABLE [dbo].[PasswordHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[HashedPassword] [nvarchar](max) NOT NULL,
	[Date] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end

if not exists (select * from sysobjects where name='tbl_Messages' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_Messages](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[MobileNo] [nvarchar](255) NULL,
	[Message] [nvarchar](max) NULL,
	[Picture] [bigint] NULL,
	[Timestamp] [datetime] NULL,
	[IsRead] [bit] NULL,
	[MessageUserID] [nvarchar](255) NULL,
	[GroupMessagesID] [nvarchar](255) NULL,
	[MessagesSenderID] [nvarchar](255) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_Messages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end
GO

if not exists (select * from sysobjects where name='tbl_MessagesDelivered' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_MessagesDelivered](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[IsRead] [bit] NULL,
	[MessageDeliveredToPersonID] [nvarchar](max) NULL,
	[DeliveredMessageID] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_MessagesDelivered] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end

GO

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_RuleAction')
                    AND name = 'IsElseAction' )                   
ALTER TABLE tbl_RuleAction ADD IsElseAction bit NOT NULL  DEFAULT '0'

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_BusinessRule')
                    AND name = 'Disable' ) 
ALTER TABLE tbl_BusinessRule ADD Disable bit NOT NULL DEFAULT '0'
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_BusinessRule')
                    AND name = 'Freeze' )                   
ALTER TABLE tbl_BusinessRule ADD Freeze bit NOT NULL  DEFAULT '0'

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_BusinessRule')
                    AND name = 'Description' ) 
ALTER TABLE tbl_BusinessRule ADD Description [nvarchar](max) NULL

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_BusinessRule')
                    AND name = 'InformationMessage' ) 
ALTER TABLE tbl_BusinessRule ADD InformationMessage [nvarchar](max) NULL

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_BusinessRule')
                    AND name = 'FailureMessage' ) 
ALTER TABLE tbl_BusinessRule ADD FailureMessage [nvarchar](max) NULL

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_UserDefinePages')
                    AND name = 'IsPageDisabled' ) 
ALTER TABLE tbl_UserDefinePages ADD IsPageDisabled bit NULL DEFAULT '0'

if(Select count(*) from tbl_BusinessRuleType Where [TypeName] = 'OnUpdate') <= '0'
begin
INSERT INTO tbl_BusinessRuleType (TypeNo,TypeName,Description,DisplayValue)
VALUES (1,'OnAdd',NULL,'On Add')
INSERT INTO tbl_BusinessRuleType (TypeNo,TypeName,Description,DisplayValue)
VALUES (2,'OnUpdate',NULL,'On Update')
INSERT INTO tbl_BusinessRuleType (TypeNo,TypeName,Description,DisplayValue)
VALUES (3,'OnAddUpdate',NULL,'On Add & Update')
end
if(Select count(*) from tbl_BusinessRuleType Where [TypeName] = 'OnPropertyChange') <= '0'
begin
INSERT INTO tbl_BusinessRuleType (TypeNo,TypeName,Description,DisplayValue)
VALUES (4,'OnPropertyChange',NULL,'On Property Change')
end
if(Select count(*) from tbl_BusinessRuleType Where [TypeName] = 'Scheduled') <= '0'
begin
INSERT INTO tbl_BusinessRuleType (TypeNo,TypeName,Description,DisplayValue)
VALUES (5,'Scheduled',NULL,'Scheduled')
end
if(Select count(*) from tbl_BusinessRuleType Where [TypeName] = 'BeforeSave') <= '0'
begin
INSERT INTO tbl_BusinessRuleType (TypeNo,TypeName,Description,DisplayValue)
VALUES (6,'BeforeSave',NULL,'Before Save')
end
--Add Record For OnLoadingCreate
if(Select count(*) from tbl_BusinessRuleType Where [TypeName] = 'OnLoadingCreate') <= '0'
begin
INSERT INTO tbl_BusinessRuleType (TypeNo,TypeName,Description,DisplayValue)
VALUES (7,'OnLoadingCreate',NULL,'OnLoading Create')
end
--Add Record For OnLoadingUpdate
if(Select count(*) from tbl_BusinessRuleType Where [TypeName] = 'OnLoadingUpdate') <= '0'
begin
INSERT INTO tbl_BusinessRuleType (TypeNo,TypeName,Description,DisplayValue)
VALUES (8,'OnLoadingUpdate',NULL,'OnLoading Update')
end
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_DefaultEntityPage')
                    AND name = 'ViewEntityPage' ) 
begin
ALTER TABLE tbl_DefaultEntityPage ADD [ViewEntityPage] [nvarchar](max) NULL
ALTER TABLE tbl_DefaultEntityPage ADD [ListEntityPage] [nvarchar](max) NULL
ALTER TABLE tbl_DefaultEntityPage ADD [EditEntityPage] [nvarchar](max) NULL
ALTER TABLE tbl_DefaultEntityPage ADD [SearchEntityPage] [nvarchar](max) NULL
ALTER TABLE tbl_DefaultEntityPage ADD [CreateEntityPage] [nvarchar](max) NULL
ALTER TABLE tbl_DefaultEntityPage ADD [WizardEntityPage] [nvarchar](max) NULL
ALTER TABLE tbl_DefaultEntityPage ADD [HomePage] [nvarchar](max) NULL
end

if not exists (select * from sysobjects where name='tbl_EntityDataSource' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_EntityDataSource](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[EntityName] [nvarchar](255) NULL,
	[DataSource] [nvarchar](max) NULL,
	[SourceType] [nvarchar](255) NULL,
	[MethodType] [nvarchar](255) NULL,
	[Action] [nvarchar](255) NULL,
	[flag] [bit] NULL,
	[other] [nvarchar](255) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_EntityDataSource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[tbl_DataSourceParameters](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[ArgumentName] [nvarchar](255) NULL,
	[HostingEntity] [nvarchar](255) NULL,
	[flag] [bit] NULL,
	[other] [nvarchar](255) NULL,
	[EntityDataSourceParametersID] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[ArgumentValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_DataSourceParameters] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[tbl_DataSourceParameters]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_DataSourceParameters_tbl_EntityDataSource_EntityDataSourceParameters] FOREIGN KEY([EntityDataSourceParametersID])
REFERENCES [dbo].[tbl_EntityDataSource] ([Id])

ALTER TABLE [dbo].[tbl_DataSourceParameters] NOCHECK CONSTRAINT [FK_tbl_DataSourceParameters_tbl_EntityDataSource_EntityDataSourceParameters]

if exists (select * from sysobjects where name='tbl_DataSourceParameters' and xtype='U')
begin
	ALTER TABLE [tbl_DataSourceParameters] 
	ALTER COLUMN [ArgumentValue] [nvarchar](max) NULL
END


CREATE TABLE [dbo].[tbl_PropertyMapping](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[PropertyName] [nvarchar](255) NULL,
	[DataName] [nvarchar](255) NULL,
	[DataSource] [nvarchar](max) NULL,
	[SourceType] [nvarchar](255) NULL,
	[MethodType] [nvarchar](255) NULL,
	[Action] [nvarchar](255) NULL,
	[EntityPropertyMappingID] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_PropertyMapping] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[tbl_PropertyMapping]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_PropertyMapping_tbl_EntityDataSource_EntityPropertyMapping] FOREIGN KEY([EntityPropertyMappingID])
REFERENCES [dbo].[tbl_EntityDataSource] ([Id])
ALTER TABLE [dbo].[tbl_PropertyMapping] NOCHECK CONSTRAINT [FK_tbl_PropertyMapping_tbl_EntityDataSource_EntityPropertyMapping]

end

GO

if not exists (select * from sysobjects where name='tbl_ScheduledTaskHistory' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_ScheduledTaskHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TaskName] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [nvarchar](max) NULL,
	[GUID] [nvarchar](max) NULL,
	[CallbackUri] [nvarchar](max) NULL,
	[BusinessRuleId] [bigint] NULL,
	[Other] [nvarchar](max) NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[RunDateTime] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.tbl_ScheduledTaskHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end

GO

if not exists (select * from sysobjects where name='tbl_RecurrenceDays' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_RecurrenceDays](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_RecurrenceDays] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
INSERT INTO [dbo].[tbl_RecurrenceDays] (Name,Description,DisplayValue)
VALUES ('Sun','Sunday','Sun'), ('Mon','Monday','Mon'),('Tue','Tuesday','Tuesday'),('Wed','Wednesday','Wed'),('Thu','Thursday','Thu'),('Fri','Friday','Fri'),('Sat','Saturday','Sat')
end
GO

if not exists (select * from sysobjects where name='tbl_RecurringEndType' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_RecurringEndType](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_RecurringEndType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO [dbo].[tbl_RecurringEndType] (Name,Description,DisplayValue)
VALUES ('Never','Never','Never'), ('After Specified Occurrences','After Specified Occurrences','After Specified Occurrences'),('On Specified Date','On Specified Date','On Specified Date')
end
GO

if not exists (select * from sysobjects where name='tbl_RecurringFrequency' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_RecurringFrequency](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [int] NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_RecurringFrequency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO [dbo].[tbl_RecurringFrequency] (Name,Description,DisplayValue)
VALUES (1,'1','1'),(2,'2','2'),(3,'3','3'),(4,'4','4'),(5,'5','5'),(6,'6','6'),(7,'7','7'),(8,'8','8'),(9,'9','9'),(10,'10','10'),(11,'11','11'),(12,'12','12'),(13,'13','13'),(14,'14','14'),(15,'15','15'),(16,'16','16'),(17,'17','17'),(18,'18','18'),(19,'19','19'),(20,'20','20'),(21,'21','21'),(22,'22','22'),(23,'23','23'),(24,'24','24'),(25,'25','25'),(26,'26','26'),(27,'27','27'),(28,'28','28'),(29,'29','29'),(30,'30','30'),(31,'31','31')
end
GO

if not exists (select * from sysobjects where name='tbl_RecurringScheduleDetails' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_RecurringScheduleDetails](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[EndDate] [datetime] NULL,
	[Summary] [nvarchar](max) NULL,
	[OccurrenceCount] [int] NULL,
	[AssociatedRecurringScheduleDetailsTypeID] [bigint] NULL,
	[RecurringRepeatFrequencyID] [bigint] NULL,
	[RecurringTaskEndTypeID] [bigint] NULL,
	[RepeatByID] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_RecurringScheduleDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

end
GO


if not exists (select * from sysobjects where name='tbl_RecurringScheduleDetailstype' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_RecurringScheduleDetailstype](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_RecurringScheduleDetailstype] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
INSERT INTO [dbo].[tbl_RecurringScheduleDetailstype] (Name,Description,DisplayValue)
VALUES ('Daily','Daily','Daily'),('Weekly','Weekly','Weekly'),('Monthly','Monthly','Monthly'),('Yearly','Yearly','Yearly')
end
GO

if not exists (select * from sysobjects where name='tbl_RepeatOn' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_RepeatOn](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[ScheduleID] [bigint] NULL,
	[RecurrenceDaysID] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_RepeatOn] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end
GO

if not exists (select * from sysobjects where name='tbl_Schedule' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_Schedule](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[StartDateTime] [datetime] NULL,
	[AssociatedScheduleTypeID] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[EndDate] [datetime] NULL,
	[OccurrenceLimitCount] [int] NULL,
	[Summary] [nvarchar](max) NULL,
	[AssociatedRecurringScheduleDetailsTypeID] [bigint] NULL,
	[RecurringRepeatFrequencyID] [bigint] NULL,
	[RecurringTaskEndTypeID] [bigint] NULL,
	[RepeatByID] [bigint] NULL,
 CONSTRAINT [PK_tbl_Schedule] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end
GO

if not exists (select * from sysobjects where name='tbl_Scheduletype' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_Scheduletype](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_Scheduletype] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
INSERT INTO [dbo].[tbl_Scheduletype] (Name,Description,DisplayValue)
VALUES ('OneTime','OneTime','OneTime'),('Repeat','Repeat','Repeat')
end
GO

if not exists (select * from sysobjects where name='tbl_MonthlyRepeatType' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_MonthlyRepeatType](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_MonthlyRepeatType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
INSERT INTO [dbo].[tbl_MonthlyRepeatType] (Name,Description,DisplayValue)
VALUES ('day of the month','day of the month','day of the month'),('last weekend day of the month','last weekend day of the month','last weekend day of the month'),('last day of the month','last day of the month','last day of the month'),('first day of the month','first day of the month','first day of the month')
end

GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_BusinessRule')
                    AND name = 'SchedulerTaskID' ) 
ALTER TABLE tbl_BusinessRule ADD SchedulerTaskID bigint NULL

GO
if not exists (select * from sysobjects where name='tbl_MultiTenantLoginSelected' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_MultiTenantLoginSelected](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[AccessNo] [bigint] NULL,
	[AccessDateTime] [datetime] NOT NULL,
	[User] [nvarchar](255) NOT NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[MainEntity] [bigint] NULL,
	
 CONSTRAINT [PK_tbl_MultiTenantLoginSelected] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end

GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_MultiTenantLoginSelected')
                    AND name = 'MainEntityValue' ) 
ALTER TABLE tbl_MultiTenantLoginSelected ADD MainEntityValue [nvarchar](max) NULL


GO
if not exists (select * from sysobjects where name='tbl_MultiTenantExtraAccess' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_MultiTenantExtraAccess](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[User] [nvarchar](255) NOT NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[MainEntityID] [bigint] NULL,
	
 CONSTRAINT [PK_tbl_MultiTenantExtraAccess] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end

GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_MultiTenantExtraAccess')
                    AND name = 'MainEntityValue' ) 
ALTER TABLE tbl_MultiTenantExtraAccess ADD MainEntityValue [nvarchar](max) NULL


GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_Schedule')
                    AND name = 'StartTime' ) 
ALTER TABLE tbl_Schedule ADD StartTime datetime NULL

GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_Schedule')
                    AND name = 'EndTime' ) 
ALTER TABLE tbl_Schedule ADD EndTime datetime NULL

GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_Schedule')
                    AND name = 'EntityName' ) 
ALTER TABLE tbl_Schedule ADD EntityName [nvarchar](max) NULL

GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_Schedule')
                    AND name = 'IsDeleted' ) 
ALTER TABLE tbl_Schedule ADD IsDeleted [bit] NULL

GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_Schedule')
                    AND name = 'DeleteDateTime' ) 
ALTER TABLE tbl_Schedule ADD DeleteDateTime [datetime] NULL
GO

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_Schedule')
                    AND name = 'ExportDataLogId' ) 
ALTER TABLE tbl_Schedule ADD ExportDataLogId [bigint] NULL
GO

if not exists (select * from sysobjects where name='tbl_ApiToken' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_ApiToken](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[AuthToken] [nvarchar](255) NOT NULL,
	[IssuedOn] [datetime] NOT NULL,
	[ExpiresOn] [datetime] NOT NULL,
	[UsersID] [nvarchar](255) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_ApiToken] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end

if not exists (select * from sysobjects where name='tbl_CustomReports' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_CustomReports](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[ReportName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ReportType] [nvarchar](255) NOT NULL,
	[EntityName] [nvarchar](255) NULL,
	[ResultProperty] [nvarchar](max) NULL,
	[ColumnOrder] [int] NULL,
	[OrderBy] [nvarchar](255) NULL,
	[GroupBy] [bit] NULL,
	[CrossTabRow] [nvarchar](255) NULL,
	[CrossTabColumn] [nvarchar](255) NULL,
	[FilterProperty] [nvarchar](max) NULL,
	[FilterCondition] [nvarchar](max) NULL,
	[FilterType] [nvarchar](255) NULL,
	[SelectValueFromList] [nvarchar](255) NULL,
	[SelectProperty] [nvarchar](255) NULL,
	[RelatedEntity] [nvarchar](255) NULL,
	[ForeignKeyEntity] [nvarchar](255) NULL,
	[RelationName] [nvarchar](255) NULL,
	[AggregateEntity] [nvarchar](255) NULL,
	[AggregateProperty] [nvarchar](255) NULL,
	[AggregateFunction] [nvarchar](255) NULL,
	[FilterValue] [nvarchar](255) NULL,
	[EntityValues] [nvarchar](255) NULL,
	[CrossTabPropertyValues] [nvarchar](max) NULL,
	[QueryConditionValues] [nvarchar](max) NULL,
	[RelationsValues] [nvarchar](max) NULL,
	[OtherValues] [nvarchar](255) NULL,
	[CreatedByUserID] [nvarchar](255) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_CustomReports] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end

if not exists (select * from sysobjects where name='tbl_PermissionAdminPrivilege' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_PermissionAdminPrivilege](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AdminFeature] [nvarchar](max) NULL,
	[RoleName] [nvarchar](max) NULL,
	[IsAllow] [bit] NOT NULL,
	[IsAdd] [bit] NULL,
	[IsEdit] [bit] NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_dbo.tbl_PermissionAdminPrivilege] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end

if not exists (select * from sysobjects where name='tbl_ReportsInRole' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_ReportsInRole](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[ReportId] [varchar](150) NULL,
	[RoleId] [varchar](150) NULL,
	[EntityName] [varchar](150) NULL,
	[Flag] [bit] NULL DEFAULT (0),
	[Argument] [varchar](150) NULL,
	[DisplayValue] [nvarchar](max) NULL) ON [PRIMARY]
end

GO

IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('AspNetRoles') AND name = 'Discriminator')
ALTER TABLE AspNetRoles ADD [Discriminator] [varchar](150) NULL 
IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('AspNetRoles') AND name = 'Description')
ALTER TABLE AspNetRoles ADD [Description] [nvarchar](max) NULL 
IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('AspNetRoles') AND name = 'RoleType')
ALTER TABLE AspNetRoles ADD [RoleType] [nvarchar](150) NULL 
IF NOT EXISTS ( SELECT  * FROM syscolumns WHERE   id = OBJECT_ID('AspNetRoles') AND name = 'TenantId')
ALTER TABLE AspNetRoles ADD [TenantId] [bigint] NULL

GO

Update [dbo].[AspNetRoles] set [Discriminator] = 'ApplicationRole'  
Update [dbo].[AspNetRoles] set [RoleType] = 'Global' Where [RoleType] is NULL

GO

IF EXISTS (SELECT  * FROM syscolumns WHERE id = OBJECT_ID('tbl_ChangeData') AND name = 'JsonData')
alter table tbl_ChangeData alter column JsonData [nvarchar](max) null


--Entity Property Page
if not exists (select * from sysobjects where name='tbl_EntityPage' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_EntityPage](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[EntityName] [nvarchar](255) NOT NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_EntityPage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
end
GO
---Added Column For Disable EntityHelp
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_EntityPage')
                    AND name = 'Disable' ) 
ALTER TABLE tbl_EntityPage ADD Disable bit NOT NULL DEFAULT '0'

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_EntityPage')
                    AND name = 'EnableDataCache' ) 
ALTER TABLE tbl_EntityPage ADD EnableDataCache bit NOT NULL DEFAULT '0'
----
if not exists (select * from sysobjects where name='tbl_EntityHelpPage' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_EntityHelpPage](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[SectionName] [nvarchar](255) NOT NULL,
	[Order] [int] NOT NULL,
	[Disable] [bit] NULL,
	[SectionText] [nvarchar](max) NULL,
	[EntityOfEntityHelpID] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[EntityName] [nvarchar](255) NULL,
 CONSTRAINT [PK_tbl_EntityHelpPage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[tbl_EntityHelpPage]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_EntityHelpPage_tbl_EntityPage_EntityOfEntityHelp] FOREIGN KEY([EntityOfEntityHelpID])
REFERENCES [dbo].[tbl_EntityPage] ([Id])
ALTER TABLE [dbo].[tbl_EntityHelpPage] CHECK CONSTRAINT [FK_tbl_EntityHelpPage_tbl_EntityPage_EntityOfEntityHelp]
end
GO
if not exists (select * from sysobjects where name='tbl_PropertyHelpPage' and xtype='U')
begin
CREATE TABLE [dbo].[tbl_PropertyHelpPage](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[PropertyDataType] [nvarchar](255) NULL,
	[HelpText] [nvarchar](max) NULL,
	[PropertyName] [nvarchar](255) NOT NULL,
	[PropertyHelpOfEntityID] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[EntityName] [nvarchar](255) NULL,
 CONSTRAINT [PK_tbl_PropertyHelpPage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
ALTER TABLE [dbo].[tbl_PropertyHelpPage]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_PropertyHelpPage_tbl_EntityPage_PropertyHelpOfEntity] FOREIGN KEY([PropertyHelpOfEntityID])
REFERENCES [dbo].[tbl_EntityPage] ([Id])

ALTER TABLE [dbo].[tbl_PropertyHelpPage] NOCHECK CONSTRAINT [FK_tbl_PropertyHelpPage_tbl_EntityPage_PropertyHelpOfEntity]
end

GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_PropertyHelpPage')
                    AND name = 'Tooltip' ) 
ALTER TABLE tbl_PropertyHelpPage ADD Tooltip [nvarchar](max) NULL

GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_PropertyHelpPage')
                    AND name = 'ObjectType' ) 
ALTER TABLE tbl_PropertyHelpPage ADD ObjectType [nvarchar](255) NULL

GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_PropertyHelpPage')
                    AND name = 'GroupId' ) 
ALTER TABLE tbl_PropertyHelpPage ADD GroupId int NULL

GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_PropertyHelpPage')
                    AND name = 'GroupName' ) 
ALTER TABLE tbl_PropertyHelpPage ADD GroupName [nvarchar](255) NULL

GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_PropertyHelpPage')
                    AND name = 'Disable' ) 
ALTER TABLE tbl_PropertyHelpPage ADD Disable bit NULL

GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_EntityDataSource')
                    AND name = 'RootNode' ) 
ALTER TABLE tbl_EntityDataSource ADD RootNode [nvarchar](MAX) NULL

-- End Entity Property Page

--Reporting tables
if not exists (select * from sysobjects where name='tbl_ReportList' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_ReportList](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[IsDeleted] [bit] NULL,
	[DeleteDateTime] [datetime] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[ReportsGroupSSRSReportAssociationID] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[ReportID] [nvarchar](255) NULL,
	[ReportPath] [nvarchar](max) NULL,
	[ReportNo] [int] NULL,
	[DisplayName] [nvarchar](255) NOT NULL,
	[IsHidden] [bit] NULL,
	[Description] [nvarchar](max) NULL,
	[EntityName] [nvarchar](255) NULL,
	[Type] [nvarchar](255) NULL,
 CONSTRAINT [PK_dbo.tbl_ReportList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ReportList')
                    AND name = 'ExportDataLogId' ) 
ALTER TABLE tbl_ReportList ADD ExportDataLogId [bigint] NULL
GO


if not exists (select * from sysobjects where name='tbl_ReportsGroup' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_ReportsGroup](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[IsDeleted] [bit] NULL,
	[DeleteDateTime] [datetime] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[DisplayOrder] [int] NULL,
 CONSTRAINT [PK_dbo.tbl_ReportsGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ReportsGroup')
                    AND name = 'ExportDataLogId' ) 
ALTER TABLE tbl_ReportsGroup ADD ExportDataLogId [bigint] NULL
GO

--End reporting tables
if not exists (select * from sysobjects where name='tbl_PropertyValidationandFormat' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_PropertyValidationandFormat](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[EntityName] [nvarchar](255) NOT NULL,
	[PropertyName] [nvarchar](255) NOT NULL,
	[RegExPattern] [nvarchar](max) NULL,
	[ErrorMessage] [nvarchar](255) NULL,
	[IsEnabled] [bit] NULL,
	[Other1] [nvarchar](255) NULL,
	[DisplayFormat] [nvarchar](255) NULL,
	[Other2] [nvarchar](255) NULL,
	[LowerBound] [nvarchar](255) NULL,
	[UpperBound] [nvarchar](255) NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[MaskPattern] [nvarchar](255) NULL,
 CONSTRAINT [PK_tbl_PropertyValidationandFormat] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END

if not exists (select * from sysobjects where name='tbl_LoginSelectedRoles' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_LoginSelectedRoles](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[T_User] [nvarchar](max) NULL,
	[T_Roles] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_tbl_LoginSelectedRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END

if not exists (select * from sysobjects where name='tbl_DataMetric' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_DataMetric](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[ToolTip] [nvarchar](max) NULL,
	[Aggregate] [nvarchar](255) NULL,
	[EntityName] [nvarchar](255) NULL,	
	[Hide] [bit] NULL,
	[BackGroundColor] [nvarchar](255) NULL,
	[FontColor] [nvarchar](255) NULL,
	[AggregatePropertyName] [nvarchar](255) NULL,
	[EntitySubscribe] [bit] NULL,
	[AssociatedDataMetricTypeID] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[ClassIcon] [nvarchar](255) NULL,
	[Roles] [nvarchar](max) NULL,
	[AssociatedFacetedSearchID] [bigint] NULL,
	[DisplayOrder] [int] NULL,
	[DisplayOn] [nvarchar](255) NULL,
	[QueryUrl] [nvarchar](max) NULL,
	[GraphType] [nvarchar](255) NULL,
 CONSTRAINT [PK_tbl_tbl_DataMetric] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_DataMetric')
                    AND name = 'BackgroundImage' ) 
ALTER TABLE tbl_DataMetric ADD BackgroundImage [bigint] NULL
if not exists (select * from sysobjects where name='tbl_DataMetrictype' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_DataMetrictype](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_DataMetrictype] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


INSERT INTO tbl_DataMetrictype (Name,Description,DisplayValue)
VALUES ('Number','Number','Number')
INSERT INTO tbl_DataMetrictype (Name,Description,DisplayValue)
VALUES ('List','List','List')
INSERT INTO tbl_DataMetrictype (Name,Description,DisplayValue)
VALUES ('Graph','Graph','Graph')
END
GO
--added a value for background image.
if(Select count(*) from [tbl_DataMetrictype] Where [Name] = 'HyperLink') <= '0'
begin
INSERT INTO tbl_DataMetrictype (Name,Description,DisplayValue)
VALUES ('HyperLink','HyperLink','HyperLink')
end
--
if not exists (select * from sysobjects where name='tbl_DocumentTemplate' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_DocumentTemplate](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[AutoNo] [bigint] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[RecordAddedInsertDate] [datetime] NULL,
	[RecordAddedInsertBy] [nvarchar](255) NULL,
	[RecordAdded] [datetime] NULL,
	[RecordAddedUser] [nvarchar](255) NULL,
	[Document] [bigint] NULL,
	[DocumentType] [nvarchar](255) NULL,
	[ActionType] [nvarchar](255) NULL,
	[EntityName] [nvarchar](255) NOT NULL,
	[DefaultOutputFormat] [nvarchar](255) NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[ToolTip] [nvarchar](255) NULL,
	[DisplayType] [nvarchar](255) NULL,
	[AllowedRoles] [nvarchar](max) NULL,
	[AttachDocumentTo] [nvarchar](255) NULL,
	[DisplayOrder] [int] NULL,
	[BackGroundColor] [nvarchar](255) NULL,
	[FontColor] [nvarchar](255) NULL,
	[EntitySubscribe] [bit] NULL,
	[Disable] [bit] NULL,
	[EnableDownload] [bit] NULL,
	[AttachmentName] [nvarchar](max) NULL,
	[Tenants] [nvarchar](max) NULL
 CONSTRAINT [PK_tbl_DocumentTemplate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END

IF NOT EXISTS ( SELECT  * FROM  syscolumns  WHERE   id = OBJECT_ID('tbl_DocumentTemplate') AND name = 'Tenants' ) 
ALTER TABLE tbl_DocumentTemplate ADD Tenants [nvarchar](max) NULL

IF NOT EXISTS ( SELECT  * FROM  syscolumns  WHERE   id = OBJECT_ID('tbl_DocumentTemplate') AND name = 'EnablePreview' ) 
ALTER TABLE tbl_DocumentTemplate ADD EnablePreview [bit] NULL
GO


---Hierarchical Security

IF NOT EXISTS ( SELECT  * FROM  syscolumns  WHERE   id = OBJECT_ID('tbl_Condition') AND name = 'DynamicRuleConditionsID' ) 
ALTER TABLE tbl_Condition ADD DynamicRuleConditionsID bigint NULL
GO

IF NOT EXISTS ( SELECT  * FROM  syscolumns  WHERE   id = OBJECT_ID('tbl_Permission') AND name = 'EditR' ) 
ALTER TABLE tbl_Permission ADD EditR [nvarchar](max) NULL
GO
IF NOT EXISTS ( SELECT  * FROM  syscolumns  WHERE   id = OBJECT_ID('tbl_Permission') AND name = 'ViewR' ) 
ALTER TABLE tbl_Permission ADD ViewR [nvarchar](max) NULL
GO
IF NOT EXISTS ( SELECT  * FROM  syscolumns  WHERE   id = OBJECT_ID('tbl_Permission') AND name = 'DeleteR' ) 
ALTER TABLE tbl_Permission ADD DeleteR [nvarchar](max) NULL
GO



----End Hierarchical Security

--For CompanyInformation
if not exists (select * from sysobjects where name='tbl_CompanyInformation' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_CompanyInformation](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[CompanyName] [nvarchar](255) NOT NULL,
	[CompanyEmail] [nvarchar](255) NOT NULL,
	[CompanyAddress] [nvarchar](max) NULL,
	[CompanyCountry] [nvarchar](255) NULL,
	[CompanyState] [nvarchar](255) NULL,
	[CompanyCity] [nvarchar](255) NULL,
	[CompanyZipCode] [nvarchar](255) NULL,
	[ContactNumber1] [nvarchar](255) NULL,
	[ContactNumber2] [nvarchar](255) NULL,
	[LoginBg] [bigint] NULL,
	[Logo] [bigint] NULL,
	[Icon] [bigint] NULL,
	[LogoWidth] [nvarchar](255) NULL,
	[LogoHeight] [nvarchar](255) NULL,
	[IconWidth] [nvarchar](255) NULL,
	[IconHeight] [nvarchar](255) NULL,
	[LoginBackgroundWidth] [nvarchar](255) NULL,
	[LoginBackgroundHeight] [nvarchar](255) NULL,
	[SMTPUser] [nvarchar](255) NOT NULL,
	[SMTPPassword] [nvarchar](255) NOT NULL,
	[SMTPPort] [nvarchar](255) NOT NULL,
	[SSL] [bit] NULL,
	[SMTPServer] [nvarchar](255) NOT NULL,
	[UseAnonymous] [bit] NULL,
	[AboutCompany] [nvarchar](max) NULL,
	[Disclaimer] [nvarchar](max) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_CompanyInformation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO tbl_CompanyInformation
           ([CompanyName]
           ,[CompanyEmail]
           ,[CompanyAddress]
           ,[CompanyCountry]
           ,[CompanyState]
           ,[CompanyCity]
           ,[CompanyZipCode]
           ,[ContactNumber1]
           ,[ContactNumber2]
           ,[LoginBg]
           ,[Logo]
           ,[Icon]
           ,[LogoWidth]
           ,[LogoHeight]
           ,[IconWidth]
           ,[IconHeight]
           ,[LoginBackgroundWidth]
           ,[LoginBackgroundHeight]
           ,[SMTPUser]
           ,[SMTPPassword]
           ,[SMTPPort]
           ,[SSL]
           ,[SMTPServer]
           ,[UseAnonymous]
           ,[AboutCompany]
           ,[Disclaimer]
           ,[DisplayValue])
     VALUES
           ('Turanto','do-not-reply@turanto.com','5388 Twin Hickory Rd','USA','VA','Glen Allen','23059'
           ,'1.866.591.5906','1.866.591.5906',NULL,NULL,NULL,'155px','29px','28px','28px','','',
           'apikey','U0cuLWVGLW9YNDJUQjJrVjVyQ3V5RXdody41LXRKNnhLNU9OMDJjQkxpelhDUERXYW50Q2RIdnRiR2kwSFR4cWI1cWlv','587',1,'smtp.sendgrid.net',0
           ,'<h2 class="tour-content-title tour-title-0  " helvetica="" neue",="" helvetica,="" arial,="" sans-serif;="" color="" rgb(81,="" 81,="" 81);="" margin-top="" 0px;="" margin-right="" margin-bottom="" 15px;="" font-size="" 28px;="" -webkit-font-smoothing="" antialiased;="" letter-spacing="" normal;="" background-color="" rgb(251,="" 252,="" 253);"="">
    <p style="margin: 0cm 0cm 7.5pt; line-height: 18pt;">
        <span style="font-weight: bolder;">
            <span lang="EN-US" arial",sans-serif;color:#5c5c5c"="" style="font-size: 10pt;">
                Turanto<strong>– A Complete Solution</strong>
            </span>
        </span>
        <span lang="EN-US" arial",sans-serif;color:#5c5c5c"="" style="font-size: 10pt;">
            <o:p></o:p>
        </span>
    </p><p style="margin: 0cm 0cm 7.5pt; line-height: 18pt;">
        <span lang="EN-US" arial",sans-serif;="" color:#5c5c5c"="" style="font-size: 10pt;">
            With Turanto you can build data-driven applications (both web and mobile) on cloud that can be deployed on to private datacenter or cloud servers.
            The deployment can be linked to 3rd party login solutions (say Facebook or Google),
            or can be leveraged with Active Directory or Windows Domain login within an enterprise.
            <o:p></o:p>
        </span>
    </p><p class="MsoNormal" style="margin-bottom: 7.5pt; line-height: 18pt;">
        <span lang="EN-US" arial",sans-serif;mso-fareast-font-family="" "times=" " new=" " roman" ;color:#5c5c5c"="" style="font-size: 10pt;">
            Turanto application is based on following standard parameters, thus, an organization’s standards are truly followed, even during tight schedule.
            <o:p></o:p>
        </span>
    </p><ul type="disc">
        <li class="MsoNormal" style="color: rgb(92, 92, 92); line-height: normal;">
            <span lang="EN-US" arial",sans-serif;="" mso-fareast-font-family:"times="" new="" roman""="" style="font-size: 10pt;">
                User Interface<o:p></o:p>
            </span>
        </li>
        <li class="MsoNormal" style="color: rgb(92, 92, 92); line-height: normal;">
            <span lang="EN-US" arial",sans-serif;="" mso-fareast-font-family:"times="" new="" roman""="" style="font-size: 10pt;">
                Coding practices<o:p></o:p>
            </span>
        </li>
        <li class="MsoNormal" style="color: rgb(92, 92, 92); line-height: normal;">
            <span lang="EN-US" arial",sans-serif;="" mso-fareast-font-family:"times="" new="" roman""="" style="font-size: 10pt;">
                Documentation Templates<o:p></o:p>
            </span>
        </li>
        <li class="MsoNormal" style="color: rgb(92, 92, 92); line-height: normal;">
            <span lang="EN-US" arial",sans-serif;="" mso-fareast-font-family:"times="" new="" roman""="" style="font-size: 10pt;">
                Deployment Processes<o:p></o:p>
            </span>
        </li>
        <li class="MsoNormal" style="color: rgb(92, 92, 92); line-height: normal;">
            <span lang="EN-US" arial",sans-serif;="" mso-fareast-font-family:"times="" new="" roman""="" style="font-size: 10pt;">
                Security compliance<o:p></o:p>
            </span>
        </li>
    </ul><p class="MsoNormal" style="margin-bottom: 7.5pt; line-height: 18pt;">
        <span lang="EN-US" arial",sans-serif;mso-fareast-font-family="" "times=" " new=" " roman" ;color:#5c5c5c"="" style="font-size: 10pt;">
            Turanto can import your existing database (excel, access, MS-SQL, oracle) and create an application for it.
            Importing the database is advanced feature of Turanto and users can access this using a simple&nbsp;wizard.
            Once the application is created, it can be deployed as web application, mobile application and android application (APK).<o:p></o:p>
        </span>
    </p><p class="MsoNormal" style="margin-bottom: 7.5pt; line-height: 18pt;">
        <span lang="EN-US" arial",sans-serif;mso-fareast-font-family="" "times=" " new=" " roman" ;color:#5c5c5c"="" style="font-size: 10pt;">
            The applications can be launched and are ready to use at any point of time.
            This enables stakeholders to see a working application sooner, so they can provide more complete feedback.
            The patterns included are generic software engineering patterns.
            The significant capabilities of Turanto takes a concept to all the way a complete software system.
            A large organization can customize these patterns to include internal integration patterns, security practices,
            and built-in objects (such as “Patient” for HealthCare, or “Inventory” for a manufacturing outfit),
            so that the prototypes built already include the custom integration. Turanto already has such integration capability and thus,
            the custom application is more specific to the end user environment.<o:p></o:p>
        </span>
    </p>
    <p class="MsoNormal" style="margin-bottom: 7.5pt; line-height: 18pt;">
        <span style="font-weight: bolder;">
            <span lang="EN-US" arial",sans-serif;mso-fareast-font-family="" "times=" " new=" " roman" ;color:#5c5c5c"="" style="font-size: 10pt;">
                Creating Applications in&nbsp;
            </span>
        </span><span style="font-weight: bolder;">
            <span lang="EN-US" arial",sans-serif;mso-fareast-font-family:"times="" new="" roman";="" color:#5c5c5c"="" style="font-size: 10pt;">
                Turanto
            </span>
        </span><span lang="EN-US" arial",sans-serif;mso-fareast-font-family:"times="" new="" roman";="" color:#5c5c5c"="" style="font-size: 10pt;">
            <o:p></o:p>
        </span>
    </p><p class="MsoNormal" style="margin-bottom: 7.5pt; line-height: 18pt;">
        <span lang="EN-US" arial",sans-serif;mso-fareast-font-family="" "times=" " new=" " roman" ;color:#5c5c5c"="" style="font-size: 10pt;">
            Turanto, a browser based tool, enables domain model specification with prediction around visual interface, database design, and integration needs.<o:p></o:p>
        </span>
    </p>
    <p class="MsoNormal" style="margin-bottom: 7.5pt; line-height: 18pt;">
        <span lang="EN-US" arial",sans-serif;mso-fareast-font-family="" "times=" " new=" " roman" ;color:#5c5c5c"="" style="font-size: 10pt;">
            Once the basics of the problem are available in a model, Turanto then overlays the common enterprise requirements on that,
            to create a complete working application.<o:p></o:p>
        </span>
    </p><p class="MsoNormal" style="margin-bottom: 7.5pt; line-height: 18pt;">
        <span lang="EN-US" arial",sans-serif;mso-fareast-font-family="" "times=" " new=" " roman" ;color:#5c5c5c"="" style="font-size: 10pt;">
            The applications created by Turanto are typically data driven and need custom database built. The database tables, views,
            and stored procedure follow standard naming conventions. The database is very specific to an application, and is completely de-normalized.
            By default, it is a SQL Server 2012 database, but can be easily tuned to an alternate version or vendor.<o:p></o:p>
        </span>
    </p><p class="MsoNormal" style="margin-bottom: 7.5pt; line-height: 18pt;">
        <span lang="EN-US" arial",sans-serif;mso-fareast-font-family="" "times=" " new=" " roman" ;color:#5c5c5c"="" style="font-size: 10pt;">
            A brief example can illustrate how application is created. Suppose you need to create Asset management system then you need to add nouns
            e.g.: Equipment as Entity. Then add collective nouns as property to each entity e.g.: Name; Description and then create association between 2 entities
            e.g.: One Equipment has many Models. Then you can add Application roles and application security, as&nbsp;desired. When ready you
            can launch application and with launch application Turanto creates database, webpages, documentation and source code.<o:p></o:p>
        </span>
    </p><p class="MsoNormal" style="margin-bottom: 7.5pt; line-height: 18pt;">
        <span lang="EN-US" arial",sans-serif;mso-fareast-font-family="" "times=" " new=" " roman" ;color:#5c5c5c"="" style="font-size: 10pt;">
            The asset management application has number of entities (or common nouns) that are independent but in combined form are aggregated to manage assets.
            Assets are the items used by a company. The assets are related to a vendor, contract terms, purchase, manufacturer, item details, work order etc.
            All these are entities of an asset management application. The entities are created in Turanto through simple ‘Add Entity’ feature.
            Every entity has its own set of properties. For example: an asset item is related to a person. A ‘Person’ entity has properties
            like: First name, Last Name, Middle Initial, Sex, Age, Picture, and Phone number etc. Adding a property to an entity is same as adding an entity in application.
            Additionally users can also define the type of data that a property should have (say a web URL should follow ‘www.example.com’ format only).
            All entities of asset management application have association with each other. For example: one asset type has many assets. One complete configuration,
            a working asset management application is ready to use.<o:p></o:p>
        </span>
    </p>
    <p class="MsoNormal">
        <span lang="EN-US" arial",sans-serif"="" style="font-size: 10pt; line-height: 14.2667px;">
            <o:p>
                &nbsp;
            </o:p>
        </span>
    </p><p class="MsoNormal" style="margin-bottom: 7.5pt; line-height: 18pt;">
        <span style="font-weight: bolder;">
            <span lang="EN-US" arial",sans-serif;mso-fareast-font-family="" "times=" " new=" " roman" ;color:#5c5c5c"="" style="font-size: 10pt;">
                Download Source Code
            </span>
        </span>
        <span lang="EN-US" arial",sans-serif;mso-fareast-font-family="" "times=" " new=" " roman" ;color:#5c5c5c"="" style="font-size: 10pt;"><o:p></o:p></span>
    </p>
    <p class="MsoNormal" style="margin-bottom: 7.5pt; line-height: 18pt;">
        <span lang="EN-US" arial",sans-serif;mso-fareast-font-family="" "times=" " new=" " roman" ;color:#5c5c5c"="" style="font-size: 10pt;">
            Source code can be downloaded and open in Visual studio:
            <br>(i) The code is architected as per Microsoft recommendations for their .NET MVC (Model View Controller) framework.
            <br>(ii) It follows .NET 4.5 guidelines, and uses Entity libraries version 6.
            <br>(iii) It uses Twitter User Interface libraries for Responsive Layout<br>(iv) The source code is available as a Visual Studio 2013 solution.
            <o:p></o:p>
        </span>
    </p>
    <p class="MsoNormal">
        <span lang="EN-US" arial",sans-serif"="" style="font-size: 10pt; line-height: 14.2667px;"><o:p>&nbsp;</o:p></span>
    </p>
    <p class="MsoNormal" style="margin-bottom: 7.5pt; line-height: 18pt;">
        <span style="font-weight: bolder;">
            <span lang="EN-US" arial",sans-serif;mso-fareast-font-family="" "times=" " new=" " roman" ;color:#5c5c5c"="" style="font-size: 10pt;">Summary</span>
        </span>
        <span lang="EN-US" arial",sans-serif;mso-fareast-font-family="" "times=" " new=" " roman" ;color:#5c5c5c"="" style="font-size: 10pt;">
            <o:p></o:p>
        </span>
    </p><p style="margin: 0cm 0cm 7.5pt; line-height: 18pt; background: rgb(251, 252, 253);"></p>
    <p class="MsoNormal" style="margin-bottom: 7.5pt; line-height: 18pt;">
        <span lang="EN-US" arial",sans-serif;mso-fareast-font-family="" "times=" " new=" " roman" ;color:#5c5c5c"="" style="font-size: 10pt;">
            Turanto provides an upper hand to users who have stunning application but models relies on developers to write code. Eliminating the
            need of coding has on one hand benefits the common person, also allows programmers to prototype their application. The cloud application
            deployment allows application access through web devices (or devices with web browsers). The applications are easy to build, upgradable and can
            be updated with minimum efforts. Overall, Turanto provides an unmatched platform for application development through modeling simplicity and high end benefits.
        </span>
    </p>
</h2>'
           ,'<strong>Notice and Warning:</strong> This computer system is the property of Etelic and is intended for authorized users only.'
           ,'Turanto')
END
GO
Update[dbo].[tbl_CompanyInformation]
set [SMTPUser] = 'apikey',[SMTPPassword] = 'U0cuLWVGLW9YNDJUQjJrVjVyQ3V5RXdody41LXRKNnhLNU9OMDJjQkxpelhDUERXYW50Q2RIdnRiR2kwSFR4cWI1cWlv'  
Where [SMTPUser]='azure_15eb8f52958d116ced4774522747a486@azure.com' AND [SMTPPassword]='VHVyYW50bzIxNiM='
GO
if not exists (select * from sysobjects where name='tbl_FooterSection' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_FooterSection](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[WebLinkTitle] [nvarchar](255) NULL,
	[WebLink] [nvarchar](255) NULL,
	[DocumentUpload] [bigint] NULL,
	[CompanyInformationFooterSectionAssociationID] [bigint] NULL,
	[AssociatedFooterSectionTypeID] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[TenantId] [bigint] NULL,
 CONSTRAINT [PK_tbl_FooterSection] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
ALTER TABLE [dbo].[tbl_FooterSection]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_FooterSection_tbl_CompanyInformation_CompanyInformationFooterSectionAssociation] FOREIGN KEY([CompanyInformationFooterSectionAssociationID])
REFERENCES [dbo].[tbl_CompanyInformation] ([Id])
ALTER TABLE [dbo].[tbl_FooterSection] NOCHECK CONSTRAINT [FK_tbl_FooterSection_tbl_CompanyInformation_CompanyInformationFooterSectionAssociation]
INSERT INTO tbl_FooterSection
           ([Name],[WebLinkTitle],[WebLink],[DocumentUpload],[CompanyInformationFooterSectionAssociationID]
           ,[AssociatedFooterSectionTypeID],[DisplayValue],[TenantId])
VALUES('Legal Information','Legal Information','/PolicyAndService/Licensing.pdf',NULL,1,1,'',0),
      ('Privacy Policy','Privacy Policy','/PolicyAndService/PrivacyPolicy.pdf',NULL,1,1,'',0),
      ('Terms Of Service','Terms Of Service','/PolicyAndService/Terms_Of_Service.pdf',NULL,1,1,'',0),
      ('Third-Party Licenses','Third-Party Licenses','/PolicyAndService/Third_Party_Licenses.pdf',NULL,1,1,'',0),
      ('Cookie Policy','Cookie Policy','/PolicyAndService/Third_Party_Licenses.pdf',NULL,1,1,'',0),
      ('Email To','Email To','contact@turanto.com',NULL,1,1,'',0),
      ('Created With','Turanto','http://www.turanto.com/',NULL,1,1,'',0)
      
END
if not exists (select * from sysobjects where name='tbl_CompanyInformationCompanyListAssociation' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_CompanyInformationCompanyListAssociation](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[CompanyInformationID] [bigint] NULL,
	[CompanyListID] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[TenantId] [bigint] NULL,
 CONSTRAINT [PK_tbl_CompanyInformationCompanyListAssociation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
ALTER TABLE [dbo].[tbl_CompanyInformationCompanyListAssociation]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_CompanyInformationCompanyListAssociation_tbl_CompanyInformation_CompanyInformation] FOREIGN KEY([CompanyInformationID])
REFERENCES [dbo].[tbl_CompanyInformation] ([Id])
ALTER TABLE [dbo].[tbl_CompanyInformationCompanyListAssociation] NOCHECK CONSTRAINT [FK_tbl_CompanyInformationCompanyListAssociation_tbl_CompanyInformation_CompanyInformation]
	INSERT INTO tbl_CompanyInformationCompanyListAssociation([CompanyInformationID],[CompanyListID],[DisplayValue],[TenantId])
	VALUES (1,0,'Global',0)
END

--Alter Tables for add soft delete column
--tbl_PropertyValidationandFormat
GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_PropertyValidationandFormat')
                    AND name = 'IsDeleted' ) 
ALTER TABLE tbl_PropertyValidationandFormat ADD IsDeleted [bit] NULL

GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_PropertyValidationandFormat')
                    AND name = 'DeleteDateTime' ) 
ALTER TABLE tbl_PropertyValidationandFormat ADD DeleteDateTime [datetime] NULL
--tbl_DataMetric
GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_PropertyValidationandFormat')
                    AND name = 'ExportDataLogId' ) 
ALTER TABLE tbl_PropertyValidationandFormat ADD ExportDataLogId [bigint] NULL
GO


IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_DataMetric')
                    AND name = 'IsDeleted' ) 
ALTER TABLE tbl_DataMetric ADD IsDeleted [bit] NULL
GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_DataMetric')
                    AND name = 'DeleteDateTime' ) 
ALTER TABLE tbl_DataMetric ADD DeleteDateTime [datetime] NULL
--tbl_DataMetrictype
GO

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_DataMetric')
                    AND name = 'ExportDataLogId' ) 
ALTER TABLE tbl_DataMetric ADD ExportDataLogId [bigint] NULL
GO


IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_DataMetrictype')
                    AND name = 'IsDeleted' ) 
ALTER TABLE tbl_DataMetrictype ADD IsDeleted [bit] NULL
GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_DataMetrictype')
                    AND name = 'DeleteDateTime' ) 
ALTER TABLE tbl_DataMetrictype ADD DeleteDateTime [datetime] NULL
--tbl_DocumentTemplate
GO

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_DataMetrictype')
                    AND name = 'ExportDataLogId' ) 
ALTER TABLE tbl_DataMetrictype ADD ExportDataLogId [bigint] NULL
GO

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_DocumentTemplate')
                    AND name = 'IsDeleted' ) 
ALTER TABLE tbl_DocumentTemplate ADD IsDeleted [bit] NULL
GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_DocumentTemplate')
                    AND name = 'DeleteDateTime' ) 
ALTER TABLE tbl_DocumentTemplate ADD DeleteDateTime [datetime] NULL
--tbl_CompanyInformation
GO

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_DocumentTemplate')
                    AND name = 'ExportDataLogId' ) 
ALTER TABLE tbl_DocumentTemplate ADD ExportDataLogId [bigint] NULL
GO



IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_CompanyInformation')
                    AND name = 'IsDeleted' ) 
ALTER TABLE tbl_CompanyInformation ADD IsDeleted [bit] NULL
GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_CompanyInformation')
                    AND name = 'DeleteDateTime' ) 
ALTER TABLE tbl_CompanyInformation ADD DeleteDateTime [datetime] NULL
--tbl_FooterSection
GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_CompanyInformation')
                    AND name = 'ExportDataLogId' ) 
ALTER TABLE tbl_CompanyInformation ADD ExportDataLogId [bigint] NULL
GO


IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_FooterSection')
                    AND name = 'IsDeleted' ) 
ALTER TABLE tbl_FooterSection ADD IsDeleted [bit] NULL
GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_FooterSection')
                    AND name = 'DeleteDateTime' ) 
ALTER TABLE tbl_FooterSection ADD DeleteDateTime [datetime] NULL
--tbl_CompanyInformationCompanyListAssociation
GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_FooterSection')
                    AND name = 'ExportDataLogId' ) 
ALTER TABLE tbl_FooterSection ADD ExportDataLogId [bigint] NULL
GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_CompanyInformationCompanyListAssociation')
                    AND name = 'IsDeleted' ) 
ALTER TABLE tbl_CompanyInformationCompanyListAssociation ADD IsDeleted [bit] NULL
GO
IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_CompanyInformationCompanyListAssociation')
                    AND name = 'DeleteDateTime' ) 
ALTER TABLE tbl_CompanyInformationCompanyListAssociation ADD DeleteDateTime [datetime] NULL

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_CompanyInformationCompanyListAssociation')
                    AND name = 'ExportDataLogId' ) 
ALTER TABLE tbl_CompanyInformationCompanyListAssociation ADD ExportDataLogId [bigint] NULL
GO


if(Select count(*) from tbl_AppSetting Where [Key] = 'AllowedRoles2FA') <= '0'
begin
INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'AllowedRoles2FA', N'Admin', N'Bypass 2FA authentication for Roles.',8, 1, CAST(getdate() AS DateTime), N'Admin', N'AllowedRoles2FA')
end

if((Select count(*) from tbl_AppSetting Where [Key] = 'PasswordRequireAlphaNumericCharacter') > '0' and (Select count(*) from tbl_AppSetting Where [Key] = 'PasswordRequireSpecialCharacter') = '0')
begin

delete from [dbo].[tbl_AppSetting] Where [Key]='PasswordRequireAlphaNumericCharacter' and [Description] = 'Password require at least one alphanumeric(Non digit or letter) character.'

INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
VALUES (N'PasswordRequireSpecialCharacter', N'Yes', N'Password requires at least one special character.', 3, 1, CAST(getdate() AS DateTime), N'Admin', N'PasswordRequireSpecialCharacter')
end

if((Select count(*) from tbl_AppSetting Where [Key] = 'PasswordRequireAlphaNumericCharacter') > '0' and (Select count(*) from tbl_AppSetting Where [Key] = 'PasswordRequireSpecialCharacter') > '0')
begin
delete from [dbo].[tbl_AppSetting] Where [Key]='PasswordRequireAlphaNumericCharacter' and [Description] = 'Password require at least one alphanumeric(Non digit or letter) character.'
end
GO
IF NOT EXISTS (SELECT * FROM sys.types where is_table_type = 1 and name ='SQLReportTableTypeParameter')
CREATE TYPE SQLReportTableTypeParameter AS TABLE (ID NVARCHAR(MAX) NULL)

IF NOT EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_Condition') AND name = 'ExportDetailConditionsID')
begin					
	ALTER TABLE tbl_Condition ADD ExportDetailConditionsID bigint NULL
end

if not exists (select * from sysobjects where name='tbl_ExportDataConfiguration' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_ExportDataConfiguration](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[AutoNo] [bigint] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[BackGroundColor] [nvarchar](255) NULL,
	[FontColor] [nvarchar](255) NULL,
	[EntityName] [nvarchar](255) NULL,
	[AllowedRoles] [nvarchar](max) NULL,
	[ToolTip] [nvarchar](max) NULL,
	[EntitySubscribe] [bit] NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[Disable] [bit] NULL,
	[EnableDelete] [bit] NULL,
	[IsRootDeleted] [bit] NULL,
	[UploadOneDrive] [bit] NULL,
	[UploadGoogleDrive] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[DeleteDateTime] [datetime] NULL,
	[ExportDataLogId] [bigint] NULL,
	CONSTRAINT [PK_tbl_ExportDataConfiguration] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ExportDataConfiguration')
                    AND name = 'EnableDelete')
BEGIN
ALTER TABLE tbl_ExportDataConfiguration ADD EnableDelete [bit] NULL
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ExportDataConfiguration')
                    AND name = 'IsRootDeleted')
BEGIN
ALTER TABLE tbl_ExportDataConfiguration ADD IsRootDeleted [bit] NULL
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ExportDataConfiguration')
                    AND name = 'IsDeleted')
BEGIN
ALTER TABLE tbl_ExportDataConfiguration ADD IsDeleted [bit] NULL
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ExportDataConfiguration')
                    AND name = 'DeleteDateTime' ) 
BEGIN
ALTER TABLE tbl_ExportDataConfiguration ADD DeleteDateTime [datetime] NULL
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ExportDataConfiguration')
                    AND name = 'ExportDataLogId' )
BEGIN					
ALTER TABLE tbl_ExportDataConfiguration ADD ExportDataLogId [bigint] NULL
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ExportDataConfiguration')
                    AND name = 'UploadOneDrive')
BEGIN
ALTER TABLE tbl_ExportDataConfiguration ADD UploadOneDrive [bit] NULL
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ExportDataConfiguration')
                    AND name = 'UploadGoogleDrive')
BEGIN
ALTER TABLE tbl_ExportDataConfiguration ADD UploadGoogleDrive [bit] NULL
END

if not exists (select * from sysobjects where name='tbl_ExportDataDetails' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_ExportDataDetails](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[AssociationName] [nvarchar](max) NULL,
	[EntitySubscribe] [bit] NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[ParentEntity] [nvarchar](max) NULL,
	[ChildEntity] [nvarchar](max) NULL,
	[IsNested] [bit] NULL,
	[Hierarchy] [nvarchar](max) NULL,
	[ExportDataConfigurationExportDataDetailsAssociationID] [bigint] NULL,
	[IsDeleted] [bit] NULL,
	[DeleteDateTime] [datetime] NULL,
	[ExportDataLogId] [bigint] NULL,
 CONSTRAINT [PK_tbl_ExportDataDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ExportDataDetails')
                    AND name = 'IsDeleted' )
BEGIN
ALTER TABLE tbl_ExportDataDetails ADD IsDeleted [bit] NULL
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ExportDataDetails')
                    AND name = 'DeleteDateTime' ) 
BEGIN
ALTER TABLE tbl_ExportDataDetails ADD DeleteDateTime [datetime] NULL
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ExportDataDetails')
                    AND name = 'ExportDataLogId' ) 
BEGIN
ALTER TABLE tbl_ExportDataDetails ADD ExportDataLogId [bigint] NULL
END

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'FK_tbl_ExportDataDetails_tbl_ExportDataConfiguration_ExportDataConfigurationExportDataDetailsAssociation')
BEGIN
 ALTER TABLE [dbo].[tbl_ExportDataDetails] DROP CONSTRAINT [FK_tbl_ExportDataDetails_tbl_ExportDataConfiguration_ExportDataConfigurationExportDataDetailsAssociation]
END;

ALTER TABLE [dbo].[tbl_ExportDataDetails]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_ExportDataDetails_tbl_ExportDataConfiguration_ExportDataConfigurationExportDataDetailsAssociation] FOREIGN KEY([ExportDataConfigurationExportDataDetailsAssociationID])
REFERENCES [dbo].[tbl_ExportDataConfiguration] ([Id])


ALTER TABLE [dbo].[tbl_ExportDataDetails] NOCHECK CONSTRAINT [FK_tbl_ExportDataDetails_tbl_ExportDataConfiguration_ExportDataConfigurationExportDataDetailsAssociation]

if not exists (select * from sysobjects where name='tbl_ExportDataLog' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_ExportDataLog](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[AutoNo] [bigint] NULL,
	[Tag] [nvarchar](max) NULL,
	[ExportDataConfigurationExportDataLogAssociationID] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[DateTimeRunInsertBy] [nvarchar](255) NULL,
	[DateTimeRunUser] [nvarchar](255) NULL,
	[Notes] [nvarchar](max) NULL,
	[Status] [nvarchar](50) NULL,
	[IsDeleted] [bit] NULL,
	[DeleteDateTime] [datetime] NULL,
	[ExportDataLogId] [bigint] NULL,
	[AssociatedExportDataLogStatusID] [bigint] NULL,
 CONSTRAINT [PK_tbl_ExportDataLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ExportDataLog')
                    AND name = 'Summary' ) 
BEGIN
ALTER TABLE tbl_ExportDataLog ADD Summary [nvarchar](max) NULL
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ExportDataLog')
                    AND name = 'OneDriveUrl' ) 
BEGIN
ALTER TABLE tbl_ExportDataLog ADD OneDriveUrl [nvarchar](max) NULL
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_ExportDataLog')
                    AND name = 'GoogleDriveUrl' ) 
BEGIN
ALTER TABLE tbl_ExportDataLog ADD GoogleDriveUrl [nvarchar](max) NULL
END

if not exists (select * from sysobjects where name='tbl_ExportDataLogstatus' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_ExportDataLogstatus](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[IsDeleted] [bit] NULL,
	[DeleteDateTime] [datetime] NULL,
	[ExportDataLogId] [bigint] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_ExportDataLogstatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

if exists (select * from sysobjects where name='tbl_ExportDataLogstatus' and xtype='U') 
if(Select count(*) from tbl_ExportDataLogstatus Where [Name] = 'Open') <= '0'
begin
INSERT INTO tbl_ExportDataLogstatus (Name,Description,DisplayValue)
VALUES ('Open','Open','Open')
end

if exists (select * from sysobjects where name='tbl_ExportDataLogstatus' and xtype='U') 
if(Select count(*) from tbl_ExportDataLogstatus Where [Name] = 'Purged') <= '0'
begin
INSERT INTO tbl_ExportDataLogstatus (Name,Description,DisplayValue)
VALUES ('Purged','Purged','Purged')
end

if exists (select * from sysobjects where name='tbl_ExportDataLogstatus' and xtype='U') 
if(Select count(*) from tbl_ExportDataLogstatus Where [Name] = 'Restored') <= '0'
begin
INSERT INTO tbl_ExportDataLogstatus (Name,Description,DisplayValue)
VALUES ('Restored','Restored','Restored')
end


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'FK_tbl_ExportDataLog_tbl_ExportDataConfiguration')
BEGIN
 ALTER TABLE [dbo].[tbl_ExportDataLog] DROP CONSTRAINT [FK_tbl_ExportDataLog_tbl_ExportDataConfiguration]
END;
ALTER TABLE [dbo].[tbl_ExportDataLog]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_ExportDataLog_tbl_ExportDataConfiguration] FOREIGN KEY([ExportDataConfigurationExportDataLogAssociationID])
REFERENCES [dbo].[tbl_ExportDataConfiguration] ([Id])
ALTER TABLE [dbo].[tbl_ExportDataLog] NOCHECK CONSTRAINT [FK_tbl_ExportDataLog_tbl_ExportDataConfiguration]

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'FK_tbl_ExportDataLog_tbl_ExportDataConfiguration_ExportDataConfigurationExportDataLogAssociation')
BEGIN
 ALTER TABLE [dbo].[tbl_ExportDataLog] DROP CONSTRAINT [FK_tbl_ExportDataLog_tbl_ExportDataConfiguration_ExportDataConfigurationExportDataLogAssociation]
END;
ALTER TABLE [dbo].[tbl_ExportDataLog]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_ExportDataLog_tbl_ExportDataConfiguration_ExportDataConfigurationExportDataLogAssociation] FOREIGN KEY([ExportDataConfigurationExportDataLogAssociationID])
REFERENCES [dbo].[tbl_ExportDataConfiguration] ([Id])
ALTER TABLE [dbo].[tbl_ExportDataLog] NOCHECK CONSTRAINT [FK_tbl_ExportDataLog_tbl_ExportDataConfiguration_ExportDataConfigurationExportDataLogAssociation]

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'FK_tbl_ExportDataLog_tbl_ExportDataLogstatus_AssociatedExportDataLogStatus')
BEGIN
ALTER TABLE [dbo].[tbl_ExportDataLog] DROP CONSTRAINT [FK_tbl_ExportDataLog_tbl_ExportDataLogstatus_AssociatedExportDataLogStatus]
END;
ALTER TABLE [dbo].[tbl_ExportDataLog]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_ExportDataLog_tbl_ExportDataLogstatus_AssociatedExportDataLogStatus] FOREIGN KEY([AssociatedExportDataLogStatusID])
REFERENCES [dbo].[tbl_ExportDataLogstatus] ([Id])
ALTER TABLE [dbo].[tbl_ExportDataLog] NOCHECK CONSTRAINT [FK_tbl_ExportDataLog_tbl_ExportDataLogstatus_AssociatedExportDataLogStatus]
GO

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_CompanyInformation')
                    AND name = 'OneDriveClientId' ) 
BEGIN
ALTER TABLE tbl_CompanyInformation ADD OneDriveClientId [nvarchar](250) NULL
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_CompanyInformation')
                    AND name = 'OneDriveSecret' ) 
BEGIN
ALTER TABLE tbl_CompanyInformation ADD OneDriveSecret [nvarchar](250) NULL
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_CompanyInformation')
                    AND name = 'OneDriveTenantId' ) 
BEGIN
ALTER TABLE tbl_CompanyInformation ADD OneDriveTenantId [nvarchar](250) NULL
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_CompanyInformation')
                    AND name = 'OneDriveUserName' ) 
BEGIN
ALTER TABLE tbl_CompanyInformation ADD OneDriveUserName [nvarchar](250) NULL
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_CompanyInformation')
                    AND name = 'OneDrivePassword' ) 
BEGIN
ALTER TABLE tbl_CompanyInformation ADD OneDrivePassword [nvarchar](250) NULL
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_CompanyInformation')
                    AND name = 'OneDriveFolderName' ) 
BEGIN
ALTER TABLE tbl_CompanyInformation ADD OneDriveFolderName [nvarchar](250) NULL
END

if not exists (select * from sysobjects where name='tbl_MenuBar' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_MenuBar](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,	
	[AutoNo] [bigint] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Horizontal] [bit] NULL,
	[Disabled] [bit] NULL,
	[Roles] [nvarchar](max) NULL,
	[EntitySubscribe] [bit] NULL,
	[DisplayValue] [nvarchar](max) NULL,	
	[EnableDelete] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[DeleteDateTime] [datetime] NULL,
	[ExportDataLogId] [bigint] NULL,
	 CONSTRAINT [PK_tbl_MenuBar] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
 
END


if not exists (select * from sysobjects where name='tbl_MenuItem' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_MenuItem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[AutoNo] [bigint] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[ToolTip] [nvarchar](max) NULL,
	[Entity] [nvarchar](255) NULL,
	[LinkAddress] [nvarchar](255) NULL,
	[DisplayOrder] [nvarchar](255) NULL,
	[ClassIcon] [nvarchar](255) NULL,
	[EntitySubscribe] [bit] NULL,
	[MenuItemMenuItemAssociationID] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[Action] [nvarchar](255) NULL,
	[EntityValue] [nvarchar](255) NULL,
	[SavedSearch] [nvarchar](255) NULL,	
	[EnableDelete] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[DeleteDateTime] [datetime] NULL,
	[ExportDataLogId] [bigint] NULL,
 CONSTRAINT [PK_tbl_MenuItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
if not exists (select * from sysobjects where name='tbl_MenuBarMenuItemAssociation' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_MenuBarMenuItemAssociation](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[OrderNumber] [int] NULL,
	[MenuBarID] [bigint] NULL,
	[MenuItemID] [bigint] NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[EnableDelete] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[DeleteDateTime] [datetime] NULL,
	[ExportDataLogId] [bigint] NULL,
 CONSTRAINT [PK_tbl_MenuBarMenuItemAssociation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[tbl_MenuBarMenuItemAssociation]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_MenuBarMenuItemAssociation_tbl_MenuBar_MenuBar] FOREIGN KEY([MenuBarID])
REFERENCES [dbo].[tbl_MenuBar] ([Id])
ALTER TABLE [dbo].[tbl_MenuBarMenuItemAssociation] NOCHECK CONSTRAINT [FK_tbl_MenuBarMenuItemAssociation_tbl_MenuBar_MenuBar]
ALTER TABLE [dbo].[tbl_MenuBarMenuItemAssociation]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_MenuBarMenuItemAssociation_tbl_MenuItem_MenuItem] FOREIGN KEY([MenuItemID])
REFERENCES [dbo].[tbl_MenuItem] ([Id])
ALTER TABLE [dbo].[tbl_MenuBarMenuItemAssociation] NOCHECK CONSTRAINT [FK_tbl_MenuBarMenuItemAssociation_tbl_MenuItem_MenuItem]
END

IF NOT EXISTS ( SELECT  *
            FROM    syscolumns
            WHERE   id = OBJECT_ID('tbl_MenuBarMenuItemAssociation')
                    AND name = 'OrderNumber' ) 
BEGIN
ALTER TABLE tbl_MenuBarMenuItemAssociation ADD OrderNumber [int] NULL
END


if(Select count(*) from tbl_AppSetting Where [Key] = 'ShowMenuOnTop') <= '0'
	begin
		INSERT [dbo].[tbl_AppSetting] ([Key], [Value], [Description], [AssociatedAppSettingGroupID], [IsDefault], [LastUpdatedBy], [LastUpdatedByUser], [DisplayValue]) 
		VALUES (N'ShowMenuOnTop', N'false', N'Default unchecked and displays menu in left, if checked menu bar will be displayed on top.',null, 1, CAST(getdate() AS DateTime), N'Admin', N'ShowMenuOnTop')
	end

IF NOT EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_RuleAction') AND name = 'TemplateId' ) 
BEGIN
ALTER TABLE tbl_RuleAction ADD TemplateId [bigint] NULL
END

--tbl_IconType
IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_IconType'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_IconType') AND  name = 'IsDeleted')
	BEGIN
		ALTER TABLE tbl_IconType ADD IsDeleted [bit] NULL
	END
 END

 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_IconType'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_IconType') AND  name = 'DeleteDateTime')
	BEGIN
		ALTER TABLE tbl_IconType ADD DeleteDateTime [datetime] NULL
	END
 END

 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_IconType'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_IconType') AND  name = 'ExportDataLogId')
	BEGIN
		ALTER TABLE tbl_IconType ADD ExportDataLogId [bigint] NULL
	END
 END
 --tbl_BackColor
 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_BackColor'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_BackColor') AND  name = 'IsDeleted')
	BEGIN
		ALTER TABLE tbl_BackColor ADD IsDeleted [bit] NULL
	END
 END

 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_BackColor'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_BackColor') AND  name = 'DeleteDateTime')
	BEGIN
		ALTER TABLE tbl_BackColor ADD DeleteDateTime [datetime] NULL
	END
 END

 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_BackColor'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_BackColor') AND  name = 'ExportDataLogId')
	BEGIN
		ALTER TABLE tbl_BackColor ADD ExportDataLogId [bigint] NULL
	END
 END

 --tbl_Notificationtype
  IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_Notificationtype'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_Notificationtype') AND  name = 'IsDeleted')
	BEGIN
		ALTER TABLE tbl_Notificationtype ADD IsDeleted [bit] NULL
	END
 END

 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_Notificationtype'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_Notificationtype') AND  name = 'DeleteDateTime')
	BEGIN
		ALTER TABLE tbl_Notificationtype ADD DeleteDateTime [datetime] NULL
	END
 END

 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_Notificationtype'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_Notificationtype') AND  name = 'ExportDataLogId')
	BEGIN
		ALTER TABLE tbl_Notificationtype ADD ExportDataLogId [bigint] NULL
	END
 END
 --tbl_Notification
  IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_Notification'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_Notification') AND  name = 'IsDeleted')
	BEGIN
		ALTER TABLE tbl_Notification ADD IsDeleted [bit] NULL
	END
 END

 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_Notification'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_Notification') AND  name = 'DeleteDateTime')
	BEGIN
		ALTER TABLE tbl_Notification ADD DeleteDateTime [datetime] NULL
	END
 END

 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_Notification'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_Notification') AND  name = 'ExportDataLogId')
	BEGIN
		ALTER TABLE tbl_Notification ADD ExportDataLogId [bigint] NULL
	END
 END
 --tbl_NotificationRolestabAssociation

  IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_NotificationRolestabAssociation'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_NotificationRolestabAssociation') AND  name = 'IsDeleted')
	BEGIN
		ALTER TABLE tbl_NotificationRolestabAssociation ADD IsDeleted [bit] NULL
	END
 END

 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_NotificationRolestabAssociation'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_NotificationRolestabAssociation') AND  name = 'DeleteDateTime')
	BEGIN
		ALTER TABLE tbl_NotificationRolestabAssociation ADD DeleteDateTime [datetime] NULL
	END
 END

 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_NotificationRolestabAssociation'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_NotificationRolestabAssociation') AND  name = 'ExportDataLogId')
	BEGIN
		ALTER TABLE tbl_NotificationRolestabAssociation ADD ExportDataLogId [bigint] NULL
	END
 END
 --tbl_NotificationUserAssociation
 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_NotificationUserAssociation'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_NotificationUserAssociation') AND  name = 'IsDeleted')
	BEGIN
		ALTER TABLE tbl_NotificationUserAssociation ADD IsDeleted [bit] NULL
	END
 END

 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_NotificationUserAssociation'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_NotificationUserAssociation') AND  name = 'DeleteDateTime')
	BEGIN
		ALTER TABLE tbl_NotificationUserAssociation ADD DeleteDateTime [datetime] NULL
	END
 END

 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_NotificationUserAssociation'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_NotificationUserAssociation') AND  name = 'ExportDataLogId')
	BEGIN
		ALTER TABLE tbl_NotificationUserAssociation ADD ExportDataLogId [bigint] NULL
	END
 END

--------------------------Verb Setting-------
if not exists (select * from sysobjects where name='tbl_VerbGroup' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_VerbGroup](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[InternalName] [nvarchar](255) NULL,
	[Icon] [nvarchar](255) NULL,
	[BackGroundColor] [nvarchar](255) NULL,
	[FontColor] [nvarchar](255) NULL,
	[Flag1] [bit] NULL,
	[DisplayOrder] [int] NULL,
	[EntityInternalName] [nvarchar](255) NULL,
	[UIGroupInternalName] [nvarchar](255) NULL,
	[GroupId] [int] NULL,
	[EntitySubscribe] [bit] NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[EnableDelete] [bit] NULL,
  CONSTRAINT [PK_tbl_VerbGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

if not exists (select * from sysobjects where name='tbl_VerbsName' and xtype='U')
BEGIN
CREATE TABLE [dbo].[tbl_VerbsName](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[BackGroundColor] [nvarchar](255) NULL,
	[FontColor] [nvarchar](255) NULL,
	[DisplayOrder] [int] NULL,
	[VerbIcon] [nvarchar](255) NULL,
	[VerbId] [nvarchar](max) NULL,
	[VerbTypeID] [bigint] NULL,
	[VerbName] [nvarchar](255) NULL,
	[EntitySubscribe] [bit] NULL,
	[VerbNameSelectID] [bigint] NULL,
	[VerbIds] [nvarchar](max) NULL,
	[DisplayValue] [nvarchar](max) NULL,
	[EnableDelete] [bit] NULL,
   CONSTRAINT [PK_tbl_VerbsName] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
ALTER TABLE [dbo].[tbl_VerbsName]  WITH NOCHECK ADD  CONSTRAINT [FK_tbl_VerbsName_tbl_VerbGroup_VerbNameSelect] FOREIGN KEY([VerbNameSelectID])
REFERENCES [dbo].[tbl_VerbGroup] ([Id])
ALTER TABLE [dbo].[tbl_VerbsName] CHECK CONSTRAINT [FK_tbl_VerbsName_tbl_VerbGroup_VerbNameSelect]
END
---------------------------
 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_VerbGroup'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_VerbGroup') AND  name = 'IsDeleted')
	BEGIN
		ALTER TABLE tbl_VerbGroup ADD IsDeleted [bit] NULL
	END
 END

 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_VerbGroup'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_VerbGroup') AND  name = 'DeleteDateTime')
	BEGIN
		ALTER TABLE tbl_VerbGroup ADD DeleteDateTime [datetime] NULL
	END
 END

 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_VerbGroup'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_VerbGroup') AND  name = 'ExportDataLogId')
	BEGIN
		ALTER TABLE tbl_VerbGroup ADD ExportDataLogId [bigint] NULL
	END
 END
 ----------------------------
  IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_VerbsName'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_VerbsName') AND  name = 'IsDeleted')
	BEGIN
		ALTER TABLE tbl_VerbsName ADD IsDeleted [bit] NULL
	END
 END

 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_VerbsName'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_VerbsName') AND  name = 'DeleteDateTime')
	BEGIN
		ALTER TABLE tbl_VerbsName ADD DeleteDateTime [datetime] NULL
	END
 END

 IF EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('tbl_VerbsName'))
 BEGIN
	IF NOT EXISTS (SELECT* FROM  syscolumns WHERE  id = OBJECT_ID('tbl_VerbsName') AND  name = 'ExportDataLogId')
	BEGIN
		ALTER TABLE tbl_VerbsName ADD ExportDataLogId [bigint] NULL
	END
 END