using System.Data;
using Dapper;
using FlukeBox.Models;
using Microsoft.Data.Sqlite;

namespace FlukeBox.MediaLibrary {
    internal class SqliteMetadataRepo : IMetadataRepo {
        private const string ConnectionString = "Filename=./FlukeboxLibrary.db";

        private static IDbConnection GetConnection() {
            return new SqliteConnection(ConnectionString);
        }

        public TrackFull GetTrackFull(int id)
        {
            using (var connection = GetConnection())
            {
                const string query = "SELECT * FROM Tracks WHERE Id = @Id";
                connection.Open();
                return connection.QueryFirstOrDefault<TrackFull>(query, new
                {
                    Id = id
                });
            }
        }
    }
}
