using UnnamHS_App_Backend.Models;
using System.Threading.Tasks;

namespace UnnamHS_App_Backend.Services;

public interface IStudentVerifyService
{
    // 학생코드가 존재 && Users에 아직 연결되지 않았는가?
    Task<bool> IsUsableAsync(string studentCode);

    // 학생코드로 Student 조회
    Task<Student?> GetByCodeAsync(string studentCode);
}

