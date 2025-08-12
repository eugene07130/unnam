namespace UnnamHS_App_Backend.Services;

public enum RegisterResult
{
    Success,
    InvalidCode,        // 학생코드가 존재하지 않거나 사용 중
    UsernameTaken,     // 사용자 ID가 이미 존재
    CodeAlreadyUsed     // 학생코드가 이미 사용 중
}

