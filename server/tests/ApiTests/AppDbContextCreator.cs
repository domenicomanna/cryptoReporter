using System.Data.Common;
using Api.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class AppDbContextCreator : IDisposable
{
    private DbConnection? _connection;

    public AppDbContext CreateContext()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        DbContextOptions contextOptions = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        AppDbContext context = new AppDbContext(contextOptions);
        context.Database.EnsureCreated();
        return context;
    }

    public void Dispose()
    {
        if (_connection is not null)
        {
            _connection.Dispose();
            _connection = null;
        }
    }
}
