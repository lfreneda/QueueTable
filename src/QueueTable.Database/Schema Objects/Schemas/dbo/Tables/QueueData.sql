CREATE TABLE [dbo].[QueueData] (
    [QueueID] [int] NOT NULL,
    [TextData] [varchar](4000) NOT NULL
) ON [PRIMARY]

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QueueData] ON [dbo].[QueueData]
(
    [QueueID] ASC
)