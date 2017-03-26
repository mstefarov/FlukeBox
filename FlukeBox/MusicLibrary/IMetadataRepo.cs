using System.Threading.Tasks;
using FlukeBox.Models;

namespace FlukeBox.MusicLibrary
{
    public interface IMetadataRepo
    {
        TrackFull GetTrackFull(int id);

        Task PrepareAsync();
    }
}
