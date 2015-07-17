CREATE TABLE [dbo].[History]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[ThreadId] INT NOT NULL,
    [Time] DATETIME NOT NULL,
	[Score] INT NOT NULL, 
    CONSTRAINT [PK_dbo.History] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_History_Thread] FOREIGN KEY ([ThreadId]) REFERENCES [dbo].[Thread]([Id])
)

GO
CREATE NONCLUSTERED INDEX [HistoryIndex]
    ON [dbo].[History]([Id],[ThreadId],[Time],[Score] ASC);