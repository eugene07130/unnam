using UnnamHS_App_Backend.Models;


namespace UnnamHS_App_Backend.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string userId);                 // 로그인/권한체크용
    Task<User?> GetByStudentCodeAsync(string studentCode);   // 학생-사용자 매핑 확인용
    Task<bool> ExistsAsync(string userId);
    Task AddAsync(User user);                                // 회원가입
    Task<int>  SaveChangesAsync();
}
