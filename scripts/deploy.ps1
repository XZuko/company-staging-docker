param([Parameter(Mandatory=$true)][string]$Version)
$ErrorActionPreference = 'Stop'
$state = '.deploy/last-good-version'
New-Item -ItemType Directory -Force .deploy | Out-Null
$previous = if (Test-Path $state) { (Get-Content $state -Raw).Trim() } else { 'local' }
$env:APP_VERSION = $Version

try {
  docker compose config --quiet
  docker compose build api frontend
  docker compose up -d --wait
  docker compose --profile test run --rm smoke-test
  Set-Content $state $Version
  Write-Host "PASS: staging version $Version is healthy." -ForegroundColor Green
} catch {
  Write-Host "FAIL: deployment validation failed: $($_.Exception.Message)" -ForegroundColor Red
  docker compose ps
  docker compose logs --tail 100 database api frontend gateway
  if ($previous -ne 'local') {
    Write-Host "Rolling application services back to $previous" -ForegroundColor Yellow
    $env:APP_VERSION = $previous
    docker compose up -d --no-build --wait api frontend gateway
  } else {
    Write-Host 'No previous image version is recorded; unhealthy containers remain available for diagnosis.' -ForegroundColor Yellow
  }
  exit 1
}

