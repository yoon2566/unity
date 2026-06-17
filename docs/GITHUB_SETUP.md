# GitHub에 올리기

로컬 프로젝트는 이미 `git init` + 첫 커밋까지 완료된 상태입니다.

## OAuth vs SSH — 뭘 쓸까?

| | **OAuth (브라우저 로그인)** | **SSH** |
|---|---------------------------|---------|
| 난이도 | 쉬움 (추천) | 키 생성·등록 필요 |
| 설정 | `gh auth login` 한 번 | `ssh-keygen` + GitHub에 공개키 등록 |
| 이 프로젝트 push | `push-to-github.ps1` 바로 사용 | `-Protocol ssh` 옵션 사용 |
| 장점 | 5분 안에 끝, gh·git 둘 다 연동 | PC마다 키만 있으면 비밀번호 없이 push |
| 단점 | 토큰 만료 시 재로그인 | 초기 설정이 조금 더 김 |

**지금 한 번 올리기만 하면 된다** → **OAuth**  
**앞으로 git을 자주 쓸 예정** → **SSH**도 좋음

---

## 방법 A: OAuth (추천)

### 1. GitHub 로그인 (최초 1회)

```powershell
gh auth login
```

선택 순서:

1. `GitHub.com`
2. `HTTPS`
3. `Login with a web browser` (OAuth)
4. 브라우저에서 승인

### 2. 저장소 생성 + push

```powershell
cd C:\Users\User\3d
.\scripts\push-to-github.ps1
```

---

## 방법 B: SSH

### 1. SSH 키 만들기 (없을 때만)

```powershell
ssh-keygen -t ed25519 -C "your_email@example.com" -f "$env:USERPROFILE\.ssh\id_ed25519_github"
```

Enter 두 번 (패스프레이즈 없이도 가능).

### 2. 공개키를 GitHub에 등록

```powershell
Get-Content "$env:USERPROFILE\.ssh\id_ed25519_github.pub" | Set-Clipboard
```

GitHub → **Settings → SSH and GPG keys → New SSH key** → 붙여넣기 → 저장

### 3. ssh-agent에 키 등록 (Windows)

```powershell
Get-Service ssh-agent | Set-Service -StartupType Manual
Start-Service ssh-agent
ssh-add "$env:USERPROFILE\.ssh\id_ed25519_github"
```

### 4. gh도 SSH로 로그인

```powershell
gh auth login
```

선택 순서:

1. `GitHub.com`
2. **`SSH`** ← 여기
3. 기존 키 경로 확인 또는 새로 생성
4. 브라우저 OAuth로 gh 인증 (gh CLI용, git push는 SSH 키 사용)

### 5. push (SSH remote)

```powershell
cd C:\Users\User\3d
.\scripts\push-to-github.ps1 -Protocol ssh
```

---

## 2. 저장소 생성 + push (자동)

프로젝트 폴더에서:

```powershell
cd C:\Users\User\3d
.\scripts\push-to-github.ps1
```

기본 저장소 이름: `zombie-fps-stage1` (public)

다른 이름을 쓰려면:

```powershell
.\scripts\push-to-github.ps1 -RepoName "my-zombie-fps"
```

## 3. 수동으로 할 경우

```powershell
cd C:\Users\User\3d
gh repo create zombie-fps-stage1 --public --source=. --remote=origin --push
```

## 저장소에 포함되는 것

| 포함 | 제외 (.gitignore) |
|------|-------------------|
| Assets/, ProjectSettings/, Packages/ | Library/, Temp/, Logs/ |
| 전체 C# 스크립트 + 씬 + 프리팹 | UserSettings/ |
| README + docs/ 튜토리얼 | 빌드 산출물 |

## 클론 후 재현 방법

다른 PC에서:

```bash
git clone git@github.com:yoon2566/unity.git
```

Unity Hub로 폴더 열기 → `docs/QUICK_START.md` 또는 `docs/BUILD_FROM_SCRATCH.md` 참고.

## README URL 업데이트

push 후 `docs/QUICK_START.md`의 `<YOUR_REPO_URL>`을 실제 clone URL로 바꾸면 좋습니다.