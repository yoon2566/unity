param(
    [switch]$SkipGhLogin
)

$ErrorActionPreference = "Stop"
$keyPath = "$env:USERPROFILE\.ssh\id_ed25519_github"
$pubPath = "$keyPath.pub"
$sshConfig = "$env:USERPROFILE\.ssh\config"

Write-Host "=== GitHub SSH 설정 ===" -ForegroundColor Cyan

if (-not (Test-Path $keyPath)) {
    Write-Host "SSH 키 생성 중..."
    ssh-keygen -t ed25519 -C "github-zombie-fps" -f $keyPath -N '""'
}

if (-not (Test-Path $sshConfig) -or -not (Select-String -Path $sshConfig -Pattern "Host github.com" -Quiet)) {
    @"

Host github.com
  HostName github.com
  User git
  IdentityFile ~/.ssh/id_ed25519_github
  IdentitiesOnly yes
"@ | Add-Content -Path $sshConfig -Encoding utf8
    Write-Host "SSH config에 github.com 항목 추가됨"
}

$pub = Get-Content $pubPath -Raw
Set-Clipboard -Value $pub.Trim()
Write-Host ""
Write-Host "공개키가 클립보드에 복사되었습니다." -ForegroundColor Green
Write-Host $pub.Trim()
Write-Host ""
Write-Host "GitHub에 키 등록:" -ForegroundColor Yellow
Write-Host "  https://github.com/settings/ssh/new"
Write-Host "  Title: zombie-fps-pc (아무 이름)"
Write-Host "  Key: Ctrl+V 붙여넣기 → Add SSH key"
Write-Host ""

Start-Process "https://github.com/settings/ssh/new"

if (-not $SkipGhLogin) {
    $gh = "C:\Program Files\GitHub CLI\gh.exe"
    if (Test-Path $gh) {
        $auth = & $gh auth status 2>&1
        if ($LASTEXITCODE -ne 0) {
            Write-Host "GitHub CLI 로그인 (브라우저에서 승인):" -ForegroundColor Yellow
            Write-Host "  SSH 프로토콜 + 기존 키 업로드 옵션 선택 가능"
            & $gh auth login --hostname github.com --git-protocol ssh --web
        } else {
            & $gh config set git_protocol ssh
            Write-Host "gh 이미 로그인됨. git_protocol=ssh 설정 완료." -ForegroundColor Green
        }
    }
}

Write-Host ""
Write-Host "키 등록 + gh 로그인 후:" -ForegroundColor Cyan
Write-Host "  cd C:\Users\User\3d"
Write-Host "  .\scripts\push-to-github.ps1 -Protocol ssh"
Write-Host ""
Write-Host "연결 테스트:"
Write-Host "  ssh -T git@github.com"