using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;



namespace TestVisualCode
{
    internal class YandexMusic
    {
        /// <summary>
        /// PathYandexDir указывает путь к папке корневой Яндекс Музыка.
        /// </summary>
        public static string? PathYandexDir
        {
            get; private set;
        }
        /// <summary>
        /// PathMusicSours указывает путь к папке с треками приложения Яндекс Музыка.
        /// </summary>
        public static string? PathMusicSours { get; private set; }
        /// <summary>
        /// PathDBSqlite путь к БД Яндекс Музыка.
        /// </summary>
        public static string? PathDBSqlite { get; private set; }

        /// <summary>
        /// PathCopyTo путь к папке куда копируются треки.
        /// </summary>
        public static string PathCopyTo { get; set; } = "";
        /// <summary>
        /// PathCopyFrom путь к папке откуда добавляем файлы
        /// </summary>
        public static string PathCopyFrom { get; set; } = "";

        /// <summary>
        /// Data сегодняшняя дата.
        /// </summary>
        public static string Data { get; private set; }

        public static string PathMusicDirYandex { get; private set; }
        static YandexMusic()
        {

            PathYandexDir = GetPathYandexMusic();
            PathMusicSours = GetPathMusicSoursDir(PathYandexDir);
            PathDBSqlite = GetPathDbSqliteYandex(PathYandexDir);
            Data = DateTime.Now.ToString("d");
            PathMusicDirYandex = GetPathMusicSoursDir(PathYandexDir);


        }



        /// <summary>
        /// Возвращает путь к корневой папке Яндекс Музыка.
        /// </summary>
        /// <returns>путь к папке</returns>
        /// <exception cref="Exception"></exception>
        private static string? GetPathYandexMusic()
        {
            string user_dict = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

            string path1 = Path.Combine(user_dict, @"AppData\Local\Packages");
            string yandex = "Yandex.Music_";
            string path2 = "LocalState";
            string? _path_yandex_music = null;
            foreach (var dir in Directory.GetDirectories(path1))
            {
                if (dir.Contains(yandex))
                {
                    _path_yandex_music = Path.Combine(dir, path2);
                    break;
                }

            }
            if (_path_yandex_music == null)
                throw new Exception($"{path1}\\...{yandex}... not found.");
            else
                return _path_yandex_music;
        }


        /// <summary>
        /// Находит путь к папке с треками приложения Яндекс Музыка.
        /// </summary>
        /// <param name="path">Путь к корневой папке Яндекс Музыка</param>
        /// <returns>Путь к папке с треками</returns>
        /// <exception cref="Exception">Если путь не найден.</exception>
        private static string GetPathMusicSoursDir(string? path)
        {
            string music = "Music";
            string path_music_files = "";
            if (path != null)
            {
                path_music_files = Path.Combine(path, music);
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    if (file.EndsWith(".sqlite"))
                    {
                        FileInfo fileDB = new FileInfo(file);
                        string name = fileDB.Name;
                        name = name[..^7];
                        name = name.Substring(8);
                        path_music_files = Path.Combine(path_music_files, name);
                    }
                }

            }
            return path_music_files;
        }

        /// <summary>
        /// Находит путь к БД Яндекс Музыка.
        /// </summary>
        /// <param name="path">Путь к корневой папке Яндекс Музыка</param>
        /// <returns> Путь к БД</returns>
        /// <exception cref="Exception">Если БД не нашли.</exception>
        private static string? GetPathDbSqliteYandex(string? path)
        {
            string? path_db = null;
            if (path != null)
                foreach (var file in Directory.GetFiles(path))
                {
                    if (new FileInfo(file).Extension.ToLower() == ".sqlite")
                    {
                        path_db = file;
                        break;
                    }
                }
            if (path_db == null)
            {
                throw new Exception($"{path}\\..DB.sqlite  not found.");
            }
            else
            {
                return path_db;
            }

        }





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
                    var result = db.Read(Tools.Sql_queries["SelectTrackIdYandex"], Tools.MyFu, SqliteDb.GetSqliteParameters(new (string, string?)[] { ("@value", Tools.Kind) }));
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
        private static bool IsExist(string pathDir, string file_name, out string extension, out string path)
        {
            extension = "";
            path = "";
            bool is_exist = false;
            DirectoryInfo dir = new DirectoryInfo(pathDir);
            var files = dir.GetFiles();
            foreach (var item in files)
            {
                if (item.Name.StartsWith(file_name))
                {
                    is_exist = true;
                    extension = item.Extension;
                    path = item.FullName;
                    break;
                }
            }
            return is_exist;
        }

        /// <summary>
        /// Получаем треки из Яндекс БД.
        /// </summary>
        /// <param name="trackId">trackId трека</param>
        /// <param name="data_sours">Полное имя Яндекс БД</param>
        /// <param name="path_dir">Путь к папке в Яндекс Музыка где находятся аудио_файлы</param>
        /// <returns>Список траков</returns>
        public static List<Track> GetTracks(List<string> trackId, string data_sours, string path_dir)
        {
            var track_list = new List<Track>();
            SqliteDb? db = null;
            try
            {
                db = new SqliteDb(data_sours);
                if (db.Connection != null)
                {
                    foreach (var item in trackId)
                    {
                        try
                        {
                            var track = new Track(item);
                            var title = db.Read(Tools.Sql_queries["SelectTitle"], Tools.MyFu, SqliteDb.GetSqliteParameters(new (string, string?)[] { ("@value", item) }));
                            string name_track = title.FirstOrDefault(new[] { "unknown" })[0];
                            if (Tools.isNormalize(name_track))
                            {
                                name_track = Tools.Normalize(name_track);
                            }
                            track.Name = name_track;
                            track.Title = name_track;
                            var albumId = db.Read(Tools.Sql_queries["SelectAlbumId"], Tools.MyFu, SqliteDb.GetSqliteParameters(new (string, string?)[] { ("@value", item) }));
                            track.AlbumId = albumId.FirstOrDefault(new[] { "unknown" })[0];
                            if (track.AlbumId != "unknown")
                            {
                                var albumTitleYearArtist = db.Read(Tools.Sql_queries["SelectAlbumTitleYearArtist"], Tools.MyFu, SqliteDb.GetSqliteParameters(new (string, string?)[] { ("@value", track.AlbumId) }), 3);
                                track.Album = albumTitleYearArtist.FirstOrDefault(new[] { "unknown" })[0];
                                track.Year = albumTitleYearArtist.FirstOrDefault(new[] { "", "unknown" })[1];
                                track.Artist = albumTitleYearArtist.FirstOrDefault(new[] { "", "", null })[2];
                                if (YandexMusic.IsExist(path_dir, track.TrackId, out string extension, out string path))
                                {
                                    track.Extension = extension;
                                    track.IsExist = true;
                                    track.PathSours = path;
                                }
                                track.DataCreate = Track.Data();
                                track_list.Add(track);
                            }
                        }
                        catch (System.Exception ex)
                        {

                            Tools.DisplayColor(ex.Message, ConsoleColor.Red);
                            Tools.DisplayColor($"TrackId:{item}", ConsoleColor.Red);
                            continue;
                        }
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
            return track_list;
        }
    }
}

