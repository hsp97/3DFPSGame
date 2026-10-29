// 무기 모드. 마우스 오른쪽 버튼이 하는 일이 모드마다 다르다.
public enum WeaponMode
{
    Normal,  // 일반 모드(라이플): 폭탄을 던진다.
    Sniper,  // 저격 모드(스나이퍼): 조준경으로 확대한다.
}

public static class WeaponModeExtensions
{
    // 무기마다 장전할 수 있는 최대 총알 수가 다르다.
    public static int GetMaxBulletCount(this WeaponMode mode)
    {
        return mode switch
        {
            WeaponMode.Normal => 30,
            WeaponMode.Sniper => 5,
            _ => 0,
        };
    }
}
