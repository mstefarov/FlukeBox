namespace FlukeBox.Models
{
    public class TrackLight
    {
        public long Id { get; set; }
        public long ParentId { get; set; }

        public string Album { get; set; }
        public uint TrackNumber { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }

        public long Length { get; set; }
    }
}
