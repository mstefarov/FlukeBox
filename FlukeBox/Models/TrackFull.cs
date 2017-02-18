using System.ComponentModel.DataAnnotations;

namespace FlukeBox.Models {
    public class TrackFull : TrackLight{
        public long RootID { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long LastModified { get; set; }

        public string Genre { get; set; }
        public string AlbumArtist { get; set; }
        public uint Year { get; set; }
        public MediaFormat MediaFormat { get; set; }

        public int Bitrate { get; set; }
        public long FileSize { get; set; }
    }
}
