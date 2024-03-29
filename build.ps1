
param(
    [string] $c = "Debug"
)

function rmFile([string] $file) {
    if (Test-Path $file) {
        rm $file
    }
}

function checkExitCode([string] $message) {
    $exitCode = $LastExitCode
    if ($exitCode -ne 0) {
        Write-Host "Error: $message"
        exit $exitCode
    }
}

# clean up
dotnet clean
checkExitCode "Clean failed"

rmFile .\test\bin\Debug\net6.0\pwXt-cli.db
rmFile .\tests\bin\Debug\net6.0\test-log.log

# build sln
dotnet build -c $c
checkExitCode "Build failed"

# run tests
dotnet test .\tests\pwXt-cli.tests\pwXt-cli.Tests.csproj
checkExitCode "Tests failed"