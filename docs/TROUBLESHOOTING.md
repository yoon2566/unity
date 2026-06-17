# 문제 해결

## 총알·VFX가 공중에 떠 보임

### 원인 (가장 흔함)

`Muzzle`이 `PlayerCamera` 자식인데 **localPosition.y가 너무 큼** (예: 1.5).

카메라보다 1.5m 위에서 총구 플래시·탄환 선이 시작되어 허공에서 나가는 것처럼 보입니다.

### 해결

**Muzzle** Local Position을 다음으로 설정:

```
(0, -0.1, 0.45)
```

### 올바른 구조

| 구분 | 기준 |
|------|------|
| **판정 (Raycast)** | `PlayerCamera.position` + `forward` |
| **시각 (LineRenderer)** | `Muzzle.position` → `hit.point` |

Y를 0 근처·Z를 0.4~0.5로 두면 화면 중앙 근처에서 VFX가 시작됩니다.

---

## 탄환 선이 이동 후에도 공중에 남음

`HitscanWeapon`이 단일 `LineRenderer`를 재사용하고 `useWorldSpace = true`입니다.  
이전에 쏜 선이 0.08초간 월드에 고정되어 남을 수 있습니다.

**완화:** `tracerDuration`을 0.03~0.05로 줄이기  
**근본:** 매 발마다 tracer 오브젝트 생성 후 `Destroy` (Stage 2 개선 항목)

---

## Raycast가 자기 몸에 맞음

1. **Edit → Project Settings → Tags and Layers** → Layer 8에 `Player`
2. Player 오브젝트 Layer = Player
3. **Hitscan Weapon** → Layer Mask에서 **Player 체크 해제**

---

## Input이 동작하지 않음

- **Edit → Project Settings → Player → Active Input Handling**  
  → `Input System Package` 또는 `Both`
- `InputSystem_Actions`가 FirstPersonController·HitscanWeapon에 연결되었는지 확인

---

## UI가 안 보임

- `GameUI_Canvas`에 `GameUI` 스크립트가 붙어 있는지
- Player Tag가 `Player`인지
- Console에 NullReference 에러 없는지

---

## 좀비가 스폰되지 않음

- `WaveManager`에 Zombie Prefab·Spawn Points 5개 연결
- Prefab에 `ZombieAI`, `ZombieHealth` 있는지

---

## Game Over 후 R이 안 됨

정상 동작입니다. `timeScale = 0`이어도 **Input System**의 Keyboard는 동작합니다.  
`GameUI.Update()`에서 R키 시 씬을 리로드합니다.

---

## Unity MCP / Import Error

`com.coplaydev.unity-mcp`는 개발 도구입니다. 플레이만 할 때는 manifest에서 제거 가능.  
`Import Error Code:(4)`는 게임 로직과 무관한 경우가 많습니다.

---

## 콘솔 체크

Play 전 **Window → General → Console**에서 에러 0인지 확인하세요.