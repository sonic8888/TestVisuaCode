using System;

namespace TestVisualCode;

internal class ManagerEf
{
    public static void PrintMessage()
    {
        Console.WriteLine("Hello World!");
    }

    public static IEnumerable<string> GetDifferentTracksId()
    {
        var trackIdYandex = DB.GetTracksIdYandexDb();
        var trackIdOther = DB.GetTracksIdOtherDb();
        return trackIdYandex.Except(trackIdOther);
    }

    public static void AddTracksToOtherDb()
    {
        var tracksId = DB.GetTracksIdYandexDb();
        var tracks = DB.GetTracks(tracksId);
        using OC db = new();
        foreach (var track in tracks)
        {
            db.Tracks.Add(track);
        }
        db.SaveChanges();
    }

}
