param(
    [string]$RepoName = "zombie-fps-stage1",
    [ValidateSet("public", "private")]
    [string]$Visibility = "public"
)

$ErrorActionPreference = "Stop"
$projectRoot = Split-Path -Parent $PSScriptRoot
Set-Location $projectRoot

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error "GitHub CLI(gh)가 없습니다. winget install GitHub.cli 로 설치하세요."
}

$auth = gh auth status 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "GitHub 로그인이 필요합니다. 아래 명령을 실행한 뒤 다시 시도하세요."
    Write-Host "  gh auth login"
    exit 1
}

if (-not (Test-Path ".git")) {
    Write-Error "git 저장소가 아닙니다: $projectRoot"
}

$status = git status --porcelain
if ($status) {
    git add -A
    git commit -m "Update project files"
}

$existingRemote = git remote get-url origin 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "origin 이미 존재: $existingRemote"
    git push -u origin HEAD
    exit 0
}

Write-Host "GitHub 저장소 생성: $RepoName ($Visibility)"
gh repo create $RepoName --$Visibility --source=. --remote=origin --push

if ($LASTEXITCODE -eq 0) {
    $url = gh repo view --json url -q .url
    Write-Host ""
    Write-Host "완료: $url"
}