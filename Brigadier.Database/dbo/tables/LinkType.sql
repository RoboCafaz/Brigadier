CREATE TABLE [dbo].[LinkType]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[Type] NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_dbo.LinkType] PRIMARY KEY CLUSTERED ([Id] ASC)
)

GO
CREATE NONCLUSTERED INDEX [LinkTypeIndex]
    ON [dbo].[LinkType]([Id],[Type] ASC);