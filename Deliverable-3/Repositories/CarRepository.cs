using System.Data;
using Dapper;
using DriveNow.Models;
using DriveNow.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace DriveNow.Repositories;

public class CarRepository : ICarRepository
{
    private readonly string _connectionString;
    public CarRepository(IConfiguration configuration) =>
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing.");

    private IDbConnection Connection() => new SqlConnection(_connectionString);

    public async Task<IEnumerable<Car>> GetAllAsync(string? search = null)
    {
        const string sql = @"SELECT Id, Name, Rate FROM Cars
            WHERE (@Search IS NULL OR Name LIKE '%' + @Search + '%')
            ORDER BY Id DESC";
        using var db = Connection();
        return await db.QueryAsync<Car>(sql, new { Search = string.IsNullOrWhiteSpace(search) ? null : search.Trim() });
    }

    public async Task<Car?> GetByIdAsync(int id)
    {
        using var db = Connection();
        return await db.QueryFirstOrDefaultAsync<Car>(
            "SELECT Id, Name, Rate FROM Cars WHERE Id = @Id", new { Id = id });
    }

    public async Task<int> AddAsync(Car car)
    {
        const string sql = @"INSERT INTO Cars (Name, Rate) VALUES (@Name, @Rate);
                            SELECT CAST(SCOPE_IDENTITY() AS int);";
        using var db = Connection();
        return await db.ExecuteScalarAsync<int>(sql, car);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var db = Connection();
        var used = await db.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Bookings WHERE CarId = @Id", new { Id = id });
        if (used > 0) return false;
        return await db.ExecuteAsync("DELETE FROM Cars WHERE Id = @Id", new { Id = id }) > 0;
    }
}
