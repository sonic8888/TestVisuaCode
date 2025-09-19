using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace TestVisualCode;

/// <summary>
/// Класс содержит вспомогательные методы и константы для работы с файлами, базой данных и отладкой.
/// </summary>
internal class Tools
{
    /// <summary>
    /// Тип данных для фильтрации (например, "5").
    /// </summary>
    internal static string? Kind = "5";

    /// <summary>
    /// Исходная директория для работы с файлами.
    /// </summary>
    internal static string? PathDir = null;

    /// <summary>
    /// Временная директория для перемещения файлов.
    /// </summary>
    static string? newTempFolderPath = null;

    /// <summary>
    /// Директория назначения для файлов.
    /// </summary>
    internal static string? PathDirDestination = @"D:\test";

    /// <summary>
    /// Директория для других операций (например, работа с БД).
    /// </summary>
    internal static string? PathDirOther = @"D:\testDb";

    /// <summary>
    /// Регулярное выражение для поиска недопустимых символов в именах.
    /// </summary>
    private static string Pattern = @"[\*\|\\\:\""<>\?\/]";

    /// <summary>
    /// Замена для недопустимых символов.
    /// </summary>
    private static string Target = ".";

    /// <summary>
    /// Словарь SQL-запросов для взаимодействия с базой данных.
    /// </summary>
    internal static Dictionary<string, string> Sql_queries = new Dictionary<string, string>
    {
        {"SelectTrackIdYandex", "SELECT TrackId FROM T_PlaylistTrack WHERE Kind = @value;" },
        {"SelectTrackIdDBDestination", "SELECT  TrackId FROM T_Track_Yandex WHERE Sours = @value" },
        { "SelectTitle", "SELECT Title FROM T_Track WHERE Id = @value" },
        {"SelectAlbumId", "SELECT AlbumId FROM T_TrackAlbum WHERE TrackId = @value;"},
        {"SelectAlbumTitleYearArtist", "SELECT Title, Year, ArtistsString FROM T_Album WHERE Id = @value" },
        {"CreateTables", "CREATE TABLE T_Track_Yandex (Id  INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE  NOT NULL, Name VARCHAR, Title  VARCHAR, Artist  VARCHAR, Album VARCHAR, Year VARCHAR, TrackId  VARCHAR,  Data  VARCHAR , Sours VARCHAR DEFAULT ('Yandex'));"},
        {"InsertTrack","INSERT INTO T_Track_Yandex (Name, Title, Artist,Album, Year, TrackId, Data)  VALUES (@name, @title, @artist,@album, @year, @track_id, @data)" },
        {"DeleteTrack", "DELETE FROM T_Track_Yandex WHERE TrackId = @value" },
        {"InsertTrackFromOther","INSERT INTO T_Track_Yandex (Name, Title, Artist,Album, Year, TrackId, Data, Sours)  VALUES (@name, @title, @artist,@album, @year, @track_id, @data, @sours)" },
    };

    /// <summary>
    /// Преобразует данные из SqliteDataReader в массив строк.
    /// </summary>
    /// <param name="reader">SqliteDataReader для чтения данных.</param>
    /// <param name="n">Количество элементов для чтения.</param>
    /// <returns>Массив строк с данными.</returns>
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
    /// Выводит текст на консоль с указанием цвета.
    /// </summary>
    /// <param name="message">Текст для отображения.</param>
    /// <param name="color">Цвет текста (по умолчанию белый).</param>
    /// <param name="isLine">Флаг, определяющий вывод строки или символа.</param>
    public static void DisplayColor(string message, System.ConsoleColor color = ConsoleColor.White, bool isLine = true)
    {
        Console.ForegroundColor = color;
        if (isLine)
        {
            Console.WriteLine(message);
        }
        else
        {
            Console.Write($"{message}: ");
        }
        Console.ResetColor();
    }

    /// <summary>
    /// Перемешивает файлы в указанной директории.
    /// </summary>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    public static async Task ShuffleFiles()
    {
        if (PathDir != null)
        {
            var dir = new DirectoryInfo(PathDir);
            var files = dir.GetFiles();
            if (await Task.Run(() => MoveTo(files)))
            {
                if (await Task.Run(() => MoveBack()))
                {
                    DisplayColor("Файлы успешно перемешаны.");
                }
            }
        }
        else
        {
            throw new ArgumentException($"Папка:{PathDir} не найдена.");
        }
    }

    /// <summary>
    /// Перемещает файлы во временную директорию.
    /// </summary>
    /// <param name="files">Массив файлов для перемещения.</param>
    /// <returns>Статус успешности операции.</returns>
    public static bool MoveTo(FileInfo[] files)
    {
        string tempPath = Path.GetTempPath();
        newTempFolderPath = Path.Combine(tempPath, "MyAppData");
        bool isSuccess = false;
        try
        {
            var tempDir = Directory.CreateDirectory(newTempFolderPath);
            foreach (var file in files)
            {
                file.MoveTo(Path.Combine(newTempFolderPath, file.Name));
            }
            isSuccess = true;
        }
        catch (System.Exception ex)
        {
            DisplayColor(ex.Message, ConsoleColor.Red);
        }
        return isSuccess;
    }

    /// <summary>
    /// Возвращает файлы из временной директории обратно.
    /// </summary>
    /// <returns>Статус успешности операции.</returns>
    public static bool MoveBack()
    {
        bool isSuccess = false;
        if (newTempFolderPath != null)
        {
            try
            {
                var tempDir = Directory.CreateDirectory(newTempFolderPath);
                var files = tempDir.GetFiles();
                Random.Shared.Shuffle(files);
                foreach (var file in files)
                {
                    file.MoveTo(Path.Combine(PathDir!, file.Name));
                    DisplayColor(file.Name, ConsoleColor.Green);
                }
                isSuccess = true;
            }
            catch (System.Exception ex)
            {
                DisplayColor(ex.Message, ConsoleColor.Red);
            }
        }
        return isSuccess;
    }

    /// <summary>
    /// Копирует файл в указанную директорию.
    /// </summary>
    /// <param name="track">Объект трека с путем источника.</param>
    /// <param name="dirDestination">Путь назначения.</param>
    /// <returns>Статус успешности операции.</returns>
    public static bool Copy(Track track, string dirDestination)
    {
        bool isSuccessful = true;
        try
        {
            if (!Directory.Exists(dirDestination)) throw new ArgumentException($"папка:{dirDestination} не найдена.");

            File.Copy(track.PathSours!, Path.Combine(dirDestination, track.Name));
        }
        catch (System.Exception ex)
        {
            isSuccessful = false;
            Tools.DisplayColor(ex.Message, ConsoleColor.Red, false);
            Tools.DisplayColor(track.ToString(), ConsoleColor.Blue);
        }
        return isSuccessful;
    }

    /// <summary>
    /// Проверяет наличие недопустимых символов в строке.
    /// </summary>
    /// <param name="text">Строка для проверки.</param>
    /// <returns>True, если найдены недопустимые символы.</returns>
    public static bool isNormalize(string text)
    {
        return Regex.IsMatch(text, Tools.Pattern);
    }

    /// <summary>
    /// Заменяет недопустимые символы в строке на допустимые.
    /// </summary>
    /// <param name="text">Строка для нормализации.</param>
    /// <returns>Нормализованная строка.</returns>
    public static string Normalize(string text)
    {
        return Regex.Replace(text, Tools.Pattern, Tools.Target);
    }

    /// <summary>
    /// Переименовывает трек с учетом артиста и альбома.
    /// </summary>
    /// <param name="track">Объект трека.</param>
    /// <param name="pathDir">Путь директории для проверки существования файла.</param>
    /// <returns>Обновленный объект трека.</returns>
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

    /// <summary>
    /// Переименовывает трек с учетом идентификатора.
    /// </summary>
    /// <param name="track">Объект трека.</param>
    /// <param name="pathDir">Путь директории для проверки существования файла.</param>
    /// <returns>Обновленный объект трека.</returns>
    public static Track RenameOther(Track track, string pathDir)
    {
        string text = track.Artist ?? track.TrackId;
        string _name = track.Name + track.Extension;
        int n = 0;
        while (File.Exists(Path.Combine(pathDir, _name)))
        {
            if (n == 0)
            {
                _name = $"{track.Name}({text}){track.Extension}";
            }
            else
            {
                _name = $"{track.Name}({track.TrackId}){track.Extension}";
            }
            n++;
        }
        track.Name = _name;
        return track;
    }

    /// <summary>
    /// Проверяет, является ли файл аудиофайлом.
    /// </summary>
    /// <param name="file">Информация о файле.</param>
    /// <returns>True, если файл имеет расширение .mp3, .flack или .wav.</returns>
    public static bool IsAudio(FileInfo file)
    {
        if (file == null) return false;
        return new[] { ".mp3", ".flack", ".wav" }.Contains(file.Extension.ToLower());
    }

    /// <summary>
    /// Получает список идентификаторов треков из базы данных.
    /// </summary>
    /// <param name="data_sours">Путь к базе данных.</param>
    /// <returns>Список идентификаторов треков.</returns>
    public static List<string> GetTrackIds(string? data_sours)
    {
        if (data_sours == null) throw new ArgumentException($"БД:{data_sours} не обнаружена.");
        var trackId = new List<string>();
        SqliteDb? db = null;
        try
        {
            db = new SqliteDb(data_sours);
            if (db.Connection != null)
            {
                var result = db.Read(Tools.Sql_queries["SelectTrackIdDBDestination"], Tools.MyFu, SqliteDb.GetSqliteParameters(new (string, string?)[] { ("@value", "Yandex") }));
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

    /// <summary>
    /// Генерирует случайный уникальный идентификатор трека.
    /// </summary>
    /// <param name="pathDirDestination">Путь к директории базы данных.</param>
    /// <returns>Случайный идентификатор трека.</returns>
    public static string GetRandomTrackId(string pathDirDestination)
    {
        string sours_db_destination = SqliteDb.GetNameDbDefault(pathDirDestination);
        var tracksIds = Tools.GetTrackIds(sours_db_destination);
        int trackId = -1;
        string other = "other_";
        do
        {
            trackId = new Random().Next() * -1;
        } while (tracksIds.Contains(other + trackId.ToString()));
        return other + trackId.ToString();
    }

    /// <summary>
    /// Создает объект трека из файла.
    /// </summary>
    /// <param name="file">Информация о файле.</param>
    /// <returns>Объект трека или null в случае ошибки.</returns>
    public static Track? GetTrackFromFile(FileInfo file)
    {
        Track? track = null;
        if (file != null)
        {
            try
            {
                var trackId = GetRandomTrackId(Tools.PathDirDestination!);
                track = new Track(trackId);
                TrackFromTags(track, file);
                track = RenameOther(track, Tools.PathDirDestination!);
            }
            catch (System.Exception)
            {
                Tools.DisplayColor("Не удалось создать объект класса Track.", ConsoleColor.Red);
                return null;
            }
        }
        return track;
    }

    /// <summary>
    /// Заполняет объект трека данными из метаданных файла.
    /// </summary>
    /// <param name="track">Объект трека для заполнения.</param>
    /// <param name="file">Информация о файле.</param>
    private static void TrackFromTags(Track track, FileInfo file)
    {
        var t_file = TagLib.File.Create(file.FullName);
        string title = t_file.Tag.Title ?? "unknown";
        string? year = t_file.Tag.Year > 0 ? t_file.Tag.Year.ToString() : "unknown";
        string? album = t_file.Tag.Album ?? "unknown";
        string? artist = t_file.Tag.Performers.Length > 0 ? t_file.Tag.Performers[0] : null;
        track.Title = title;
        track.Year = year;
        track.Album = album;
        track.Artist = artist;
        track.Extension = file.Extension;
        int i = file.Name.IndexOf(file.Extension);
        track.Name = file.Name.Substring(0, i);
        track.PathSours = file.FullName;
    }
}
