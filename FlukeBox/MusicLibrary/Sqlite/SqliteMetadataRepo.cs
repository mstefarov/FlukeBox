using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using FlukeBox.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace FlukeBox.MusicLibrary.Sqlite
{
    internal class SqliteMetadataRepo : IMetadataRepo
    {
        private const string ConnectionString = "Filename=./FlukeboxLibrary.db; Mode=ReadWriteCreate";
        private readonly ILogger _logger;

        public SqliteMetadataRepo(ILogger logger)
        {
            _logger = logger;
        }

        private static DbConnection Connect()
        {
            return new SqliteConnection(ConnectionString);
        }

        public TrackFull GetTrackFull(int id)
        {
            using (var connection = Connect())
            {
                const string query = "SELECT * FROM Tracks WHERE Id = @Id";
                connection.Open();
                return connection.QueryFirstOrDefault<TrackFull>(query, new
                {
                    Id = id
                });
            }
        }

        public async Task PrepareAsync()
        {
            using (DbConnection conn = Connect())
            {
                await conn.OpenAsync().ConfigureAwait(false);
                int currentVersion = ReadSchemaVersion(conn);
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    switch (currentVersion)
                    {
                        case 0:
                            _logger.LogWarning("No usable MusicLibrary file found. Creating a new one.");
                            SetUpInitial(conn);
                            break;

                        case 1:
                            break; // up to date

                        default:
                            _logger.LogWarning(
                                "Existing MusicLibrary file was created with a newer version of FlukeBox. " +
                                "Hopefully it's backwards-compatible!",
                                currentVersion);
                            break;
                    }
                    trans.Commit();
                }
            }
        }

        private static int ReadSchemaVersion(IDbConnection conn)
        {
            string gotName = conn.QueryFirstOrDefault<string>(
                "SELECT `name` FROM `sqlite_master` WHERE `type`='table' AND `name`='LibraryMetadata';");
            if (gotName != null)
            {
                return conn.QueryFirstOrDefault<int>("SELECT `SchemaVersion` FROM `LibraryMetadata`;");
            }
            return 0;
        }

        private static void SetUpInitial(IDbConnection conn)
        {
            conn.Execute(@"
DROP TABLE IF EXISTS `LibraryMetadata`;
CREATE TABLE `LibraryMetadata` (
	`SchemaVersion`	INTEGER NOT NULL,
	`DateCreated`	INTEGER NOT NULL,
	`DateModified`	INTEGER NOT NULL,
	`CreatedWith`	TEXT NOT NULL
);

DROP TABLE IF EXISTS `MusicFiles`;
CREATE TABLE `MusicFiles` (
	`Id`	            INTEGER NOT NULL,
	`FilePath`	        TEXT NOT NULL,
	`FileName`	        TEXT NOT NULL,
	`FileSize`	        INTEGER NOT NULL,
	`DateFileModified`	INTEGER NOT NULL,
	`DateAdded`	        INTEGER NOT NULL,
	`FormatCode`	    INTEGER,
	`Bitrate`	        INTEGER,
	`SamplingRate`	    INTEGER,
	`Year`	            INTEGER,
	`ReleaseArtist`	    TEXT,
	`ReleaseName`	    TEXT,
	`TrackNumber`	    INTEGER,
	`Artist`	        TEXT,
	`Title`	            TEXT,
	`Length`	        INTEGER,
	`Genre`	            TEXT,
	PRIMARY KEY(`Id`)
);

DROP TABLE IF EXISTS `Users`;
CREATE TABLE `Users` (
	`Name`	        TEXT NOT NULL UNIQUE,
	`PasswordSalt`	TEXT NOT NULL,
	`PasswordHash`	TEXT NOT NULL,
	`Note`	        TEXT,
	`CanInvite`	    INTEGER NOT NULL,
	`CanAdmin`	    INTEGER NOT NULL
);");

            long now = DateTime.UtcNow.Ticks;

            conn.Insert(new LibraryMetadata
            {
                SchemaVersion = 1,
                DateCreated = now,
                DateModified = now,
                CreatedWith = "FlukeBox " + Program.Version
            });
        }
    }
}