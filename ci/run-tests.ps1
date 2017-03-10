Write-Output "Running tests..."

$commonPath="$PSScriptRoot\..\CloudStorage\test"

dir "$commonPath" | % { dotnet test "$commonPath\$_\$_.csproj" }

Write-Output "Tests done."