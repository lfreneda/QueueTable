using System;

namespace QueueTable
{
    public class QueueItem<T> where T : class {

        protected QueueItem() { }

        public QueueItem(string title, T data) {
            Title = title;
            Data = data;
        }

        public string Title { get; set; }
        public T Data { get; protected set; }
        public int Id { get; protected set; }
        public DateTime Date { get; protected set; }
        public QueueStatus Status { get; protected set; }

        public void SetAsProcessed() {
            Status = QueueStatus.Processed;
        }
    }
}