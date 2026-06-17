# 처음부터 똑같이 만들기 (단계별 튜토리얼)

이 문서만 따라 하면 **저장소 없이** 동일한 Zombie FPS Stage 1을 만들 수 있습니다.  
또는 저장소를 클론한 뒤 스크립트·씬 구조를 이해하는 학습 자료로 쓸 수 있습니다.

**예상 소요:** 2~4시간 (Unity 초보 기준)

---

## 0단계: 새 프로젝트

1. Unity Hub → **New project**
2. 템플릿: **3D (URP)** 또는 **Universal 3D**
3. 프로젝트 이름: `ZombieFPS-Stage1`
4. Unity 버전: **6000.3.17f1** 권장

### 필수 패키지

**Window → Package Manager**에서 설치:

- **Input System** (`com.unity.inputsystem`)
- **Universal RP** (템플릿에 포함된 경우 생략)

Input System 설치 후 **재시작** 안내가 나오면 재시작하고, Active Input Handling을 **Input System Package**로 설정합니다.

---

## 1단계: 폴더·스크립트

`Assets/Scripts/` 아래 폴더 생성:

```
Scripts/
├── Combat/
├── Player/
├── Enemy/
└── Game/
```

이 저장소의 `Assets/Scripts/**/*.cs` 파일을 **그대로 복사**하거나, 아래 순서로 하나씩 붙여 넣습니다.

| 파일 | 역할 |
|------|------|
| `Combat/IDamageable.cs` | 데미지 인터페이스 |
| `Combat/HitscanWeapon.cs` | 히트스캔 사격 + VFX |
| `Player/PlayerHealth.cs` | 플레이어 HP |
| `Player/FirstPersonController.cs` | FPS 이동·시점 |
| `Enemy/ZombieHealth.cs` | 좀비 HP |
| `Enemy/ZombieAI.cs` | 좀비 추적·공격 |
| `Game/WaveManager.cs` | 스폰·승패 |
| `Game/GameUI.cs` | HUD·재시작 |

컴파일 에러가 없을 때까지 Unity 콘솔을 확인합니다.

---

## 2단계: 입력 (Input Actions)

1. `Assets/InputSystem_Actions.inputactions`를 이 저장소에서 복사  
   (또는 Hub URP 템플릿 기본 `InputSystem_Actions` 사용)
2. **Player** 액션 맵에 다음 액션이 있는지 확인:

| 액션 | 타입 | 연결 스크립트 |
|------|------|---------------|
| Move | Vector2 | FirstPersonController |
| Look | Vector2 | FirstPersonController |
| Jump | Button | FirstPersonController |
| Sprint | Button | FirstPersonController |
| Attack | Button | HitscanWeapon |

---

## 3단계: 맵 (Primitive)

`SampleScene`에서 다음 오브젝트를 만듭니다.

### Floor

- **3D Object → Cube** → 이름 `Floor`
- Position `(0, 0, 0)`, Scale `(20, 1, 20)`

### 벽 (Wall_N / Wall_S / Wall_E / Wall_W)

각각 **Cube**, Scale `(20, 3, 1)` 또는 `(1, 3, 20)`:

| 이름 | Position | Scale |
|------|----------|-------|
| Wall_N | `(0, 1.5, 10)` | `(20, 3, 1)` |
| Wall_S | `(0, 1.5, -10)` | `(20, 3, 1)` |
| Wall_E | `(10, 1.5, 0)` | `(1, 3, 20)` |
| Wall_W | `(-10, 1.5, 0)` | `(1, 3, 20)` |

### 스폰 포인트 (빈 GameObject)

이름 `SpawnPoint_1` ~ `SpawnPoint_5`:

| 이름 | Position |
|------|----------|
| SpawnPoint_1 | `(0, 0.5, 12)` |
| SpawnPoint_2 | `(11.3, 0.5, 4.6)` |
| SpawnPoint_3 | `(7.1, 0.5, -9.7)` |
| SpawnPoint_4 | `(-7.1, 0.5, -9.7)` |
| SpawnPoint_5 | `(-11.3, 0.5, 4.6)` |

