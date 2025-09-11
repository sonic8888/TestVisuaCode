using System;
using System.Data.Common;
using System.Security.Authentication.ExtendedProtection;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

namespace TestVisualCode;

internal class Tools
{
    internal static string Kind = "5";
    internal static string? PathDirDestination = @"D:\test";
    internal static string? PathDirOther;
    private static string Pattern = @"[\*\|\\\:\""<>\?\/]";
    private static string Target = ".";
    // private static Regex regex = new Regex(Tools.Pattern);
    internal static Dictionary<string, string> Sql_queries = new Dictionary<string, string>
    {
      {"SelectTrackIdYandex", "SELECT TrackId FROM T_PlaylistTrack WHERE Kind = @value;" },
      {"SelectTrackIdDBDestination", "SELECT  TrackId FROM T_Track_Yandex WHERE Sours = @value" },
      { "SelectTitle", "SELECT Title FROM T_Track WHERE Id = @value" },
      {"SelectAlbumId", "SELECT AlbumId FROM T_TrackAlbum WHERE TrackId = @value;"},
      {"SelectAlbumTitleYearArtist", "SELECT Title, Year, ArtistsString FROM T_Album WHERE Id = @value" },
      {"CreateTables", "CREATE TABLE T_Track_Yandex (Id  INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE  NOT NULL, Name VARCHAR, Title  VARCHAR, Artist  VARCHAR, Album VARCHAR, Year VARCHAR, TrackId  VARCHAR,  Data  VARCHAR , Sours VARCHAR DEFAULT ('Yandex'));"},
      {"InsertTrack","INSERT INTO T_Track_Yandex (Name, Title, Artist,Album, Year, TrackId, Data)  VALUES (@name, @title, @artist,@album, @year, @track_id, @data)" },
      {"DeleteTrack", "DELETE FROM T_Track_Yandex WHERE TrackId = @value" }
    };

    internal static string[] MyFu(SqliteDataReader reader, int n)
    {
        string[] arr = new string[n];
        for (int i = 0; i < n; i++)
        {
            if (!reader.IsDBNull(i)) arr[i] = reader.GetString(i);
        }
        return arr;
    }

    /// <summary>
    /// Выводит на консоль текст разными цветами.
    /// </summary>
    /// <param name="message">Текст</param>
    /// <param name="color">ConsoleColor color</param>
    public static void DisplayColor(string message, System.ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }


    public static bool Copy(Track track, string dirDestination)
    {
        bool isSuccessful = true;
        try
        {
            if (!Directory.Exists(dirDestination)) throw new ArgumentException($"папка:{dirDestination} не найдена.");

            track = Rename(track, dirDestination);
            File.Copy(track.PathSours!, Path.Combine(dirDestination, track.Name));
        }
        catch (System.Exception ex)
        {
            isSuccessful = false;
            Tools.DisplayColor(ex.Message, ConsoleColor.Red);
        }
        return isSuccessful;
    }



    public static bool isNormalize(string text)
    {

        if (Regex.IsMatch(text, Tools.Pattern)) { return true; } else { return false; }
    }

    public static string Normalize(string text)
    {
        return Regex.Replace(text, Tools.Pattern, Tools.Target);
    }

    public static Track Rename(Track track, string pathDir)
    {
        string text = track.Artist ?? track.TrackId;
        if (Tools.isNormalize(text))
        {
            text = Tools.Normalize(text);
        }
        string _name = $"{track.Name}({text}){track.Extension}";
        int n = 0;
        while (File.Exists(Path.Combine(pathDir, _name)))
        {
            if (n == 0)
            {
                _name = $"{track.Name}({track.Artist + "(" + track.Album + ")"}){track.Extension}";
            }
            else
            {
                _name = $"{track.Name}({track.Artist + "-" + track.TrackId}){track.Extension}";

            }
            n++;

        }
        track.Name = _name;
        return track;
    }
    public static bool IsAudio(FileInfo file)
    {
        if (file == null) return false;
        if (file.Extension.ToLower() == ".mp3" || file.Extension.ToLower() == ".flack" || file.Extension.ToLower() == ".wav") { return true; } else return false;
    }

    // public static SqliteDb? GetDB()
    // {
    //     if (Directory.Exists(PathDirDestination))
    //     {
    //         if (File.Exists(Path.Combine(PathDirDestination, SqliteDb.nameDb)))
    //         {
    //             return new SqliteDb(Path.Combine(PathDirDestination, SqliteDb.nameDb));
    //         }
    //         else
    //         {
    //             return SqliteDb.CreateDB(PathDirDestination);
    //         }
    //     }
    //     else
    //     {
    //         throw new ArgumentException($"Папка назначения:{PathDirDestination} не найдена.");
    //     }

    // }

    public static List<string> GetTrackId(string? data_sours)
    {
        if (data_sours == null) throw new ArgumentException($"БД:{data_sours} не обнаружена.");
        var trackId = new List<string>();
        SqliteDb? db = null;
        try
        {
            db = new SqliteDb(data_sours);
            if (db.Connection != null)
            {
                var result = db.Read(Tools.Sql_queries["SelectTrackIdDBDestination"], Tools.MyFu, SqliteDb.GetSqliteParameters(new[] { ("@value", "Yandex") }));
                foreach (var item in result)
                {
                    trackId.Add(item[0]);
                }
            }
        }
        catch (System.Exception ex)
        {

            Tools.DisplayColor(ex.Message, ConsoleColor.Red);
        }
        finally
        {
            db?.Dispose();
        }
        return trackId;
    }


}
