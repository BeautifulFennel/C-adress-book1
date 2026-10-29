param(
    [string]$PhpPath = 'C:\Program Files\XAMPP\php\php.exe',
    [string]$MantisPath = '',
    [int]$Port = 8085
)

$ErrorActionPreference = 'Stop'
$projectDirectory = Split-Path $PSScriptRoot -Parent
$repositoryDirectory = Split-Path $projectDirectory -Parent

if (-not $MantisPath) {
    $sourceDirectory = Join-Path $repositoryDirectory 'artifacts\mantis\source'
    $installation = Get-ChildItem -LiteralPath $sourceDirectory -Directory |
        Where-Object { Test-Path -LiteralPath (Join-Path $_.FullName 'vendor\autoload.php') } |
        Select-Object -First 1
    if (-not $installation) {
        throw 'MantisBT with dependencies was not found. Pass its directory using -MantisPath.'
    }
    $MantisPath = $installation.FullName
}

$MantisPath = (Resolve-Path -LiteralPath $MantisPath).Path
if (-not (Test-Path -LiteralPath (Join-Path $MantisPath 'core.php'))) {
    throw "MantisBT was not found in $MantisPath."
}
if (-not (Test-Path -LiteralPath $PhpPath)) {
    throw "PHP was not found: $PhpPath. Pass its path using -PhpPath."
}

$localDirectory = Join-Path $projectDirectory '.local'
$sessionDirectory = Join-Path $localDirectory 'sessions'
New-Item -ItemType Directory -Path $sessionDirectory -Force | Out-Null
$serverUrl = "http://127.0.0.1:$Port/"
try {
    $response = Invoke-WebRequest -Uri ($serverUrl + 'login_page.php') -UseBasicParsing -TimeoutSec 3
    if ($response.Content -match 'MantisBT') {
        Write-Output "MantisBT is already running: $serverUrl"
        return
    }
    throw "Port $Port is in use by another HTTP server."
}
catch [System.Net.WebException] {
    if ($_.Exception.Response) {
        throw "Port $Port is in use by an HTTP server. Check the URL or choose another -Port."
    }
}

$soapEnabled = (& $PhpPath -m) -contains 'soap'
$arguments = @()
if (-not $soapEnabled) {
    # Enable SOAP for this server process without changing the global php.ini.
    $arguments += @('-d', 'extension=soap')
}
$arguments += @(
    '-d', ('"session.save_path=' + $sessionDirectory + '"'),
    '-S', "127.0.0.1:$Port",
    '-t', ('"' + $MantisPath + '"')
)
$serverProcess = Start-Process -FilePath $PhpPath -ArgumentList $arguments -WindowStyle Hidden -PassThru `
    -WorkingDirectory $MantisPath `
    -RedirectStandardOutput (Join-Path $localDirectory 'server.stdout.log') `
    -RedirectStandardError (Join-Path $localDirectory 'server.stderr.log')
$serverProcess.Id | Set-Content -LiteralPath (Join-Path $localDirectory 'server.pid')
Write-Output "Local MantisBT PHP server started: $serverUrl (PID $($serverProcess.Id))."
