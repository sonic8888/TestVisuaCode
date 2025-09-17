using System;
using System.Data.Common;
using System.Security.Authentication.ExtendedProtection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace TestVisualCode
{
    internal static class Manager
    {
        static string? action = "";

        public static async Task PrintMessage()
        {
            Console.WriteLine("Программа работы  с аудиофайлами Яндекс Музыка");
            while (true)
            {
                Console.WriteLine("Укажите путь к папке назначения:");
                var path = Console.ReadLine();
                if (path != null) Tools.PathDirDestination = path;
                if (!Path.Exists(Tools.PathDirDestination))
                {
                    Console.WriteLine("Такого пути не существует.");
                }
                else
                {
                    break;
                }
            }
            bool isWork = true;
            while (isWork)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("y - копирование файлов из Яндекс Музыка в указанную папку.");
                Console.WriteLine("d - добавление файлов из выбранной папки в  указанную папку");
                Console.WriteLine("s - перемешать файлы");
                Console.WriteLine("e - для завершения работы");
                action = Console.ReadLine();
                switch (action)
                {
                    case "y":
                        Manager.AddFilesFromYandexToDirDestination();
                        break;
                    case "d":
                        while (true)
                        {
                            Console.WriteLine("Укажите путь к папке c файлами для добавления:");
                            var path = Console.ReadLine();
                            if (path != null) Tools.PathDirOther = path;
                            if (Path.Exists(Tools.PathDirOther))
                            {
                                break;
                            }
                            else
                                Console.WriteLine("Такого пути не существует.");
                        }
                        Manager.AddFilesFromOtherDir();
                        break;
                    case "e":
                        isWork = false;
                        break;

                    case "s":
                        while (true)
                        {
                            Console.WriteLine("Укажите путь к папке или диску c файлами:");
                            var path = Console.ReadLine();
                            if (path != null) Tools.PathDir = path;
                            if (Path.Exists(Tools.PathDir))
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Такого пути не существует.");
                            }
                        }
                        Task moveFiles = Tools.ShuffleFiles();
                        moveFiles.Wait();
                        break;
                    default:
                        Console.WriteLine("действие выбрано не корректно:");
                        break;
                }
            }
        }




        /// <summary>
        /// Создаем теги аудиофайла скопированного  в папку(pathDestination) из данных извлеченных из объекта класса "Track". 
        /// </summary>
        /// <param name="track">Объект класса "Track".</param>
        /// <param name="path_dir">Путь к папке с аудиофайлами(pathDestination )</param>
        private static void CreateTags(Track track, string path_dir)
        {
            try
            {
                var t_file = TagLib.File.Create(Path.Combine(path_dir, track.Name));
                t_file.Tag.Title = track.Title;
                t_file.Tag.Album = track.Album ?? "unknown";
                if (track.Year != null)
                {
                    try
                    {
                        uint year = Convert.ToUInt32(track.Year);
                        t_file.Tag.Year = year;
                    }
                    catch (Exception)
                    {
                    }
                }

                t_file.Tag.Performers = new string[] { track.Artist ?? "unknown" };
                t_file.Save();
            }
            catch (Exception)
            {

                throw;
            }
        }
        internal static void AddFilesFromYandexToDirDestination()
        {
            var tracksIdDifferent = GetTracksIdDifferent();
            string sours_db = SqliteDb.GetNameDbDefault(Tools.PathDirDestination!);
            var dbDestination = new SqliteDb(sours_db);
            try
            {
                var tracks = YandexMusic.GetTracks(tracksIdDifferent, YandexMusic.PathDBSqlite!, YandexMusic.PathMusicSours!);
                var list_error = new List<string>();
                foreach (var track in tracks)
                {
                    var track_rename = Tools.Rename(track, Tools.PathDirDestination!);
                    var param = SqliteDb.GetSqliteParameters(new (string, string?)[]{("@name",track_rename.Name),("@title",track_rename.Title),
                   ("@artist",track_rename.Artist),("@album",track_rename.Album),("@year",track_rename.Year), ("@track_id",track_rename.TrackId),("@data",Track.Data()),});
                    var rows = dbDestination.Write(Tools.Sql_queries["InsertTrack"], param);
                    if (rows == -1) continue;
                    if (!Tools.Copy(track_rename, Tools.PathDirDestination!))
                    {
                        var param_delete = SqliteDb.GetSqliteParameters(new (string, string?)[] { ("@value", track_rename.TrackId) });
                        dbDestination.Write(Tools.Sql_queries["DeleteTrack"], param_delete);
                    }
                    else
                    {
                        CreateTags(track_rename, Tools.PathDirDestination!);
                    }

                    Tools.DisplayColor(track_rename.Name, ConsoleColor.Green);
                }
            }
            finally
            {
                dbDestination.Dispose();
            }
        }

        internal static List<string> GetTracksIdDifferent()
        {
            if (Tools.PathDirDestination == null) throw new ArgumentException("Путь к папке назначения не задан.");
            var tracksIdDifferent = new List<string>();
            var tracksIdDBYandex = YandexMusic.GetTrackId(YandexMusic.PathDBSqlite);
            if (File.Exists(Path.Combine(Tools.PathDirDestination, SqliteDb.nameDb)))
            {
                string soursDbDestination = SqliteDb.GetNameDbDefault(Tools.PathDirDestination);
                var tracksIdDBDestination = Tools.GetTrackIds(soursDbDestination);
                tracksIdDifferent = tracksIdDBYandex.Except(tracksIdDBDestination).ToList();
            }
            else
            {
                SqliteDb.CreateDB(Tools.PathDirDestination);
                tracksIdDifferent = tracksIdDBYandex;
            }
            return tracksIdDifferent;
        }

        internal static void AddFilesFromOtherDir()
        {
            if (!Directory.Exists(Tools.PathDirOther)) throw new ArgumentException($"Папка:{Tools.PathDirOther} не найдена.");
            if (!File.Exists(Path.Combine(Tools.PathDirDestination!, SqliteDb.nameDb))) throw new ArgumentException($"БД:{Tools.PathDirDestination + SqliteDb.nameDb} не найдена.");
            var files = new DirectoryInfo(Tools.PathDirOther!).GetFiles();
            foreach (var item in files)
            {
                if (Tools.IsAudio(item))
                {
                    var track = Tools.GetTrackFromFile(item);
                    if (track != null)
                    {
                        if (InsertDB(track))
                        {
                            if (Tools.Copy(track, Tools.PathDirDestination!))
                            {
                                CreateTags(track, Tools.PathDirDestination!);
                                Tools.DisplayColor(track.Name, ConsoleColor.Green);
                            }
                            else
                            {
                                SqliteDb? db = null;
                                try
                                {
                                    db = new SqliteDb(SqliteDb.GetNameDbDefault(Tools.PathDirDestination!));
                                    var param_delete = SqliteDb.GetSqliteParameters(new (string, string?)[] { ("@value", track.TrackId) });
                                    db.Write(Tools.Sql_queries["DeleteTrack"], param_delete);
                                }
                                catch (System.Exception ex)
                                {
                                    Tools.DisplayColor(ex.Message, ConsoleColor.Red, false);
                                    Tools.DisplayColor(track.ToString(), ConsoleColor.Blue);
                                }
                                finally
                                {
                                    db?.Dispose();
                                }
                            }
                        }

                    }

                }
            }

        }

        private static bool InsertDB(Track track)
        {
            bool isSuccessful = false;
            SqliteDb? db = null;
            try
            {
                db = new SqliteDb(SqliteDb.GetNameDbDefault(Tools.PathDirDestination!));
                var parameters = new List<(string, string?)>() { ("@name", track.Name), ("@title", track.Title), ("@artist", track.Artist), ("@album", track.Album), ("@year", track.Year), ("@track_id", track.TrackId), ("@data", Track.Data()), ("@sours", "Other") };
                var pows = db.WriteWithoutNull(Tools.Sql_queries["InsertTrackFromOther"], parameters);
                isSuccessful = true;
            }
            catch (System.Exception ex)
            {
                Tools.DisplayColor(ex.Message, ConsoleColor.Red, false);
                Tools.DisplayColor(track.ToString(), ConsoleColor.Blue);
            }
            finally
            {
                db?.Dispose();
            }
            return isSuccessful;
        }
    }
}

