using System;
using Microsoft.Data.Sqlite;

namespace TestVisualCode;

public interface Idb
{
    SqliteConnection? Connection { get; set; }

    public IEnumerable<T> Read<T>(string sql, Func<SqliteDataReader,int, T> fu, IEnumerable<SqliteParameter>? sqliteParameters,int n);
    int Read(string sql,  IEnumerable<SqliteParameter>? sqliteParameters);
    int Write(string sql,  IEnumerable<SqliteParameter>? sqliteParameters);
}
