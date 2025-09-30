using System;

namespace TestVisualCode;

internal class SqliteDbEf
{
    internal static List<string> GetTracksIdYandexDb()
    {
        List<string> trackId = new();
        try
        {
            using YC db = new();
            var tracks = from track in db.TPlaylistTracks where track.Kind == ToolsEf.Kind select track.TrackId;
            trackId = [.. tracks];

        }
        catch (System.Exception ex)
        {
            T.DisplayColor(ex.Message);
        }
        return trackId;
    }

    internal static IEnumerable<Track> GetTracks(IEnumerable<string> enumerable)
    {
        List<Track> tracks = new();
        using YC db = new();
        try
        {
            foreach (var id in enumerable)
            {
                if (!Path.Exists(Path.Combine(YandexMusic.PathMusicSours!, id + ".mp3")))
                    continue;
                var title = (from t in db.TTracks where t.Id == id select t.Title).First();
                var albumId = (from t in db.TTrackAlbums where t.TrackId == id select t.AlbumId).First();
                var albumTitle = (from t in db.TAlbums where t.Id == albumId select t.Title).FirstOrDefault();
                var albumYear = (from t in db.TAlbums where t.Id == albumId select t.Year).FirstOrDefault();
                var albumArtist = (from t in db.TAlbums where t.Id == albumId select t.ArtistsString).FirstOrDefault();
                string name = ToolsEf.isNormalize(title) ? ToolsEf.Normalize(title) : title;
                name = $"{name}({albumArtist}-{albumTitle})";
                var track = new Track(id) { Title = title, Name = name, AlbumId = albumId, Year = albumYear, Artist = albumArtist, Album = albumTitle, PathSours = Path.Combine(YandexMusic.PathMusicSours!, id + ".mp3"), SourceFail = $"{ToolsEf.yandex}" };
                // System.Console.WriteLine(track);
                tracks.Add(track);

            }

        }
        catch (System.Exception ex)
        {

            throw new Exception(ex.Message);
        }
        return tracks;
    }
    internal static List<string> GetTracksIdOtherDb()
    {
        List<string> trackId = new();
        try
        {
            using OC db = new();
            var tracks = from track in db.Tracks where track.SourceFail == ToolsEf.yandex select track.TrackId;
            trackId = [.. tracks];

        }
        catch (System.Exception ex)
        {

            T.DisplayColor(ex.Message);
        }
        return trackId;
    }
}

