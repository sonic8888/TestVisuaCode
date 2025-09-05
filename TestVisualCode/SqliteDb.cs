using System;
using Microsoft.Data.Sqlite;

namespace TestVisualCode;

public class SqliteDb : Idb, IDisposable
{
    public SqliteConnection? Connection { get; set; }
    public string StrConnection { get; set; }
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

    public void Dispose()
    {
        if (Connection != null)
        {
            Connection.Dispose();
        }
    }

    public IEnumerable<T> Read<T>(string sql, Func<SqliteDataReader, T> fu, IEnumerable<SqliteParameter> sqliteParameters )
    {
        List<T> list = new List<T>();
        try
        {
            Connection?.Open();
            SqliteCommand command = new SqliteCommand(sql, Connection);
            command.Parameters.AddRange(sqliteParameters);
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var res = fu(reader);
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

    public int Read(string sql, IEnumerable<SqliteParameter> sqliteParameters)
    {
        int count = 0;
        try
        {
            Connection?.Open();
            SqliteCommand command = new SqliteCommand(sql, Connection);
            command.Parameters.AddRange(sqliteParameters);
            var result = command.ExecuteScalar();
            if (result != null)
            {
                count = Convert.ToInt32(result);
            }

        }
        catch (System.Exception ex)
        {

            System.Console.WriteLine(ex.Message);
        }
        finally
        {
            Connection?.Close();
        }
        return count;
    }

    public int Write(string sql, Func<int> fu, IEnumerable<SqliteParameter> sqliteParameters)
    {
        throw new NotImplementedException();
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

}
