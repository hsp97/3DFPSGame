보라색 베기 공격 이펙트 (VFX Graph)

필요 환경: Unity 6 (6000.3), URP 17.3, Visual Effect Graph 17.3

사용법
1. FX_ClaudeSlash 프리팹을 캐릭터(또는 무기) 아래에 놓습니다.
2. 공격하는 순간 코드에서 한 번 재생합니다.
     GetComponent<VisualEffect>().SendEvent("OnPlay");
3. 크기/모양은 VisualEffect 의 Exposed 값으로 조절합니다.
     Radius(반지름), Width(폭), SweepDegrees(휘두르는 각도), StartDegrees(시작 각도)

애니메이션에 자동으로 맞추고 싶다면
- ClaudeSlashAttack 컴포넌트를 캐릭터에 붙이고 Animator, Slash(이 프리팹의 VisualEffect),
  Attack State 이름, vfxAtNormalizedTime(공격 모션 중 터지는 시점 0~1)을 넣으세요.
- Space / 마우스 왼쪽 클릭으로 공격 + 이펙트가 나갑니다. (autoRepeat 를 0 으로 두면 자동 반복 끔)
