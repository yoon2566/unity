# 빠른 시작 가이드

## 요구 사항

- **Unity Hub** + **Unity 6000.3.17f1** (또는 Unity 6 URP 템플릿 호환 버전)
- Windows / macOS / Linux (Input System 사용)

## 1. 저장소 받기

```bash
git clone <YOUR_REPO_URL>
cd zombie-fps-stage1
```

또는 GitHub에서 **Code → Download ZIP** 후 압축 해제.

## 2. Unity에서 열기

1. Unity Hub → **Add** → 프로젝트 폴더 선택
2. 첫 실행 시 패키지 임포트 대기 (URP, Input System 등)
3. `Assets/Scenes/SampleScene.unity` 더블클릭

## 3. Input System 확인

Unity 6 URP 템플릿은 보통 Input System이 이미 활성화되어 있습니다.

확인 경로: **Edit → Project Settings → Player → Active Input Handling**

- `Input System Package (New)` 또는 `Both` 여야 합니다.

## 4. Play

- Hierarchy에서 **Player**가 `(0, 1.5, 0)` 근처에 있는지 확인
- **Main Camera**는 비활성화, **PlayerCamera**가 활성인지 확인
- Play 후 WASD + 마우스 + 좌클릭

## 5. 기대 동작

- 시작 시 좀비 5마리 스폰
- 좌상단 `HP: 100 / 100`
- 화면 중앙 십자 조준선
- 좀비 2발(데미지 20×2)에 처치
- 전멸 시 `Stage 1 Clear!` + 일시정지 → **R** 재시작

## 선택: Unity MCP 제거

MCP 없이 플레이만 할 경우 `Packages/manifest.json`에서 다음 줄 삭제:

```json
"com.coplaydev.unity-mcp": "https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main",
```

삭제 후 Unity가 패키지 목록을 다시 해석할 때까지 잠시 대기합니다.

## 문제가 있으면

→ [TROUBLESHOOTING.md](TROUBLESHOOTING.md)