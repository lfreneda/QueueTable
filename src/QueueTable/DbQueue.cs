using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;

namespace QueueTable
{
    public class DbQueue<T> where T : class
    {
        private readonly string _connectionString;

        public DbQueue(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Enqueue(QueueItem<T> item)
        {
            ExecuteNonQuery(@"insert into QueueMeta (Title) values (@Title)
                              insert into QueueData (QueueID, TextData) values (SCOPE_IDENTITY(),@TextData)",
            new[]
            {
                new SqlParameter("@Title", SqlDbType.VarChar) {Value = item.Title},
                new SqlParameter("@TextData", SqlDbType.VarChar) {Value = JsonConvert.SerializeObject(item.Data) },
            });
        }

        public void UpdateItem(QueueItem<T> item)
        {
            ExecuteNonQuery(@"update QueueMeta set [Status] = @Status where id = @Id",

            new[]
            {
                new SqlParameter("@Status", SqlDbType.Int) { Value = (int) item.Status },
                new SqlParameter("@Id", SqlDbType.Int) { Value = item.Id },
            });
        }

        public QueueItem<T> Dequeue()
        {
            return Dequeue(1).FirstOrDefault();
        }

        public IEnumerable<QueueItem<T>> Dequeue(int batch)
        {
            return DequeueFast(batch);
        }

        private SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        private IEnumerable<QueueItem<T>> DequeueFast(int count)
        {
            using (var connection = CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "dbo.DequeueFast";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@BatchSize", count);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new QueueItemProxy<T>(
                                id: reader.GetInt32(reader.GetOrdinal("Id")),
                                queueDateTime: reader.GetDateTime(reader.GetOrdinal("QueueDateTime")),
                                title: reader.GetString(reader.GetOrdinal("Title")),
                                status: reader.GetInt32(reader.GetOrdinal("Status")),
                                textData: reader.GetString(reader.GetOrdinal("Data"))
                            );
                        }
                    }
                }
            }
        }

        private void ExecuteNonQuery(string commandText, SqlParameter[] parameters)
        {
            using (var connection = CreateConnection())
            {
                SqlTransaction transaction = null;

                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    using (var command = new SqlCommand(commandText, connection, transaction))
                    {
                        command.Parameters.AddRange(parameters);
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction?.Rollback();
                }
            }
        }
    }
}
