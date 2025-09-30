/// <summary>
/// Класс, представляющий музыкальный трек с метаданными
/// </summary>
/// <remarks>
/// Реализует интерфейс IComparable для сортировки по названию трека
/// </remarks>
namespace TestVisualCode
{
    /// <summary>
    /// Представляет аудиофайл с информацией о треке
    /// </summary>
    internal class Track : IComparable
    {
        /// <summary>
        /// Название трека (по умолчанию "unknown")
        /// </summary>
        private string title = "unknown";

        /// <summary>
        /// Расширение файла (по умолчанию ".mp3")
        /// </summary>
        public string Extension { get; set; } = ".mp3";

        /// <summary>
        /// Путь к источнику файла (может быть null)
        /// </summary>
        public string? PathSours { get; set; }
        public string? SourceFail { get; set; }

        /// <summary>
        /// Имя файла (по умолчанию пустая строка)
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Конструктор с идентификатором трека
        /// </summary>
        /// <param name="track_id">Идентификатор трека</param>
        public Track(string track_id)
        {
            TrackId = track_id;
        }

        /// <summary>
        /// Название трека
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// Идентификатор альбома (по умолчанию "-1")
        /// </summary>
        public string AlbumId { get; set; } = "-1";

        /// <summary>
        /// Исполнитель (может быть null)
        /// </summary>
        public string? Artist { get; set; } 

        /// <summary>
        /// Название альбома (по умолчанию "unknown")
        /// </summary>
        public string? Album { get; set; } = "unknown";

        /// <summary>
        /// Год выпуска (по умолчанию "unknown")
        /// </summary>
        public string? Year { get; set; } = "unknown";

        /// <summary>
        /// Идентификатор трека (по умолчанию "-1")
        /// </summary>
        public string TrackId { get; set; } = "-1";

        /// <summary>
        /// Дата создания (может быть null)
        /// </summary>
        public string? DataCreate { get; set; }



        /// <summary>
        /// Пустой конструктор по умолчанию
        /// </summary>
        public Track() { }

        /// <summary>
        /// Возвращает строковое представление объекта
        /// </summary>
        /// <returns>Строка с форматированными данными трека</returns>
        public override string ToString()
        {
            return $"Name:{Name}, Title:{Title}, Artist:{Artist}, Album:{Album}, Year:{Year}, TrackId:{TrackId}),  AlbumId:{AlbumId}, DataCreate:{DataCreate},  Extension:{Extension}";
        }

        /// <summary>
        /// Сравнивает текущий объект с другим объектом
        /// </summary>
        /// <param name="obj">Объект для сравнения</param>
        /// <returns>true, если объекты равны, иначе false</returns>
        /// <exception cref="ArgumentException">Если переданный объект не является Track</exception>
        public override bool Equals(object? obj)
        {
            if (obj is Track track)
            {
                return this.GetHashCode() == track.GetHashCode();
            }
            else
            {
                throw new ArgumentException("obj is not Track");
            }
        }

        /// <summary>
        /// Возвращает текущую дату в формате "д"
        /// </summary>
        /// <returns>Текущая дата в виде строки</returns>
        public static string Data() => DateTime.Now.ToString("d");

        /// <summary>
        /// Возвращает хэш-код на основе идентификатора трека
        /// </summary>
        /// <returns>Хэш-код</returns>
        public override int GetHashCode()
        {
            return TrackId.GetHashCode();
        }

        /// <summary>
        /// Сравнивает текущий трек с другим по названию
        /// </summary>
        /// <param name="obj">Трек для сравнения</param>
        /// <returns>Отрицательное число, 0 или положительное число в зависимости от порядка</returns>
        /// <exception cref="ArgumentException">Если переданный объект не является Track</exception>
        public int CompareTo(object? obj)
        {
            if (obj is Track track) return Title.CompareTo(track.Title);
            else throw new ArgumentException("Некорректное значение параметра"); ;
        }
    }
}
