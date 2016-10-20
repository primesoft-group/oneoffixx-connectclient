var target = Argument("target", "Default");

public string GetAssemblyVersion()
{
	var assemblyInfo = ParseAssemblyInfo("./src/OneOffixx.ConnectClient.WinApp/Properties/AssemblyInfo.cs");
	return assemblyInfo.AssemblyVersion.ToString();
}

Task("Restore-NuGet-Packages")
	.Does(() => 
	{
		NuGetRestore("OneOffixx.ConnectClient.sln");
	});

Task("Build")
	.IsDependentOn("Restore-NuGet-Packages")
	.Does(() =>
	{
		MSBuild("src/OneOffixx.ConnectClient.WinApp/OneOffixx.ConnectClient.WinApp.csproj", settings => 
			settings.SetConfiguration("Release"));
			
		EnsureDirectoryExists("./artifacts/");
			
		Zip("./src/OneOffixx.ConnectClient.WinApp/bin/Release", "./artifacts/OneOffixx.ConnectClient_"+ GetAssemblyVersion() + ".zip");
	});

Task("Default").IsDependentOn("Build");

RunTarget(target);