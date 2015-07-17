CREATE TABLE [dbo].[Post]
(
	[Id] INT IDENTITY(1,1) NOT NULL, 
	[LocalThreadId] INT NOT NULL,
	[LinkTypeId] INT NOT NULL,
    [TargetThreadId] INT NOT NULL,
    [Created] DATETIME NOT NULL, 
    [Done] BIT NOT NULL, 
    CONSTRAINT [PK_dbo.Post] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Thread_Link] FOREIGN KEY ([LinkTypeId]) REFERENCES [dbo].[LinkType]([Id]),
    CONSTRAINT [FK_Thread_Local] FOREIGN KEY ([LocalThreadId]) REFERENCES [dbo].[Thread]([Id]),
    CONSTRAINT [FK_Thread_Target] FOREIGN KEY ([TargetThreadId]) REFERENCES [dbo].[Thread]([Id])
)