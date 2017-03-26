using System.ComponentModel.DataAnnotations.Schema;

namespace FlukeBox.MusicLibrary.Sqlite
{
    [Table("LibraryMetadata")]
    internal class LibraryMetadata
    {
        public int SchemaVersion { get; set; }
        public long DateCreated { get; set; }
        public long DateModified { get; set; }
        public string CreatedWith { get; set; }
    }
}