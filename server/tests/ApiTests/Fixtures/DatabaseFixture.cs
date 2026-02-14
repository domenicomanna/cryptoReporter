using Api.Database;
using Api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;

namespace Fixtures;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:18").Build();

    public Task InitializeAsync() => _container.StartAsync();

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    public async Task<AppDbContext> CreateContext()
    {
        DbContextOptions contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;
        AppDbContext context = new AppDbContext(contextOptions);
        await context.Database.EnsureCreatedAsync();
        // reset the database to a clean state
        await ResetDatabase(context);
        return context;
    }

    private async Task ResetDatabase(AppDbContext context)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(_container.GetConnectionString());
        await connection.OpenAsync();
        Respawner respawner = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions
            {
                // prevent lookup tables from getting truncated
                TablesToIgnore = new Table[]
                {
                    context.Model.FindEntityType(typeof(FiatCurrencyType))?.GetTableName() ?? "",
                    context.Model.FindEntityType(typeof(TransactionType))?.GetTableName() ?? "",
                },
                SchemasToInclude = new[] { "public" },
                DbAdapter = DbAdapter.Postgres,
            }
        );
        await respawner.ResetAsync(connection);
        await connection.CloseAsync();
    }
}
