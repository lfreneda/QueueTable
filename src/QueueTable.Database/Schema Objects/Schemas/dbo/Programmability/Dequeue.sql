CREATE procedure [dbo].[Dequeue]
	@BatchSize int
AS

set nocount on

declare @Batch table (QueueID int, QueueDateTime datetime, Title nvarchar(255))

begin tran

insert into @Batch
select Top (@BatchSize) QueueID, QueueDateTime, Title from QueueMeta
WITH (UPDLOCK, HOLDLOCK)
where Status = 0
order by QueueDateTime ASC

declare @ItemsToUpdate int
set @ItemsToUpdate = @@ROWCOUNT

update QueueMeta
SET Status = 1
WHERE QueueID IN (select QueueID from @Batch)
AND Status = 0

if @@ROWCOUNT = @ItemsToUpdate
begin
    commit tran
    select b.*, q.TextData from @Batch b
    inner join QueueData q on q.QueueID = b.QueueID
    print 'SUCCESS'
end
else
begin
    rollback tran
    print 'FAILED'
end