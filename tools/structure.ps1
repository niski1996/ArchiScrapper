<#
.SYNOPSIS
Skrypt przetwarza solucję .NET i generuje folder Copilot/<Solucja>/<Projekt> z plikiem metadata.json zawierającym liczbę linii kodu dla każdego projektu nie-testowego.

.DESCRIPTION
1. Wyszukuje plik .sln w bieżącym katalogu lub przyjmuje ścieżkę do solucji jako parametr.
2. Tworzy folder Copilot/<NazwaSolucji>.
3. Tworzy podfolder dla każdego projektu w solucji, pomijając projekty testowe.
4. Liczy linie kodu .cs w projektach równolegle.
5. Tworzy lub aktualizuje metadata.json w folderze projektu.
#>

param(
    [string]$SolutionPath = (Get-ChildItem -Filter *.sln | Select-Object -First 1).FullName
)

if (-not $SolutionPath) {
    Write-Error "Nie znaleziono pliku .sln w bieżącym katalogu."
    exit
}

$SolutionName = [System.IO.Path]::GetFileNameWithoutExtension($SolutionPath)
Write-Host "Solucja: $SolutionName"

# Tworzenie folderu Copilot
$RepoRoot = Get-Location
$CopilotFolder = Join-Path $RepoRoot "Copilot"
if (-not (Test-Path $CopilotFolder)) {
    New-Item -ItemType Directory -Path $CopilotFolder | Out-Null
    Write-Host "Utworzono folder Copilot: $CopilotFolder"
}

# Tworzenie folderu dla solucji
$SolutionFolder = Join-Path $CopilotFolder $SolutionName
if (-not (Test-Path $SolutionFolder)) {
    New-Item -ItemType Directory -Path $SolutionFolder | Out-Null
    Write-Host "Utworzono folder dla solucji: $SolutionFolder"
}

# Parsowanie pliku .sln w poszukiwaniu projektów
$projects = Select-String -Path $SolutionPath -Pattern 'Project\("\{.*\}"\) = "([^"]+)", "([^"]+\.csproj)"' |
            ForEach-Object {
                $matches = [regex]::Match($_.Line, 'Project\("\{.*\}"\) = "([^"]+)", "([^"]+\.csproj)"')
                [PSCustomObject]@{
                    Name = $matches.Groups[1].Value
                    Path = $matches.Groups[2].Value
                }
            }

# Filtrowanie projektów testowych
$projectsToProcess = $projects | Where-Object { $_.Name -notmatch "Test|Tests" }

if (-not $projectsToProcess) {
    Write-Warning "Brak projektów do przetworzenia (wszystkie są testowe?)"
    exit
}

# Funkcja do liczenia linii i zapisu metadata
function Process-Project {
    param($proj, $SolutionFolder)

    try {
        Write-Host "Przetwarzanie projektu: $($proj.Name)"

        # Tworzenie folderu projektu w Copilot
        $ProjectFolder = Join-Path $SolutionFolder $proj.Name
        if (-not (Test-Path $ProjectFolder)) { New-Item -ItemType Directory -Path $ProjectFolder | Out-Null }

        # Lokalizacja projektu
        $ProjDir = Split-Path $proj.Path -Parent

        # Liczenie linii kodu .cs
        $lineCount = 0
        if (Test-Path $ProjDir) {
            $lineCount = Get-ChildItem -Path $ProjDir -Recurse -Filter *.cs -ErrorAction SilentlyContinue |
                         ForEach-Object {
                             try { (Get-Content $_ -ErrorAction Stop).Count } catch { 0 }
                         } |
                         Measure-Object -Sum | Select-Object -ExpandProperty Sum
        }

        # Tworzenie metadata.json
        $MetaFile = Join-Path $ProjectFolder "metadata.json"
        $metadata = @{
            project = $proj.Name
            path    = $proj.Path
            lines   = $lineCount
            updated = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
        }

        $metadata | ConvertTo-Json -Depth 3 | Out-File $MetaFile -Encoding UTF8
        Write-Host "✅ Metadata zapisane dla $($proj.Name) ($lineCount linii) -> $MetaFile"
    }
    catch {
        Write-Error "Błąd przy przetwarzaniu projektu $($proj.Name): $_"
    }
}

# Równoległe przetwarzanie projektów
$jobs = @()
foreach ($proj in $projectsToProcess) {
    $jobs += Start-Job -ScriptBlock { param($p, $folder) Process-Project -proj $p -SolutionFolder $folder } -ArgumentList $proj, $SolutionFolder
}

# Czekanie na zakończenie wszystkich jobów
$jobs | ForEach-Object { Wait-Job $_; Receive-Job $_; Remove-Job $_ }
Write-Host "Wszystkie projekty przetworzone."
