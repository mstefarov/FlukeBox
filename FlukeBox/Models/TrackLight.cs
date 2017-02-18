using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlukeBox.Models
{
    public class TrackLight
    {
        public long ID { get; set; }
        public long ParentID { get; set; }

        public string Album { get; set; }
        public uint TrackNumber { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }

        public long Length { get; set; }
    }
}
