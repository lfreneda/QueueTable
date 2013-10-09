using Newtonsoft.Json;

namespace QueueTable
{
    internal class QueueItemProxy<T> : QueueItem<T> where T : class {
        public QueueItemProxy(dynamic result) {
            Id = result.QueueID;
            Date = result.QueueDateTime;
            Title = result.Title;
            Status = (QueueStatus)result.Status;
            Data = JsonConvert.DeserializeObject<T>(result.TextData);
        }
    }
}