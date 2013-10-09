CREATE TABLE [dbo].[QueueMeta]
(
	[QueueID] INT NOT NULL ,
	[QueueDateTime] [datetime] NOT NULL,
    [Title] [nvarchar](255) NOT NULL,
    [Status] [int] NOT NULL,
	CONSTRAINT [PK_Queue] PRIMARY KEY CLUSTERED ([QueueID] ASC)
)

GO

CREATE NONCLUSTERED INDEX [IX_QueueDateTime] ON [dbo].[QueueMeta]
(
    [QueueDateTime] ASC,
    [Status] ASC
)
INCLUDE ([Title]) 