# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 프로젝트 개요

Unity 6 (`6000.3.22f1`, URP) 기반 3D FPS 학습용 게임. 씬은 `Assets/01.Scenes/GameScene.unity` 하나뿐이다. 자동화 테스트는 없다(`Assets/02.Scripts/Test/`는 C# 문법 연습용 스크립트이며 테스트 프레임워크 테스트가 아니다).

## 명령어

빌드와 실행은 Unity 에디터에서 한다. CLI로 확인할 수 있는 것은 포맷 검사뿐이다.

```bash
# 최초 1회: Husky.Net 등 로컬 dotnet 도구 복원 (core.hooksPath = .husky)
dotnet tool restore

# 포맷/네이밍 검사 (pre-commit 훅과 동일)
dotnet format 3DFPSGame.sln --verify-no-changes --include Assets/02.Scripts/

# 자동 수정
dotnet format 3DFPSGame.sln --include Assets/02.Scripts/
```

`.sln`/`.csproj`는 Unity가 생성하므로 직접 수정하지 않는다.

## 커밋 규칙 (`.husky/commit-msg`가 강제)

- 첫 줄은 `type: 설명` 형식이어야 한다. 허용 type: `asset|build|chore|ci|design|docs|feat|fix|hotfix|perf|refactor|revert|style|test` (`type(scope)!:`도 허용)
- **본문이 최소 1줄 있어야 한다.** 본문이 없으면 커밋이 거부된다.
- 이모지(✨, 🐛 등)는 훅이 자동으로 붙이므로 메시지에 직접 쓰지 않는다.
- pre-commit 훅이 위의 `dotnet format --verify-no-changes`를 실행하므로 포맷 위반이 있으면 커밋이 실패한다.

## 코드 스타일 (`.editorconfig`, `Assets/02.Scripts/**.cs`에만 적용)

- 들여쓰기 4칸, 여는 중괄호는 항상 새 줄(Allman)
- 한 줄짜리 조기 리턴은 중괄호를 생략해도 되고, 여러 줄일 때만 중괄호 필수
- private 필드는 `_camelCase`, `private const`/`private static readonly`는 `PascalCase`, 지역변수는 `camelCase`, 타입명은 언더스코어 없는 `PascalCase`
  - 예외: UI 스크립트는 `UI_` 접두사(`UI_HitEffect`, `UI_Minimap` 등)를 쓰는 기존 관례가 있다.
- Inspector 노출 값은 public 필드 대신 `[SerializeField] private`를 쓴다.
- 코루틴 메서드는 `Xxx_Coroutine` 형태로 이름 짓는다.
- 주석은 한국어로 쓴다.

## 아키텍처

스크립트는 모두 `Assets/02.Scripts/` 아래에 기능별 폴더로 나뉘며, 네임스페이스와 asmdef 없이 기본 `Assembly-CSharp`에 들어간다.

- **게임 상태 게이트**: `GameManager`(싱글턴)가 `GameState`(`Ready → Playing → GameOver`)를 관리한다. 시작 시퀀스 코루틴이 끝나야 `Playing`이 된다. 플레이어 이동/사격/카메라 회전, 좀비 AI, 커서 제어 등 대부분의 `Update()`는 맨 앞에서 `GameManager.Instance.State != GameState.Playing`이면 리턴한다. 새 게임플레이 로직에도 같은 가드를 넣는다.
- **대미지 시스템**: `Core/IDamageable` + `Core/DamageInfo`(값, 맞은 지점, 법선) 구조체. `Player`, `Zombie`, `Drum`이 구현한다. 공격하는 쪽(`PlayerGunFire` 레이캐스트, `Drum` 폭발의 `OverlapSphere`, `Bomb`, 좀비 공격)은 구체 타입을 몰라도 되도록 `IDamageable`을 찾아 `TakeDamage`를 호출한다. 사망 판정은 맞는 쪽이 스스로 처리한다(플레이어 사망 시 `GameManager.GameOver()` 호출).
- **플레이어**: 한 오브젝트에 여러 컴포넌트로 책임이 나뉜다. `PlayerStats`가 체력/스태미나와 능력치를 가지고 재생을 처리하며, `PlayerMove`(CharacterController 이동, 2단 점프, 코요테 타임/점프 버퍼, 스태미나를 쓰는 질주), `PlayerGunFire`, `PlayerBombFire`, `PlayerRotate`, `Player`(피격)가 이를 참조한다.
- **좀비 AI**: `Zombie`는 `ZombieState` 기반 FSM(`Idle/Trace/Comeback/Attack/Hit/Death/Roaming`)이며 `Update()`의 switch문과 `ChangeState()`로 상태를 바꾼다. 이동은 NavMesh가 아니라 `CharacterController.SimpleMove`로 한다. 플레이어는 `"Player"` 태그로 찾는다.
- **카메라**: `Camera/` 아래에 FPS/TPS/쿼터뷰/숄더 등 시점별 스크립트가 따로 있다. `MinimapCamera`는 싱글턴이며, 렌더 텍스처와 마스크로 둥근 미니맵을 만들고 Q/E와 UI 버튼으로 줌을 조절한다.
- **입력**: Input System 패키지가 설치되어 있지만 `activeInputHandler: 2`(Both)로 설정되어 있고, 코드는 legacy `Input.GetAxis`/`GetButton`/`GetKeyDown`을 쓴다. `Cursor.visible`이 true(Left Alt를 누르고 있을 때)이면 사격하지 않는다.
- **레이어 의존**: 폭발 범위 대미지는 `LayerMask.GetMask("Drum", "Zombie")`를 쓰므로 새 대미지 대상을 추가할 때는 레이어 설정도 확인해야 한다.
- **로깅**: `GameLogger.Log`는 `[Conditional("ENABLE_LOG")]`라서 해당 심볼이 정의되지 않으면 호출 자체가 빠진다. Warning/Error는 항상 출력된다.


## 코드 리뷰 기준

리뷰 요청을 받으면 'Docs/CodeReviewGuide.md' 를 따른다. 핵심은 다음과 같다.

- 변경된 코드에서 실제로 발생할 가능성이 있는 문제를 우선한다.
- 단순한 취향 차이나 과도한 리팩터링을 반드시 수정해야 하는 문제처럼 표현하지 않는다.
- 문제를 발견하면 해당 파일과 코드 위치, 문제의 원인, 발생할 수 있는 결과를 구체적으로 설명한다.
- 가능하면 현실적인 수정 방법이나 간단한 코드 예시를 함께 제시한다.
- 변경된 코드와 직접 관련 없는 기존 문제는 별도의 참고사항으로 구분한다.
- 확실하지 않은 내용은 사실처럼 단정하지 않는다.
- 코드의 규모와 프로젝트 단계를 고려하여 적절한 수준의 개선안을 제안한다.
- 디자인 패턴이나 추상화를 무조건적으로 적용하지 않는다.

## 저장소 주의사항

- `3DFPSGame_GA1-main/`은 git으로 추적되는 별도 프로젝트 사본이다(구버전 스크립트, 별도 `.slnx`). 작업 대상은 루트의 `Assets/`이며, 요청이 없는 한 이 폴더는 수정하지 않는다.
- `main`에 `Assets/02.Scripts/**/*.cs` 변경이 push되면 GitHub Actions(`gemini-commit-review.yml`)가 Gemini로 커밋을 리뷰하고 커밋 코멘트를 남긴다. 리뷰 기준은 `Docs/CodeReviewGuide.md`에 있다. 코드 리뷰를 요청받으면 이 문서의 중요도 기준(Critical/High/Medium/Low)과 결과 형식(총평 → 발견된 문제 → 설계 개선 제안 → 잘 작성된 점)을 따른다. 과도한 추상화는 제안하지 않는다.
