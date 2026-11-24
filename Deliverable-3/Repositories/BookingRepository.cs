using System.Data;
using Dapper;
using DriveNow.Models;
using DriveNow.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
namespace DriveNow.Repositories;
public class BookingRepository : IBookingRepository
{
    private readonly string _connectionString;
    public BookingRepository(IConfiguration configuration) =>
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing.");
    private IDbConnection Connection() => new SqlConnection(_connectionString);

    private const string SelectSql = @"SELECT b.Id, b.CarId, b.UserEmail, b.Days,
        c.Name, c.Rate
        FROM Bookings b INNER JOIN Cars c ON c.Id = b.CarId";
    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        using var db = Connection();
        return await db.QueryAsync<Booking, Car, Booking>(
            SelectSql + " ORDER BY b.Id DESC",
            (booking, car) => { car.Id = booking.CarId; booking.Car = car; return booking; }, splitOn: "Name");
    }
    public async Task<IEnumerable<Booking>> GetByCustomerAsync(string email)
    {
        using var db = Connection();
        return await db.QueryAsync<Booking, Car, Booking>(
            SelectSql + " WHERE b.UserEmail = @Email ORDER BY b.Id DESC",
            (booking, car) => { car.Id = booking.CarId; booking.Car = car; return booking; },
            new { Email = email }, splitOn: "Name");
    }
    public async Task<int> AddAsync(Booking booking)
    {
        using var db = Connection();
        return await db.ExecuteAsync(
            "INSERT INTO Bookings (CarId, UserEmail, Days) VALUES (@CarId, @UserEmail, @Days)", booking);
    }
}
