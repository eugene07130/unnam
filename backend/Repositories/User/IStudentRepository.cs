using UnnamHS_App_Backend.Models;

namespace UnnamHS_App_Backend.Repositories;

public interface IStudentRepository
{
    Task<bool> ExistsAsync(string studentCode);
    Task<Student?> GetAsync(string studentCode);
}
