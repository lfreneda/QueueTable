using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Simple.Data;

namespace QueueTable {

    public class QueueService<T> where T : class {

        private dynamic CreateConnection() {
            return Database.OpenNamedConnection("QueueConnectionString");
        }

        public void Enqueue(QueueItem<T> item) {

            var db = CreateConnection();
            var trans = db.BeginTransaction();
            try {

                var metaData = trans.QueueMeta.Insert(
                   Title: item.Title
                );

                trans.QueueData.Insert(
                    QueueID: metaData.QueueID,
                    TextData: JsonConvert.SerializeObject(item.Data)
                );

                trans.Commit();
            }
            catch (Exception) {
                trans.Rollback();
            }
        }

        public QueueItem<T> Dequeue() {
            var db = CreateConnection();
            var results = db.DequeueFast(BatchSize: 1);
            return Parse(results).FirstOrDefault();
        }

        public IEnumerable<QueueItem<T>> Dequeue(int batch) {
            var db = CreateConnection();
            var results = db.DequeueFast(BatchSize: 100);
            return Parse(results);
        }

        private IEnumerable<QueueItem<T>> Parse(dynamic results) {
            foreach (var result in results) {
                yield return new QueueItemProxy<T>(result);
            }
        }
    }
}
