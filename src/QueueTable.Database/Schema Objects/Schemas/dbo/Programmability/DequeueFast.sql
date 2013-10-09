CREATE PROCEDURE [dbo].[DequeueFast]
	@BatchSize int
AS

set nocount on

update top(@BatchSize) QueueMeta WITH (UPDLOCK, READPAST)
SET Status = 1
OUTPUT inserted.QueueID, inserted.[Status], inserted.QueueDateTime, inserted.Title, qd.TextData
FROM QueueMeta qm
INNER JOIN QueueData qd
    ON qm.QueueID = qd.QueueID
WHERE Status = 0