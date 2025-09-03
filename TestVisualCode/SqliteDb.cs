using System;
using Microsoft.Data.Sqlite;

namespace TestVisualCode;

public class SqliteDb : Idb, IDisposable
{
    public SqliteConnection Connection { get; set; }
    public SqliteDb(string connectionString)
    {
        Connection = new SqliteConnection(connectionString);
    }

    public void Dispose()
    {
        if (Connection != null)
        {
            Connection.Dispose();
        }
    }

    public IEnumerable<T> Read<T>(string sql, Func<T> fu, IEnumerable<SqliteParameter> sqliteParameters)
    {
        Connection.Open();
        SqliteCommand command = new SqliteCommand(sql, Connection);
        command.Parameters.AddRange(sqliteParameters);
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            foreach (var item in reader)
            {
                yield return fu();
            }
        }
        Connection.Close();
    }

    public int Read(string sql, Func<int> fu, IEnumerable<SqliteParameter> sqliteParameters)
    {
        throw new NotImplementedException();
    }

    public int Write(string sql, Func<int> fu, IEnumerable<SqliteParameter> sqliteParameters)
    {
        throw new NotImplementedException();
    }

    public static string GetConnectionString(string DataSours, string mode = "ReadWriteCreate", string cache = "Default")
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
