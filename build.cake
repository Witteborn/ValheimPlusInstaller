///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Publish");
var configuration = Argument("configuration", "Release");
var solutionName = "ValheimPlusInstaller";
var solutionFolder = "./";
var outputFolder = "./artifacts";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
	// Executed BEFORE the first task.
	Information("Running tasks...");
});

Teardown(ctx =>
{
	// Executed AFTER the last task.
	Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
	.Does(() =>
{
	CleanDirectory($"./{solutionName}/bin/{configuration}");
	CleanDirectory(outputFolder);
});

Task("Restore")
	.Does(()=>{
		DotNetCoreRestore(solutionFolder);
});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.Does(() =>
{
	DotNetCoreBuild(solutionFolder, new DotNetCoreBuildSettings
	{
		NoRestore = true,
		Configuration = configuration,
	});
});

Task("Test")
	.IsDependentOn("Build")
	.Does(() =>
{
	DotNetCoreTest(solutionFolder, new DotNetCoreTestSettings
	{
		NoBuild = true,
		NoRestore = true,
		Configuration = configuration,
	});
});

Task("Publish")
	.IsDependentOn("Test")
	.Does(()=>
{

	string[] runtimes = {"win-x86","win-x64","win-arm","osx-x64","linux-x64","linux-arm"};

	foreach (var runtime in runtimes)
	{
		var runtimeFolder = outputFolder + $"/{runtime}";

		Information($"\nRuntime: {runtime}");

		var settings = new DotNetCorePublishSettings
		{
			OutputDirectory = runtimeFolder,
			Configuration = configuration,

			Framework = "net5.0",
			Runtime = runtime,
			SelfContained = true,
			PublishSingleFile = true,
		};

		DotNetCorePublish(solutionFolder, settings);
		DeleteFile($"{runtimeFolder}/{solutionName}.pdb");
		Zip(runtimeFolder, $"{outputFolder}/{solutionName}_{runtime}.zip");
		DeleteDirectory(runtimeFolder, new DeleteDirectorySettings {
    		Recursive = true,
    		Force = true
		});
	}
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);