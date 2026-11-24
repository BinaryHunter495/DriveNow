using DriveNow.Models;
namespace DriveNow.Repositories.Interfaces;
public interface ICarRepository
{
    Task<IEnumerable<Car>> GetAllAsync(string? search = null);
    Task<Car?> GetByIdAsync(int id);
    // Returns the newly created car ID.
    Task<int> AddAsync(Car car);
    Task<bool> DeleteAsync(int id);
}
