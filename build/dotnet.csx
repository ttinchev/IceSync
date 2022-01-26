#load "process.csx"
#r "nuget:Newtonsoft.Json, 12.0.2"

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Xml.XPath;
using Newtonsoft.Json.Linq;

public static partial class MM
{
    public static class DotNet
    {
        public static void Build(string pathToProject, string configuration = "Debug") =>
            Shell($"dotnet build -c {configuration} -p:RunCodeAnalysis=false", pathToProject);

        public static void Publish(string pathToProject, string pathToOutput, string configuration = "Release") =>
            Shell($"dotnet publish -c {configuration} -o {pathToOutput} -p:RunCodeAnalysis=false", pathToProject);

        public static void TestWithCoverage(string pathToProject, string configuration = "Debug", string pathToOutput = null) =>
            Shell(
                $"dotnet test -c {configuration} --logger \"trx;LogFileName={pathToOutput}/dotnet.results.trx\" --logger \"junit;LogFilePath={pathToOutput}/dotnet.results.xml;MethodFormat=Class;FailureBodyFormat=Verbose\"" +
                $" /p:RunCodeAnalysis=false /p:CollectCoverage=true /p:CoverletOutputFormat=\\\\\"cobertura,opencover\\\\\" /p:CoverletOutput={pathToOutput}/",
                pathToProject);

        public static void Test(string pathToProject, string configuration = "Debug", string pathToOutput = null) =>
            Shell($"dotnet test {GetTestArgs(configuration, pathToOutput)}", pathToProject);

        private static string GetTestArgs(string configuration, string pathToOutput) =>
            "--logger trx -c " + configuration + (pathToOutput == null ? string.Empty : $" -r {pathToOutput}");
    }

    public record EnvValue(string Key, string Value);

    public static class Transform
    {
        public static void TransformSettingsJson(string pathToSettingsJson, string pathToTransformJson)
        {
            WriteLine("Transforming file " + pathToSettingsJson);
            var sourceJson = ReadJsonFile(pathToSettingsJson);
            var transformJson = ReadJsonFile(pathToTransformJson);
            var mergeSettigns = new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Replace };

            sourceJson.Merge(transformJson);

            var transformedContent = sourceJson.ToString();
            WriteLine("Transformed file content " + transformedContent, LogLevel.Debug);

            File.WriteAllText(pathToSettingsJson, transformedContent);
        }

        public static string FindProperyValueInJson(string pathToJson, string propertyName)
        {
            var sourceJson = ReadJsonFile(pathToJson) as JToken;
            var paths = propertyName.Split('.');
            foreach (var path in paths)
            {
                if (sourceJson == null)
                {
                    return null;
                }

                sourceJson = sourceJson[path];
            }

            return (string)sourceJson;
        }

        public static string GetXmlXPathValue(string pathToXml, string xpath)
        {
            WriteLine($"Get xpath {xpath} from {pathToXml}.", LogLevel.Info);
            var xml = XElement.Load(pathToXml);
            return xml.XPathSelectElement(xpath)?.Value;
        }

        public static void ReplaceInFile(string pathToFile, string pathToNewFile, params EnvValue[] values)
        {
            var content = File.ReadAllText(pathToFile);
            string transformedContent = content;
            foreach (var env in values)
            {
                transformedContent = transformedContent.Replace(env.Key, env.Value);
            }

            File.WriteAllText(pathToNewFile ?? pathToFile, transformedContent);
        }

        private static JObject ReadJsonFile(string pathToSettingsJson) =>
            JObject.Parse(File.ReadAllText(pathToSettingsJson));
    }

    public static class Sonarqube
    {
        public static void TransformGlobalSettings(string sonarScannerDirectory, string pathToTools, string sdk, params EnvValue[] values)
        {
            var pathsToSettings = Directory.GetFiles(sonarScannerDirectory, "SonarQube.Analysis.xml", SearchOption.AllDirectories);

            WriteLine($"Found {pathsToSettings.Length} SonarQube.Analysis.xml searching sdk {sdk}.", LogLevel.Info);

            var pathToMySettings = pathsToSettings.First(p => p.Contains(sdk));
            var pathToSonarqube = Path.Combine(pathToTools, "sonarqube.xml");
            var pathToTransformedSonarqube = Path.Combine(pathToTools, "SonarQube.Analysis.xml");

            if (!File.Exists(pathToMySettings))
            {
                WriteLine($"File SonarQube.Analysis.xml was not found for sdk {sdk}.", LogLevel.Error);
                return;
            }

            WriteLine("Modify tools/sonarqube.xml add base path.", LogLevel.Info);
            Transform.ReplaceInFile(pathToSonarqube, pathToTransformedSonarqube, values);
            
            WriteLine("Copy tools/sonarqube.xml to sonarqube root location " + pathToMySettings, LogLevel.Info);
            File.Copy(pathToTransformedSonarqube, pathToMySettings, true);
        }

        public static void SetNodePath()
        {
            var pathToGlobalNodeModules = Shell("npm root -g");
            
            WriteLine("Set env variable NODE_PATH to " + pathToGlobalNodeModules, LogLevel.Info);
            Environment.SetEnvironmentVariable("NODE_PATH", pathToGlobalNodeModules);
        }
    }
}