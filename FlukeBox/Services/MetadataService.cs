using System;
using System.IO;
using FlukeBox.Models;
using TagLib;
using File = TagLib.File;

namespace FlukeBox.Services {
    public class MetadataService {
        public TrackFull ReadTrack(string id, string path) {
            using (File file = File.Create(new File.LocalFileAbstraction(path))) {

                MediaFormat mf = IdentifyFormat(file);
                if (mf == MediaFormat.Unknown) return null;

                Tag tag = file.Tag;
                return new TrackFull {
                    ID = 123,
                    TrackNumber = tag.Track,
                    FileName = Path.GetFileName(path),
                    Title = tag.Title,
                    Artist = tag.FirstPerformer,
                    Year = tag.Year,
                    Album = tag.Album,
                    AlbumArtist = tag.FirstAlbumArtist,
                    Genre = tag.FirstGenre,
                    Length = (long)Math.Round(file.Properties.Duration.TotalSeconds),
                    MediaFormat = mf,
                    Bitrate = file.Properties.AudioBitrate
                };
            }
        }

        private MediaFormat IdentifyFormat(File file) {
            if (file is TagLib.Mpeg.AudioFile) {
                return MediaFormat.MP3;
            } else if (file is TagLib.Flac.File) {
                return MediaFormat.FLAC;
            } else if (file is TagLib.Ogg.File) {
                return MediaFormat.OggVorbis;
            } else if (file is TagLib.Riff.File) {
                return MediaFormat.Wave;
            } else {
                return MediaFormat.Unknown;
            }
        }
    }
}
