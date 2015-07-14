CREATE TABLE [dbo].[WatchedSub]
(
	[Id] INT IDENTITY(1,1) NOT NULL, 
    [Url] NVARCHAR(25) NOT NULL,
    CONSTRAINT [PK_dbo.WatchedSub] PRIMARY KEY CLUSTERED ([Id] ASC), 
)

GO
CREATE NONCLUSTERED INDEX [WatchedSubIndex]
    ON [dbo].[WatchedSub]([Id],[Url] ASC);