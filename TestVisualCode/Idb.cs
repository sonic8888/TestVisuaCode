using System;
using Microsoft.Data.Sqlite;

namespace TestVisualCode;

public interface Idb
{
    SqliteConnection? Connection { get; set; }

    IEnumerable<T> Read<T>(string sql, Func<T> fu, IEnumerable<SqliteParameter> sqliteParameters);
    int Read(string sql,  IEnumerable<SqliteParameter> sqliteParameters);
    int Write(string sql, Func<int> fu, IEnumerable<SqliteParameter> sqliteParameters);
}
