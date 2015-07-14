CREATE TABLE [dbo].[Thread]
(
	[Id] INT IDENTITY(1,1) NOT NULL, 
    [Url] NVARCHAR(50) NOT NULL, 
    [Comment] BIT NOT NULL,
	[Location] NVARCHAR(50) NOT NULL,
	[LinkTypeId] INT NOT NULL
    CONSTRAINT [PK_dbo.Thread] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Thread_Type] FOREIGN KEY ([LinkTypeId]) REFERENCES [dbo].[LinkType]([Id])
)

GO
CREATE NONCLUSTERED INDEX [ThreadIndex]
    ON [dbo].[Thread]([Id],[Url],[Comment] ASC);