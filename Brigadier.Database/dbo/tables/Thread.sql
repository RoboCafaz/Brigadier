CREATE TABLE [dbo].[Thread]
(
	[Id] INT IDENTITY(1,1) NOT NULL, 
    [Url] NVARCHAR(MAX) NOT NULL, 
    [Author] NVARCHAR(50) NOT NULL, 
	[Sub] NVARCHAR(50) NOT NULL,
	[LinkTypeId] INT NOT NULL,
    [TargetUrl] NVARCHAR(MAX) NOT NULL, 
    [TargetAuthor] NVARCHAR(50) NOT NULL, 
	[TargetSub] NVARCHAR(50) NOT NULL,
    [Comment] BIT NOT NULL,
    [Created] DATETIME NOT NULL, 
    CONSTRAINT [PK_dbo.Thread] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Thread_Type] FOREIGN KEY ([LinkTypeId]) REFERENCES [dbo].[LinkType]([Id])
)

GO
CREATE NONCLUSTERED INDEX [ThreadTargetIndex]
    ON [dbo].[Thread]([Id],[TargetSub],[TargetAuthor],[Comment] ASC);
	
GO
CREATE NONCLUSTERED INDEX [ThreadLocalIndex]
    ON [dbo].[Thread]([Id],[Author],[Sub],[LinkTypeId] ASC);