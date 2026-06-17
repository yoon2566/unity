# Zombie FPS — Stage 1

Unity 6 URP로 만든 **1인칭 좀비 슈팅** 프로토타입입니다. 에셋 스토어 없이 **기본 Primitive**만 사용합니다.

| 항목 | 내용 |
|------|------|
| Unity | **6000.3.17f1** (Unity 6) |
| 렌더 파이프라인 | URP |
| 메인 씬 | `Assets/Scenes/SampleScene.unity` |
| 입력 | Input System (`Assets/InputSystem_Actions.inputactions`) |

## 빠른 시작 (저장소 그대로 실행)

1. [Unity Hub](https://unity.com/download)에서 **Unity 6000.3.17f1** 설치
2. 이 저장소 클론 후 Hub에서 **Add → 프로젝트 폴더 선택**
3. `Assets/Scenes/SampleScene.unity` 열기
4. **Play** 버튼

### 조작

| 입력 | 동작 |
|------|------|
| WASD | 이동 |
| 마우스 | 시점 |
| Space | 점프 |
| Left Shift (누름) | 달리기 |
| 좌클릭 (누름) | 연사 |
| R | 승리/패배 후 재시작 |

### 목표

- 좀비 **5마리**를 모두 처치 → `Stage 1 Clear!`
- HP **0** → `Game Over`
- 종료 후 **R** 키로 재시작

## 게임 스펙

| 요소 | 값 |
|------|-----|
| 좀비 수 | 5 |
| 좀비 HP | 40 |
| 좀비 속도 | 2.5 |
| 좀비 공격 거리 / 쿨다운 / 데미지 | 1.5m / 1.0s / 15 |
| 플레이어 HP | 100 |
| 총기 데미지 / 사거리 / 연사 | 20 / 100 / 0.2s |
| 이동 (걷기 / 달리기) | 5 / 8 |

## 프로젝트 구조

```
Assets/
├── Scenes/SampleScene.unity    # 메인 플레이 씬
├── Prefabs/Zombie.prefab       # 좀비 프리팹
├── InputSystem_Actions.inputactions
├── Scripts/
│   ├── Combat/   HitscanWeapon, IDamageable
│   ├── Player/   FirstPersonController, PlayerHealth
│   ├── Enemy/    ZombieAI, ZombieHealth
│   └── Game/     WaveManager, GameUI
└── Settings/                   # URP 설정
```

## 문서

| 문서 | 설명 |
|------|------|
| [docs/QUICK_START.md](docs/QUICK_START.md) | 클론 후 실행·문제 해결 |
| [docs/BUILD_FROM_SCRATCH.md](docs/BUILD_FROM_SCRATCH.md) | **처음부터 똑같이 만드는** 단계별 튜토리얼 |
| [docs/GAME_SPEC.md](docs/GAME_SPEC.md) | 상세 스펙·시스템 설명 |
| [docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md) | 총알 공중 표시 등 알려진 이슈 |

## 처음부터 만들고 싶다면

저장소를 **그대로 열어서 플레이**하거나, 학습용으로 **직접 조립**할 수 있습니다.

→ **[docs/BUILD_FROM_SCRATCH.md](docs/BUILD_FROM_SCRATCH.md)** 를 순서대로 따라 하세요.

## 개발 환경 참고

이 프로젝트는 Grok + OpenCode + Unity MCP로 개발되었습니다. 게임 플레이만 필요하면 `Packages/manifest.json`의 `com.coplaydev.unity-mcp` 항목은 제거해도 됩니다.

## 라이선스

학습·포트폴리오용으로 자유롭게 사용·수정 가능합니다.