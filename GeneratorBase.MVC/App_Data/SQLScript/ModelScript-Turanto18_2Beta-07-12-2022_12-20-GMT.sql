CREATE TABLE [dbo].[tbl_FileDocument](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[DocumentName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](255),
	[AttachDocument] [bigint],
	[DateCreated] [datetime] NOT NULL,
	[DateLastUpdated] [datetime] NOT NULL,
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_FileDocument] PRIMARY KEY CLUSTERED 
(
	[Id]
)WITH (IGNORE_DUP_KEY = OFF)
)

CREATE TABLE [dbo].[tbl_Customer](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConcurrencyKey] [timestamp] NOT NULL,
	[AutoNo] [bigint],
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max),
	[EntitySubscribe] [bit],
	[DisplayValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_Customer] PRIMARY KEY CLUSTERED 
(
	[Id]
)WITH (IGNORE_DUP_KEY = OFF)
)



----------------------
