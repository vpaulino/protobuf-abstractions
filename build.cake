#addin nuget:https://www.nuget.org/api/v2/?package=Cake.DoInDirectory
// #addin nuget:https://www.nuget.org/api/v2/?package=Cake.FileHelpers

//////////////////////////////////////////////////////////////////////
// CONFIGURATION
//////////////////////////////////////////////////////////////////////

const string TESTER_SERVICE_INTEGRATION_TESTS = "integration-tester";

var PROJECTS_TO_PACK = new List<string>
{
    "Protobuf.Schemas",
};

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Build-AND-Test");
var configuration = Argument("configuration", "Release");
var nugetPreReleaseTag = Argument("nugetPreReleaseTag", "dev");


//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var outputDir = Directory("Output/");
var artifactsDir = outputDir + Directory("Artifacts/");
var nugetPackagesDir = artifactsDir + Directory("NuGets/");
var preReleaseNugetPackagesDir = nugetPackagesDir + Directory("PreRelease/");
var releaseNugetPackagesDir = nugetPackagesDir + Directory("Release/");
var integrationTestResultsOutputDir = outputDir + Directory("IntegrationTestsResults/");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDir);
        CleanDirectory(integrationTestResultsOutputDir);
    });

Task("Restore-NuGet-Packages")
    .Does(() =>
    {
        DotNetCoreRestore("ProtobufAbstractions.sln");
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        var dotNetBuildConfig = new DotNetCoreBuildSettings() {
            Configuration = configuration,
            MSBuildSettings = new DotNetCoreMSBuildSettings()
        };
        
       // dotNetBuildConfig.MSBuildSettings.TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error;

        DotNetCoreBuild("ProtobufAbstractions.sln", dotNetBuildConfig);
    });
    
Task("Nuget-Pack")
    .Does(()=>{
        EnsureDirectoryExists(preReleaseNugetPackagesDir);
        EnsureDirectoryExists(releaseNugetPackagesDir);
        
        CleanDirectory(preReleaseNugetPackagesDir);
        CleanDirectory(releaseNugetPackagesDir);
                
        var preReleaseSettings = new DotNetCorePackSettings{
            Configuration = configuration,
            OutputDirectory = preReleaseNugetPackagesDir,
            VersionSuffix = nugetPreReleaseTag
        };
        var releaseSettings = new DotNetCorePackSettings{
            Configuration = configuration,
            OutputDirectory = releaseNugetPackagesDir
        };


        // https://github.com/NuGet/Home/issues/4337
        // While this issue is not fixed we need to specify the version suffix in the restore task.

        Action<DotNetCorePackSettings> packProjects = (settings) => {
            var dotnetCoreRestoreSettings = new DotNetCoreRestoreSettings();
            if (settings.VersionSuffix != null) {
                dotnetCoreRestoreSettings.EnvironmentVariables = new Dictionary<string, string>()
                {
                    { "VersionSuffix", settings.VersionSuffix }
                };
            }
                
            foreach(var project in PROJECTS_TO_PACK){
                var projectFolder = "./src/" + project;
                DotNetCoreRestore(projectFolder, dotnetCoreRestoreSettings);
                DotNetCorePack(projectFolder, settings);
            }
        };

        packProjects(preReleaseSettings);
        packProjects(releaseSettings);
    });
    
Task("Run-Unit-Tests")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        
        var files = GetFiles("./tests/**/*.csproj");

        int highestExitCode = 0;

        foreach (var file in files){
            // While https://github.com/cake-build/cake/pull/1578 is not merged, 
            // we need to start our own process to run dotnet xunit.
            // Related: https://github.com/cake-build/cake/issues/1577

            var processSettings = new ProcessSettings { 
                Arguments = "xunit -trait \"TestCategory=\"Unit\"\"",
                WorkingDirectory = file.GetDirectory()
            };

            if (IsRunningOnUnix()) {
                var frameworks = XmlPeek(file, "/Project/PropertyGroup/TargetFrameworks/text()");
                if (frameworks == null)
                    frameworks = XmlPeek(file, "/Project/PropertyGroup/TargetFramework/text()");

                if (frameworks == null || frameworks.Contains("netcoreapp") == false) {
                    continue;
                }
                processSettings.Arguments.Append("-framework netcoreapp1.0");
            }

            var exitCode = StartProcess("dotnet", processSettings);

            if(exitCode > highestExitCode)
                highestExitCode = exitCode;
        }
        
        // Means there was an error
        if(highestExitCode > 0 )
            throw new Exception("Some tests failed.");
    });
 

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////


Task("Build-AND-Test")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests");


Task("TravisCI")
    .IsDependentOn("Build-AND-Test");
    

Task("Default")
    .IsDependentOn("Build-AND-Test");
    
//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);