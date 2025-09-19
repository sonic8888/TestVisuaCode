using System;
using Microsoft.Data.Sqlite;

namespace TestVisualCode;

/// <summary>
/// Класс для работы с SQLite базой данных. Реализует интерфейс Idb и IDisposable.
/// </summary>
public class SqliteDb : Idb, IDisposable
{
    /// <summary>
    /// Свойство для хранения соединения с базой данных.
    /// </summary>
    public SqliteConnection? Connection { get; set; }

    /// <summary>
    /// Строка подключения к базе данных.
    /// </summary>
    public string StrConnection { get; set; }

    /// <summary>
    /// Имя файла базы данных по умолчанию.
    /// </summary>
    public static string nameDb = "my_music.sqlite";

    /// <summary>
    /// Конструктор класса. Инициализирует строку подключения и соединение с БД.
    /// </summary>
    /// <param name="soursDb">Путь к файлу базы данных.</param>
    public SqliteDb(string soursDb)
    {
        StrConnection = GetConnectionString(soursDb);
        Connection = GetConnection();
    }

    /// <summary>
    /// Создает и возвращает объект соединения с базой данных.
    /// </summary>
    /// <returns>Объект SqliteConnection.</returns>
    /// <exception cref="ArgumentException">Если подключение не удалось.</exception>
    private SqliteConnection? GetConnection()
    {
        SqliteConnection connection;
        try
        {
            connection = new SqliteConnection(StrConnection);
        }
        catch (System.ArgumentException)
        {
            throw new System.ArgumentException("Не удалось подключиться к БД.");
        }
        return connection;
    }

    /// <summary>
    /// Возвращает полный путь к файлу базы данных по умолчанию.
    /// </summary>
    /// <param name="pathDir">Путь к директории.</param>
    /// <returns>Полный путь к файлу базы данных.</returns>
    /// <exception cref="ArgumentException">Если директория не найдена.</exception>
    public static string GetNameDbDefault(string pathDir)
    {
        if (!Directory.Exists(pathDir)) throw new ArgumentException($"Папка:{pathDir} не найдена.");
        return Path.Combine(pathDir, nameDb);
    }

    /// <summary>
    /// Освобождает ресурсы, связанные с соединением.
    /// </summary>
    public void Dispose()
    {
        if (Connection != null)
        {
            Connection.Dispose();
        }
    }

