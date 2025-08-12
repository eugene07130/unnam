namespace UnnamHS_App_Backend.Models;

public class Student
{
    public string StudentCode  { get; set; } = null!; // PK
    public string Name         { get; set; } = null!;
    
    /// <summary>
    /// 계정 생성 여부 (Users.StudentCode 존재 여부를 미리 캐시)
    /// </summary>
    public bool IsRegistered { get; set; } = false;

    public User? User         { get; set; } // 1:1 네비게이션
}
