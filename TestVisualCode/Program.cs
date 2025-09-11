
using Microsoft.Data.Sqlite;
using TestVisualCode;

internal class Program
{
    // static Dictionary<string, string> dic = new Dictionary<string, string>() {
    //                         {"str1","SELECT Count(TrackId) FROM T_PlaylistTrack WHERE Kind = @value;" },
    //         {"str2", "SELECT TrackId FROM T_PlaylistTrack WHERE Kind = @value;" },
    //         {"str10", "SELECT AlbumId FROM T_TrackAlbum WHERE TrackId = @value;" },
    //         {"str3", "SELECT Title FROM T_Track WHERE Id = @value" },
    //         {"str4", "SELECT ArtistId FROM T_TrackArtist WHERE TrackId = @value" },
    //         {"str5", "SELECT Name FROM T_Artist WHERE Id = @value" },
    //         {"str6", "SELECT Count(Name) From T_Trask_Yandex" },
    //         {"str7", "SELECT * FROM  T_Trask_Yandex " },
    //         {"str8", "SELECT Title, Year, ArtistsString FROM T_Album WHERE Id = @value" },
    //         //{"str9", "SELECT Year FROM T_Album WHERE Id = @value" },
    //         {"str_create", "CREATE TABLE T_Trask_Yandex (Id  INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE  NOT NULL, Title  VARCHAR, Artist  VARCHAR, Album VARCHAR, Year VARCHAR, TrackId  VARCHAR, ArtistId  VARCHAR, NameArtist   VARCHAR, Data  VARCHAR , Sours VARCHAR DEFAULT ('Yandex'));" },
    //         {"str_insert","INSERT INTO T_Trask_Yandex (Title, Artist,Album, Year, TrackId, ArtistId, NameArtist, Data)  VALUES (@title, @artist,@album, @year, @track_id, @artist_id, @name_artist, @data)" },
    //          { "str12", "SELECT  TrackId FROM T_Trask_Yandex WHERE Sours = @value" },
    //          { "str13", "SELECT TrackId FROM T_Trask_Yandex" }
    //         };

    static string data_sours = @"C:\Users\sonic\OneDrive\Рабочий стол\musicdb_ca3bd06f5f9004c4044dff0c57e4c09d.sqlite";
    private static void Main(string[] args)
    {
        var ym = new YandexMusic();
        var track = YandexMusic.GetTrackId(YandexMusic.PathDBSqlite!);
        var track_track = YandexMusic.GetTracks(track,YandexMusic.PathDBSqlite!,YandexMusic.PathMusicSours!);
        foreach (var item in track_track)
        {
            System.Console.WriteLine(item);
            if (!Tools.Copy(item, @"D:\ntest"))
            {
                Tools.DisplayColor(item.Title, ConsoleColor.Blue);
            }
        }
    }
 


}