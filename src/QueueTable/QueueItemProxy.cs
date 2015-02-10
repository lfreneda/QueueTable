using System;
using Newtonsoft.Json;

namespace QueueTable
{
    internal class QueueItemProxy<T> : QueueItem<T> where T : class
    {
        public QueueItemProxy(int id, DateTime queueDateTime, string title, int status, string textData)
        {
            Id = id;
            Date = queueDateTime;
            Title = title;
            Status = (QueueStatus)status;
            Data = JsonConvert.DeserializeObject<T>(textData);
        }
    }
}