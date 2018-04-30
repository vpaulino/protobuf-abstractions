#addin nuget:https://www.nuget.org/api/v2/?package=Cake.DoInDirectory
// #addin nuget:https://www.nuget.org/api/v2/?package=Cake.FileHelpers
#tool "nuget:?package=xunit.runner.console"


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
var buildNumber =  Argument<int>("buildNumber",0);
var versionSufix = "1.0.0";
var pre = HasArgument("pre");
var sourcePath = Argument<string>("sourcePath","./src");
var solutionName = "Abstractions.sln";

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
        DotNetCoreRestore(solutionName);
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

        DotNetCoreBuild(solutionName, dotNetBuildConfig);
    });
    
Task("NugetPack")
 .DoesForEach(GetFiles($"{sourcePath}/**/*.csproj"), (file) => 
    {
            var versionXPath = "/Project/PropertyGroup/VersionPrefix";

            var versionSufix = XmlPeek(file.FullPath, versionXPath + "/text()");

              var publishFolder =  releaseNugetPackagesDir ;

           if(pre)
           {
               // versionSufix = $"{versionSufix}-pre{buildNumber}";
                publishFolder =  preReleaseNugetPackagesDir ;
           }

          
              var settings = new DotNetCorePackSettings
            {
                Configuration = configuration,
                OutputDirectory =  publishFolder,
                NoDependencies = false,
                NoRestore = true,
               // VersionSuffix = versionSufix

            };
      
            DotNetCorePack(file.FullPath, settings);                 
             
});
    
Task("Run-Unit-Tests")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        
        var files = GetFiles("./tests/**/*Tests.csproj");

        int highestExitCode = 0;

        foreach (var file in files){

            DotNetCoreTest(
                file.FullPath,
                new DotNetCoreTestSettings()
                {
                    Configuration = "Release",
                    //NoBuild = true
                });
            
        }
         
    });
 

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////
Task("Full-Build")
    .IsDependentOn("Build-AND-Test")
    .IsDependentOn("NugetPack");

Task("Build-AND-Test")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests");


Task("TravisCI")
    .IsDependentOn("Full-Build");
    

Task("Default")
    .IsDependentOn("Build-AND-Test");
    
//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);