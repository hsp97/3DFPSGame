using System.Text.RegularExpressions;

// 계정 입력 규칙. 아이디는 이메일, 비밀번호는 영문/숫자/특수문자로 된 7~15 글자이고 대문자와 특수문자를 하나 이상 포함한다.
public static class AccountRule
{
    // 아이디@도메인.최상위도메인 형태인지만 본다.
    private static readonly Regex EmailPattern = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    // 공백을 뺀 영문 키보드 글자(영문, 숫자, 특수문자)만 7~15 글자.
    private static readonly Regex PasswordCharsPattern = new Regex(@"^[\x21-\x7E]{7,15}$");
    private static readonly Regex UpperCasePattern = new Regex(@"[A-Z]");
    private static readonly Regex SpecialCharPattern = new Regex(@"[^A-Za-z0-9]");

    public static bool IsValidId(string id)
    {
        return EmailPattern.IsMatch(id);
    }

    public static bool IsValidPassword(string password)
    {
        return PasswordCharsPattern.IsMatch(password)
            && UpperCasePattern.IsMatch(password)
            && SpecialCharPattern.IsMatch(password);
    }
}