    /// <summary>
    /// Выполняет SQL-запрос и возвращает результаты в виде коллекции объектов.
    /// </summary>
    /// <typeparam name="T">Тип элемента результата.</typeparam>
    /// <param name="sql">SQL-запрос.</param>
    /// <param name="fu">Функция преобразования данных.</param>
    /// <param name="sqliteParameters">Параметры запроса.</param>
    /// <param name="n">Параметр для функции преобразования.</param>
    /// <returns>Коллекция объектов типа T.</returns>
    public IEnumerable<T> Read<T>(string sql, Func<SqliteDataReader, int, T> fu, IEnumerable<SqliteParameter>? sqliteParameters = null, int n = 1)
    {
        List<T> list = new List<T>();
        try
        {
            Connection?.Open();
            SqliteCommand command = new SqliteCommand(sql, Connection);
            if (sqliteParameters != null)
            {
                command.Parameters.AddRange(sqliteParameters);
            }
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var res = fu(reader, n);
                        list.Add(res);
                    }
                }
            }
        }
        finally
        {
            Connection?.Close();
        }
        return list;
    }

    /// <summary>
    /// Выполняет SQL-запрос и возвращает скалярное значение.
    /// </summary>
    /// <param name="sql">SQL-запрос.</param>
    /// <param name="sqliteParameters">Параметры запроса.</param>
    /// <returns>Целое число, полученное из результата запроса.</returns>
    public int Read(string sql, IEnumerable<SqliteParameter>? sqliteParameters = null)
    {
        int count = 0;
        try
        {
            Connection?.Open();
            SqliteCommand command = new SqliteCommand(sql, Connection);
            if (sqliteParameters != null)
            {
                command.Parameters.AddRange(sqliteParameters);
            }
            var result = command.ExecuteScalar();
            if (result != null)
            {
                count = Convert.ToInt32(result);
            }
        }
        finally
        {
            Connection?.Close();
        }
        return count;
    }

    /// <summary>
    /// Выполняет SQL-запрос на запись данных и возвращает количество затронутых строк.
    /// </summary>
    /// <param name="sql">SQL-запрос.</param>
    /// <param name="sqliteParameters">Параметры запроса.</param>
    /// <returns>Количество затронутых строк.</returns>
    public int Write(string sql, IEnumerable<SqliteParameter>? sqliteParameters = null)
    {
        int rows = -1;
        try
        {
            Connection?.Open();
            SqliteCommand command = new SqliteCommand(sql, Connection);
            if (sqliteParameters != null)
            {
                command.Parameters.AddRange(sqliteParameters);
            }
            rows = command.ExecuteNonQuery();
        }
        finally
        {
            Connection?.Close();
        }
        return rows;
    }

    /// <summary>
    /// Выполняет SQL-запрос на запись данных с поддержкой DBNull и возвращает количество затронутых строк.
    /// </summary>
    /// <param name="sql">SQL-запрос.</param>
    /// <param name="connection">Соединение с базой данных.</param>
    /// <param name="sqliteParameters">Параметры запроса.</param>
    /// <returns>Количество затронутых строк.</returns>
    public static int WriteWithNull(string sql, SqliteConnection connection, IEnumerable<(string, string?)>? sqliteParameters = null)
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
        finally
        {
            connection?.Close();
        }
        return rows;
    }

    /// <summary>
    /// Выполняет SQL-запрос на запись данных без поддержки DBNull и возвращает количество затронутых строк.
    /// </summary>
    /// <param name="sql">SQL-запрос.</param>
    /// <param name="sqliteParameters">Параметры запроса.</param>
    /// <returns>Количество затронутых строк.</returns>
    public int WriteWithoutNull(string sql, IEnumerable<(string, string?)>? sqliteParameters = null)
    {
        int rows = -1;
        try
        {
            Connection?.Open();
            SqliteCommand command = new SqliteCommand(sql, Connection);
            if (sqliteParameters != null)
            {
                foreach (var item in sqliteParameters)
                {
                    command.Parameters.AddWithValue(item.Item1, item.Item2 ?? "unknown");
                }
            }
            rows = command.ExecuteNonQuery();
        }
        finally
        {
            Connection?.Close();
        }
        return rows;
    }

    /// <summary>
    /// Формирует строку подключения к базе данных.
    /// </summary>
    /// <param name="DataSours">Путь к файлу базы данных.</param>
    /// <param name="mode">Режим доступа к базе данных.</param>
    /// <param name="cache">Параметр кэширования.</param>
    /// <returns>Строка подключения.</returns>
    private string GetConnectionString(string DataSours, string mode = "ReadWriteCreate", string cache = "Default")
    {
        return $"Data Source={DataSours};Mode={mode};Cache={cache}";
    }

    /// <summary>
    /// Преобразует коллекцию параметров в список объектов SqliteParameter.
    /// </summary>
    /// <param name="parameters">Коллекция параметров.</param>
    /// <returns>Список SqliteParameter.</returns>
    public static List<SqliteParameter> GetSqliteParameters(IEnumerable<(string, string?)> parameters)
    {
        List<SqliteParameter> sqliteParameters = new List<SqliteParameter>();
        if (parameters != null)
        {
            foreach (var item in parameters)
            {
                if (item.Item1 != null)
                {
                    sqliteParameters.Add(new SqliteParameter(item.Item1, item.Item2 ?? "unknown"));
                }
            }
        }
        return sqliteParameters;
    }

    /// <summary>
    /// Создает новую базу данных и таблицы.
    /// </summary>
    /// <param name="pathDir">Путь к директории.</param>
    /// <returns>True, если создание успешно, иначе False.</returns>
    public static bool CreateDB(string pathDir)
    {
        SqliteDb? db = null;
        bool isSuccessful = false;
        try
        {
            string dataSours = SqliteDb.GetNameDbDefault(pathDir);
            db = new SqliteDb(dataSours);
            db.Write(Tools.Sql_queries["CreateTables"]);
            isSuccessful = true;
        }
        catch (System.Exception ex)
        {
            Tools.DisplayColor(ex.Message, ConsoleColor.Red);
        }
        finally
        {
            db?.Dispose();
        }
        return isSuccessful;
    }
}
