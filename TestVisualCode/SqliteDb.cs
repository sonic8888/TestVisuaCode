using System;
using Microsoft.Data.Sqlite;

namespace TestVisualCode;

public class SqliteDb : Idb, IDisposable
{
    public SqliteConnection? Connection { get; set; }
    public string StrConnection { get; set; }
    private static string nameDb = "my_music.sqlite";
    public SqliteDb(string soursDb)
    {
        StrConnection = GetConnectionString(soursDb);
        Connection = GetConnection();
    }

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
    public static string GetNameDbDefault(string pathDir)
    {
        if (!Directory.Exists(pathDir)) throw new ArgumentException($"Папка:{pathDir} не найдена.");
        return Path.Combine(pathDir, nameDb);
    }

    public void Dispose()
    {
        if (Connection != null)
        {
            Connection.Dispose();
        }
    }

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
        catch (System.Exception ex)
        {

            Tools.DisplayColor(ex.Message, ConsoleColor.Red);
        }

        finally
        {

            Connection?.Close();
        }
        return list;
    }

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
        catch (System.Exception ex)
        {

            Tools.DisplayColor(ex.Message, ConsoleColor.Red);
        }
        finally
        {
            Connection?.Close();
        }
        return count;
    }

    public int Write(string sql, IEnumerable<SqliteParameter>? sqliteParameters = null)
    {
        int rows = 0;
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
        catch (System.Exception ex)
        {

            Tools.DisplayColor(ex.Message, ConsoleColor.Red);
        }
        finally
        {
            Connection?.Close();
        }
        return rows;
    }

    private string GetConnectionString(string DataSours, string mode = "ReadWriteCreate", string cache = "Default")
    {
        return $"Data Source={DataSours};Mode={mode};Cache={cache}";
    }
    public static List<SqliteParameter> GetSqliteParameters(IEnumerable<(string, string)> parameters)
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

    public static SqliteDb? CreateDB(string pathDir)
    {
        SqliteDb? db = null;
        try
        {
            string dataSours = SqliteDb.GetNameDbDefault(pathDir);
            db = new SqliteDb(dataSours);
            db.Write(Tools.Sql_queries["CreateTables"]);
        }
        catch (System.Exception ex)
        {
            db?.Dispose();
            Tools.DisplayColor(ex.Message, ConsoleColor.Red);
        }
        return db;
    }


}
