# GitHub에 올리기

로컬 프로젝트는 이미 `git init` + 첫 커밋까지 완료된 상태입니다.  
아래 **한 번만** GitHub 로그인 후 실행하면 public 저장소가 생성되고 push 됩니다.

## 1. GitHub 로그인 (최초 1회)

PowerShell에서:

```powershell
gh auth login
```

- GitHub.com 선택
- HTTPS
- **Login with a web browser** 권장
- 안내에 따라 브라우저에서 인증

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
git clone https://github.com/<YOUR_USERNAME>/zombie-fps-stage1.git
```

Unity Hub로 폴더 열기 → `docs/QUICK_START.md` 또는 `docs/BUILD_FROM_SCRATCH.md` 참고.

## README URL 업데이트

push 후 `docs/QUICK_START.md`의 `<YOUR_REPO_URL>`을 실제 clone URL로 바꾸면 좋습니다.