Write-Output "Running tests..."

$commonTestPath="$PSScriptRoot\..\CloudStorage\test"
$dotnetPath="$PSScriptRoot\Tools\dotnet.exe"

dir "$commonTestPath" | % { &"$dotnetPath" test "$commonTestPath\$_\$_.csproj" }

Write-Output "Tests done."