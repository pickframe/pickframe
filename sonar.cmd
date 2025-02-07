@ECHO OFF 
:: This batch file runs sonarqube
TITLE SonarQube
ECHO Please wait... Runing sonarqube Commands.
ECHO ==========================
ECHO Sonar START
ECHO ============================
dotnet tool install --global dotnet-sonarscanner
dotnet sonarscanner begin /k:"pickframe" /d:sonar.host.url="http://localhost:9000"  /d:sonar.login="sqp_a858c4af72f48d8d1554a08b73229b5fa829eb19" /d:sonar.cs.vscoveragexml.reportsPaths=coverage.xml
ECHO ============================
ECHO BUILD
ECHO ============================
dotnet build ./pickframe.sln --no-incremental
ECHO ============================
ECHO TEST
ECHO ============================
dotnet-coverage collect "dotnet test ./Tests/Tests/Tests.csproj" -f xml -o "coverage.xml"
ECHO ============================
ECHO END
ECHO ============================
dotnet sonarscanner end /d:sonar.login="sqp_a858c4af72f48d8d1554a08b73229b5fa829eb19"
PAUSE