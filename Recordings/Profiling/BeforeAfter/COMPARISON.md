# 최적화 전후 프로파일링 비교 (2026-11-03)

## 측정 조건

- Unity 6000.3.22f1 에디터 Play 모드, Quality: PC, 1920×1080, Deep Profile 꺼짐
- 시점 고정
  - 시작 지점: 플레이어 (0, 0.89, 0), 방향 0°, 카메라 상하각 5°
  - 터널 안: 플레이어 (-24.17, 1.0, -12), 방향 180°, 카메라 상하각 5°
- 위치마다 약 300 게임 프레임 녹화 (`*.data`). 측정 중에는 플레이어를 무적으로 바꿔 GameOver가 나지 않게 함
- 표의 값은 렌더링이 일어난 프레임들의 **평균**입니다. 미니맵이 몇 프레임에 한 번만 그려지므로 중앙값 대신 평균을 씁니다. 중앙값은 `results.csv`에 있습니다.
- CPU 마커 시간은 한 프레임 안에서 같은 마커가 여러 번 호출되면 모두 더한 값입니다.

## 적용한 최적화

| # | 내용 | 대상 |
|---|---|---|
| 1 | 플레이어 모델(삼각형 50만 개) Cast Shadows Off | `Player/CallOfDuty_Ghost/texture_mapping` |
| 2 | Shadowmask 모드 Distance Shadowmask → Shadowmask | `ProjectSettings/QualitySettings.asset` (PC) |
| 3 | 터널 Spot Light 9개 그림자 끔 | `Map/Tunnel/Spot Light*` |
| 4 | 터널 벽/천장 Occluder·Occludee Static 설정 후 오클루전 다시 굽기 | `Map/Tunnel/Wall_*`, `Roof` |
| 5 | 미니맵 카메라를 매 프레임이 아니라 초당 15회만 렌더링 | `Assets/02.Scripts/Camera/MinimapCamera.cs` |

## 결과: 터널 안

| 구분 | 항목 | 적용 전 | 적용 후 | 변화 |
|---|---|---|---|---|
| CPU | PlayerLoop | 4.69 ms | 3.41 ms | **-27%** |
| CPU | 메인 스레드 프레임 시간 | 4.54 ms | 3.40 ms | -25% |
| CPU | 렌더 스레드 프레임 시간 | 3.21 ms | 2.50 ms | -22% |
| CPU | URP 렌더링 전체 (`Inl_UniversalRenderTotal`) | 2.54 ms | 1.41 ms | **-44%** |
| CPU | 메인 카메라 렌더 | 1.07 ms | 0.88 ms | -18% |
| CPU | 미니맵 카메라 렌더 | 0.55 ms | 0.10 ms | **-83%** |
| CPU | 컬링 (`CullScriptable`) | 0.39 ms | 0.23 ms | -41% |
| GPU | Batches | 236 | 81 | **-65%** |
| GPU | SetPass Calls | 94 | 54 | -43% |
| GPU | Triangles | 3.65M | 1.03M | **-72%** |
| GPU | Vertices | 2.92M | 0.79M | -73% |
| GPU | Shadow Casters | 143 | 3 | -98% |
| GPU | GPU Frame Time (에디터, 참고용) | 7.13 ms | 7.25 ms | 차이 없음 |
| RAM | Texture Memory | 1,552 MB | 1,528 MB | -24 MB |
| RAM | Render Textures | 313 MB | 309 MB | -4 MB |
| RAM | Mesh Memory | 149 MB | 149 MB | 0 |

## 결과: 시작 지점

| 구분 | 항목 | 적용 전 | 적용 후 | 변화 |
|---|---|---|---|---|
| CPU | PlayerLoop | 3.80 ms | 3.10 ms | -18% |
| CPU | 메인 스레드 프레임 시간 | 3.73 ms | 2.97 ms | -21% |
| CPU | 렌더 스레드 프레임 시간 | 2.98 ms | 2.52 ms | -15% |
| CPU | URP 렌더링 전체 | 2.15 ms | 1.41 ms | -35% |
| CPU | 미니맵 카메라 렌더 | 0.55 ms | 0.10 ms | -83% |
| CPU | 컬링 | 0.45 ms | 0.31 ms | -31% |
| GPU | Batches | 302 | 169 | -44% |
| GPU | SetPass Calls | 87 | 60 | -30% |
| GPU | Triangles | 3.29M | 1.11M | **-66%** |
| GPU | Vertices | 2.65M | 0.86M | -68% |
| GPU | Shadow Casters | 118 | 4 | -97% |
| GPU | GPU Frame Time (에디터, 참고용) | 7.04 ms | 6.73 ms | -4% |
| RAM | Texture Memory | 1,532 MB | 1,528 MB | -4 MB |
| RAM | Render Textures | 313 MB | 309 MB | -4 MB |

## 해석

- **CPU**: 렌더링 준비 비용이 크게 줄었습니다. 미니맵 갱신 주기를 낮춘 효과(-0.45ms)와 그림자 캐스터 컬링·Draw Call이 줄어든 효과가 합쳐진 결과입니다.
- **GPU**: 그리는 양(삼각형, Batch, 그림자 캐스터)은 1/3 이하로 줄었습니다. 하지만 에디터에서 잰 GPU Frame Time은 거의 변하지 않았습니다.
  - 에디터에서는 GPU 클럭 변동과 에디터 창 렌더링이 섞여서 이 비교가 믿기 어렵습니다.
  - GPU 시간은 Development Build에서 다시 확인해야 합니다.
- **RAM**: 이번 작업은 RAM을 거의 줄이지 못했습니다. 라이트맵(약 973MB)을 다시 굽지 않았기 때문입니다. Lightmap Resolution, Directional 모드, Shadowmask 텍스처 정리가 다음 단계입니다.

## 파일

| 파일 | 내용 |
|---|---|
| `before_*_1_rendering.png` / `after_*_1_rendering.png` | Rendering 모듈 상세 (Batch, 삼각형, 그림자 캐스터) |
| `before_*_2_cpu.png` / `after_*_2_cpu.png` | CPU Hierarchy (`Inl_UniversalRenderTotal` 선택) |
| `before_tunnel_3_memory.png` / `after_tunnel_3_memory.png` | Memory 모듈 |
| `after_tunnel_view.png` | 터널 측정 시점 화면 |
| `after_minimap_check.png` | 적용 후 HUD와 미니맵 정상 표시 확인 |
| `results.csv` | 모든 수치 (중앙값, 평균) |
| `*.data` | 원본 프로파일 데이터. Profiler 창의 Load로 열 수 있음 |
