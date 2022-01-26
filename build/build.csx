#! "net5.0"
#load "dotnet.csx"

using System.Net;
using static MM;

var projectName = "Humate";
var masterBranch = "production";
var projectApiName = projectName + ".Presentation.Api";
var solutionDir = Path.GetFullPath(Path.Combine(GetScriptFolder(), ".."));
var pathToApi = Path.Combine(solutionDir, projectApiName);
var pathToTests = Path.Combine(solutionDir, projectName + ".Tests");
var pathToTools = Path.Combine(solutionDir, "tools");
var pathToTestResult = Path.Combine(solutionDir, "TestResults");
var pathToApiProject = Path.Combine(pathToApi, projectApiName + ".csproj");
var pathToOut = Path.Combine(solutionDir, "publish", "out");
var pathToZip = Path.Combine(solutionDir, "publish", "app.zip");

Action("api-build", () => DotNet.Build(pathToApi, "Release"), "Build Api for Release");
Action("api-test", () => DotNet.TestWithCoverage(pathToTests, "Debug", pathToTestResult), "Unit tests");
Action("api-publish-build", () => DotNet.Publish(pathToApi, pathToOut), "Publish Api Application Build");
Action(
    "api-publish-transform",
    () =>
    {
        if (Globals.Args.Has("settings") &&
            !string.IsNullOrEmpty(Globals.Args["settings"]))
        {
            Transform.TransformSettingsJson(Path.Combine(pathToApi, "appsettings.json"), Path.Combine(solutionDir, Globals.Args["settings"]));
        }
        else
        {
            WriteLine("No settings");
        }
    },
    "Publish Api Application Transform appsettings");

Action("api-publish-migrate", 
    () =>
    {
        var paths = Path.Combine(solutionDir, Globals.Args["secrets"]);

        Shell("dotnet tool install --global dotnet-ef", solutionDir);
        Shell($"cat {paths} | dotnet user-secrets set", pathToApi);
        Shell("export PATH=$PATH:/root/.dotnet/tools && dotnet ef database update", pathToApi);
    },
    "Publish Apply Db Migrations");

Action(
    "api-publish-upload",
    () => 
    {
        if (File.Exists(pathToZip))
        {
            File.Delete(pathToZip);
        }

        System.IO.Compression.ZipFile.CreateFromDirectory(pathToOut, pathToZip);
        Shell($"curl -X POST -u '{Globals.Args["user"]}:{Globals.Args["pass"]}' --data-binary @'{pathToZip}' {Globals.Args["deploy-url"]}/api/zipdeploy", solutionDir);
    },
    "Publish Api Application Upload");
    
Action("api-publish", () => {
    Run("api-publish-transform");
    Run("api-publish-build");
    Run("api-publish-upload");
}, "Publish Api Application");

Action("sonarqube", () => {
    ShellInstall("dotnet sonarscanner --version", "dotnet tool install -g dotnet-sonarscanner");

    var version = Transform.GetXmlXPathValue(pathToApiProject, "/PropertyGroup/Version");
    var sdk = Transform.GetXmlXPathValue(pathToApiProject, "/PropertyGroup/TargetFramework");
    var branch = Globals.Args["branch"] == masterBranch ? string.Empty : $"/d:sonar.branch.name={Globals.Args["branch"]}";

    Sonarqube.SetNodePath();
    Sonarqube.TransformGlobalSettings(
        "/root/.dotnet/tools/.store/dotnet-sonarscanner",
         pathToTools,
         sdk,
         new EnvValue("$BASE_PATH", solutionDir),
         new EnvValue("$LOGIN_KEY", Globals.Args["sonar-key"]));

    WriteLine("Install typescript 3.7.5, if not exists. For eslint analysis.", LogLevel.Info);
    ShellInstall("tsc --version", "npm install -g typescript@3.7.5");

    WriteLine("Start sonarscanner analysis on branch " + Globals.Args["branch"], LogLevel.Info);
    Shell($"dotnet sonarscanner begin /k:humate /v:{version} {branch} /d:sonar.nodejs.executable=/usr/bin/node", solutionDir);
    Shell("dotnet build /p:RunCodeAnalysis=false", solutionDir);
    Shell($"dotnet sonarscanner end", solutionDir);
}, "SonarQube Analysis");
Run();
