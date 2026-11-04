using System.Text.RegularExpressions;

// 비밀번호 입력 규칙. 대문자, 소문자, 숫자, 기호를 하나 이상씩 포함한 8~30 글자다.
public static class AccountPasswordSpecification
{
    // 비밀번호: 앞쪽 (?=...)들로 대문자, 소문자, 숫자, 기호가 하나씩 있는지 보고,
    // 전체가 공백을 뺀 키보드 글자(! 부터 ~ 까지) 8~30 개인지 본다.
    private static readonly Regex PasswordPattern = new Regex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9])[!-~]{8,30}$");

    public static string ErrorMessage => "비밀번호는 대문자, 소문자, 숫자, 기호를 하나 이상씩 포함한 8~30 글자";

    // 비밀번호 규칙: 대문자, 소문자, 숫자, 기호를 하나 이상씩 포함한 8~30 글자
    public static bool IsValidPassword(string password)
    {
        return PasswordPattern.IsMatch(password);
    }
}