---

## 4단계: Player

### 4-1. Player 오브젝트

1. **3D Object → Capsule** → 이름 `Player`
2. Tag: **Player** (없으면 Tag 추가)
3. Layer: **Player** (아래 4-4에서 생성)
4. Position: `(0, 1.5, 0)`
5. Capsule Collider **제거** (CharacterController 사용)
6. 컴포넌트 추가:
   - `Character Controller` — Height 2, Radius 0.5, Center `(0,0,0)`
   - `Player Health`
   - `First Person Controller`
   - `Hitscan Weapon`

### 4-2. PlayerCamera

1. Player 자식으로 **Camera** 생성 → `PlayerCamera`
2. Local Position: `(0, 1.6, 0)`
3. **Audio Listener** 유지
4. 씬의 **Main Camera**는 **비활성화**

### 4-3. Muzzle (중요)

1. **PlayerCamera** 자식으로 빈 GameObject → `Muzzle`
2. **Local Position: `(0, -0.1, 0.45)`**  
   ⚠️ Y를 1.5처럼 크게 잡으면 총알 VFX가 공중에 떠 보입니다.

### 4-4. Player 레이어

1. **Edit → Project Settings → Tags and Layers**
2. Layer 8에 `Player` 추가
3. Player 오브젝트 Layer를 **Player**로 설정

### 4-5. Inspector 연결

**First Person Controller**

- Input Actions → `InputSystem_Actions`
- Player Camera → `PlayerCamera` Transform

**Hitscan Weapon**

- Muzzle → `Muzzle`
- Player Camera → `PlayerCamera`
- Input Actions → `InputSystem_Actions`
- Layer Mask → **Player 레이어 체크 해제** (자기 몸 Raycast 제외)

---

## 5단계: Zombie 프리팹

1. **Capsule** → 이름 `Zombie`
2. 컴포넌트: `Zombie AI`, `Zombie Health`
3. `Assets/Prefabs/Zombie.prefab`으로 드래그 저장
4. 씬에 남은 Zombie 인스턴스는 삭제 (WaveManager가 스폰)

---

## 6단계: WaveManager

1. 빈 GameObject → `WaveManager`
2. `Wave Manager` 스크립트 추가
3. Zombie Prefab → `Zombie.prefab`
4. Spawn Points → Size **5**, `SpawnPoint_1`~`5` 드래그

---

## 7단계: Game UI

1. 빈 GameObject → `GameUI_Canvas`
2. `Game UI` 스크립트 추가 (Canvas는 런타임에 자동 생성됨)

**WaveManager** Inspector:

- Game UI → `GameUI_Canvas`의 GameUI 컴포넌트 연결

---

## 8단계: 조명·빌드 설정

- Directional Light 기본 유지
- **File → Build Settings** → `SampleScene`을 Scenes In Build에 추가

---

## 9단계: 테스트 체크리스트

- [ ] Play 시 커서 잠금, WASD 이동
- [ ] Shift 달리기 (8 m/s)
- [ ] 좌클릭 연사, 좀비 HP 40 → 2발 처치
- [ ] 총구 플래시·탄환 선이 **화면 중앙 근처**에서 시작
- [ ] HP 표시 `현재 / 최대`, 피격 시 빨간 플래시
- [ ] 5마리 처치 → Clear + R 재시작
- [ ] HP 0 → Game Over + R 재시작

---

## 10단계: OpenCode / AI로 확장할 때

동일 작업을 AI에게 시킬 때 예시 프롬프트:

```text
Unity 6 URP Zombie FPS Stage 1을 만든다.
- Primitive만 사용
- 스크립트는 Assets/Scripts 구조 그대로
- BUILD_FROM_SCRATCH.md 3~7단계 씬 구성 적용
- Muzzle localPosition (0, -0.1, 0.45) 필수
```

---

## 다음 단계 (Stage 2 아이디어)

- NavMeshAgent로 좀비 이동
- 탄약·재장전
- 사운드·파티클 VFX
- static 이벤트 → 인스턴스 이벤트로 리팩터