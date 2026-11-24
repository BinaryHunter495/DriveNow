using Dapper;
using DriveNow.Models;
using DriveNow.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
namespace DriveNow.Repositories;
public class DashboardRepository : IDashboardRepository
{
    private readonly string _connectionString;
    public DashboardRepository(IConfiguration configuration) =>
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing.");
    public async Task<AdminDashboardStats> GetAdminStatsAsync()
    {
        const string sql = @"SELECT
            (SELECT COUNT(*) FROM Cars) AS Cars,
            (SELECT COUNT(*) FROM Bookings) AS Bookings,
            (SELECT COALESCE(SUM(c.Rate * b.Days), 0)
             FROM Bookings b INNER JOIN Cars c ON c.Id = b.CarId) AS Revenue";
        await using var db = new SqlConnection(_connectionString);
        return await db.QuerySingleAsync<AdminDashboardStats>(sql);
    }
}
