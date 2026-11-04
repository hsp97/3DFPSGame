using System.Text.RegularExpressions;

// 아이디 입력 규칙. 아이디는 이메일 형식이어야 한다.
public static class AccountIdSpecification
{
    // 아이디: @ 앞뒤로 공백과 @가 없는 글자가 있고, @ 뒤에는 점으로 나뉜 도메인이 온다.
    private static readonly Regex IdPattern = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    public static string ErrorMessage => "아이디는 이메일 형식이어야 합니다.";
    // 아이디 규칙: 이메일 형식(이름@도메인.최상위도메인, 공백 없음, @는 하나)
    public static bool IsValidId(string id)
    {
        return IdPattern.IsMatch(id);
    }
}
