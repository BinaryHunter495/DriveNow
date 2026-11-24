using DriveNow.Models;
namespace DriveNow.Repositories.Interfaces;
public interface IDashboardRepository
{
    Task<AdminDashboardStats> GetAdminStatsAsync();
}
