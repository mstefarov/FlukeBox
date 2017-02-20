using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlukeBox.Models;

namespace FlukeBox.MediaLibrary
{
    public interface IMetadataRepo
    {
        TrackFull GetTrackFull(int id);
    }
}
