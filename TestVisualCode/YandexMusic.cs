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

        /// <summary>
        /// Комбинирует имя трека и имя артиста.  
        /// </summary>
        /// <param name="track">класс Track</param>
        /// <returns>имя трека + (артист)</returns>
        private static string GetName(Track track) => $"{track.Name}{track.Extension}";

        /// <summary>
        /// Если файл с таким именем уже сужествует то 
        /// к имени файла добавляем: (n).
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        internal static Action<Track> RenameCopy(Track track)
        {
            int number = 1;
            void CreateCopyName(Track track)
            {
                string copy = "(" + $"{number}" + ")";
                string copy_1 = "(" + $"{number - 1}" + ")";
                if (track.Name.EndsWith(copy_1))
                {
                    string name = track.Name;
                    name = name.Replace(copy_1, "");
                    track.Name = name;
                }
                track.Name = track.Name + copy;
                number++;
            }
            return CreateCopyName;
        }


        /// <summary>
        /// Копирует трек из папки источника(Яндекс Музыка) в папку назначения.
        /// </summary>
        /// <param name="track">AudioFilesWorkC_.Track</param>
        /// <param name="sours">папка источник</param>
        /// <param name="destination">папка назначение</param>
        /// <param name="isException">false если было исключение</param>
        /// <param name="isRename">изменяем имя трека или нет</param>
        /// <param name="isOverwrite">true если файл надо перезаписывать</param>
        /// <returns>System.IO.FileInfo скопированного трека</returns>
        /// <exception cref="ArgumentException">если путь к папке назначения не существует</exception>
        // public static FileInfo CopyTo(Track track, string? sours, string destination, out bool isException, bool isOverwrite = true)
        // {
        //     isException = true;
        //     if (!Path.Exists(destination)) throw new ArgumentException($"Path:{destination} - There is no such way.");
        //     string _sours = Path.Combine(sours!, track.TrackId + track.Extension);
        //     string _destination = Path.Combine(destination, GetName(track));
        //     FileInfo file_destination = new FileInfo(_destination);
        //     if (file_destination.Exists)
        //     {
        //         var rename = RenameCopy(track);
        //         do
        //         {
        //             rename(track);
        //             _destination = Path.Combine(destination, GetName(track));
        //             file_destination = new FileInfo(_destination);

        //         }
        //         while (file_destination.Exists);
        //     }
        //     FileInfo file_sours = new FileInfo(_sours);
        //     try
        //     {
        //         if (file_sours.Exists)
        //         { file_sours = file_sours.CopyTo(_destination, isOverwrite); }
        //         else
        //             throw new ArgumentException($"Path:{_sours} - не найден");
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(track);
        //         Manager.DisplayColor(ex.Message, ConsoleColor.Magenta);
        //         isException = false;
        //     }
        //     return file_sours;

        // }


        //public static FileInfo CopyFromTo(Track track, string? sours, string destination, out bool isException, bool isRename = true, bool isOverwrite = true)
        //{
        //    isException = true;
        //    if (!Path.Exists(destination)) throw new ArgumentException($"Path:{destination} - There is no such way.");
        //    string _sours = Path.Combine(sours!, track.Name + track.Extension);
        //    string _destination = "";
        //    if (isRename)
        //        _destination = Path.Combine(destination, GetName(track));
        //    else
        //        _destination = Path.Combine(destination, track.Name + track.Extension);
        //    FileInfo file = new FileInfo(_sours);

        //    try
        //    {
        //        if (file.Exists)
        //        { file = file.CopyTo(_destination, isOverwrite); }
        //        else
        //            throw new ArgumentException($"Path:{_destination} - there is no such file");
        //    }
        //    catch (IOException)
        //    {
        //        if (isOverwrite)
        //        {
        //            Manager.DisplayColor($"Трек '{track}' уже существует. Перезаписываем его.", ConsoleColor.Green);
        //        }
        //        else { Manager.DisplayColor($"Трек '{track}' уже существует. Не перезаписываем его.", ConsoleColor.Green); }

        //        isException = false;
        //    }
        //    catch (Exception ex)
        //    {

        //        Manager.DisplayColor(ex.Message, ConsoleColor.Blue);
        //        isException = false;
        //    }
        //    return file;

        //}

        public static List<string> GetTrackId(string data_sours)
        {
            var trackId = new List<string>();
            SqliteDb? db = null;
            try
            {
                db = new SqliteDb(data_sours);
                if (db.Connection != null)
                {
                    var result = db.Read(Tools.Sql_queries["SelectTrackId"], Tools.MyFu, SqliteDb.GetSqliteParameters(new[] { ("@value", Tools.Kind) }));
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
                            var title = db.Read(Tools.Sql_queries["SelectTitle"], Tools.MyFu, SqliteDb.GetSqliteParameters(new[] { ("@value", item) }));
                            string name_track = title.FirstOrDefault(new[] { "unknown" })[0];
                            if (Tools.isNormalize(name_track))
                            {
                                name_track = Tools.Normalize(name_track);
                            }
                            track.Name = name_track;
                            track.Title = name_track;
                            var albumId = db.Read(Tools.Sql_queries["SelectAlbumId"], Tools.MyFu, SqliteDb.GetSqliteParameters(new[] { ("@value", item) }));
                            track.AlbumId = albumId.FirstOrDefault(new[] { "unknown" })[0];
                            if (track.AlbumId != "unknown")
                            {
                                var albumTitleYearArtist = db.Read(Tools.Sql_queries["SelectAlbumTitleYearArtist"], Tools.MyFu, SqliteDb.GetSqliteParameters(new[] { ("@value", track.AlbumId) }), 3);
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

