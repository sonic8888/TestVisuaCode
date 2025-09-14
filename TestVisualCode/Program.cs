
using Microsoft.Data.Sqlite;
using TagLib;
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
    Manager.AddFilesFromOtherDir();
  }

  private static void TestTags(Track track, FileInfo file)
  {
    var t_file = TagLib.File.Create(file.FullName);
    string title = t_file.Tag.Title ?? "unknown";
    string? year = t_file.Tag.Year > 0 ? t_file.Tag.Year.ToString() : "unknown";
    string? album = t_file.Tag.Album ?? "unknown";
    string? artist = t_file.Tag.Performers.Length > 0 ? t_file.Tag.Performers[0] : "unknown";
    track.Name = file.Name;
    track.Title = title;
    track.Year = year;
    track.Album = album;
    track.Artist = artist;
    // System.Console.WriteLine(artist[0]);


  }

  private static void TestSqlite()
  {
    // string Sql_queries = "CREATE TABLE Test (Id  INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE  NOT NULL, Title  VARCHAR, Artist  VARCHAR, Album VARCHAR, Year VARCHAR);";
    // string pathDir = @"D:\testDb";
    string Sql_queries = "INSERT INTO Test (Title, Artist, Album, Year) VALUES (@title, @artist, @album, @year)";
    string dataSours = @"D:\testDb\my_music.sqlite";
    var my_params = new List<(string, string?)>([("title", "Скорый поезд"), ("artist", null), ("album", "малолетка"), ("year", "2024")]);
    SqliteDb? db = new SqliteDb(dataSours);
    // db.Write(Sql_queries, my_params);
    var connection = db.Connection;
    Write(Sql_queries, connection!, my_params);
    db.Dispose();
  }

  private static List<SqliteParameter> GetSqliteParameters(IEnumerable<(string, string?)> parameters)
  {
    List<SqliteParameter> sqliteParameters = new List<SqliteParameter>();
    if (parameters != null)
    {
      foreach (var item in parameters)
      {
        if (item.Item1 != null)
        {
          sqliteParameters.Add(new SqliteParameter(item.Item1, item.Item2));
        }
      }
    }
    return sqliteParameters;
  }

  public static int Write(string sql, SqliteConnection connection, IEnumerable<(string, string?)>? sqliteParameters = null)
  {
    int rows = -1;
    try
    {
      connection?.Open();
      SqliteCommand command = new SqliteCommand(sql, connection);
      if (sqliteParameters != null)
      {
        foreach (var item in sqliteParameters)
        {
          object? value = item.Item2;
          if (value == null) value = DBNull.Value;
          command.Parameters.AddWithValue(item.Item1, value);
        }
      }
      rows = command.ExecuteNonQuery();
    }
    catch (System.Exception ex)
    {

      Tools.DisplayColor(ex.Message, ConsoleColor.Red);
    }
    finally
    {
      connection?.Close();
    }
    return rows;
  }




}