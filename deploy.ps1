# todo
# package /nuget and push to github?
# publish app as exe file

rm -Force -Recurse .\src\pwXt-cli\bin
rm -Force -Recurse .\src\pwXt-cli\obj

dotnet build .\src\pwXt-cli -c Release

if (Test-Path .\publish) {
    rm -Force -Recurse .\publish
}
if (Test-Path .\pwXt-cli-app.zip) {
    rm -Force .\pwXt-cli-app.zip
}

# publish as single file
if (Test-Path .\publish) {
    rm -Force -Recurse .\publish
}

dotnet publish .\src\pwXt-cli -r win-x64 -c Release --self-contained true -o .\publish
mv .\publish .\pwXt-cli
cp .\src\pwXt-cli\appsettings.json .\pwXt-cli\appsettings.json
gci .\pwXt-cli -File -Recurse | Compress-archive -DestinationPath .\pwXt-cli.zip -Force

if (Test-Path .\pwXt-cli) {
    rm -Force -Recurse .\pwXt-cli
}