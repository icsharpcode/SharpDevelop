// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class SettingsProviderTests
	{
		SettingsProvider settingsProvider;
		FakeSettings fakeSettings;
		FakePackageManagementProjectService projectService;
		IFileSystem fileSystemUsedToLoadSettings;
		string configFileUsedToLoadSettings;
		IMachineWideSettings machinesettingsUsedToLoadSettings;
		
		[SetUp]
		public void SetUp()
		{
			fakeSettings = new FakeSettings();
			projectService = new FakePackageManagementProjectService();
			SettingsProvider.LoadDefaultSettings = LoadDefaultSettings;
			settingsProvider = new SettingsProvider(projectService);
		}
		
		ISettings LoadDefaultSettings(IFileSystem fileSystem, string configFile, IMachineWideSettings machineSettings)
		{
			fileSystemUsedToLoadSettings = fileSystem;
			configFileUsedToLoadSettings = configFile;
			machinesettingsUsedToLoadSettings = machineSettings;
			
			return fakeSettings;
		}
		
		void OpenSolution(string fileName)
		{
			var helper = new SolutionHelper(fileName);
			projectService.OpenSolution = helper.MSBuildSolution;
		}
		
		[TearDown]
		public void TearDown()
		{
			// This resets SettingsProvider.LoadDefaultSettings.
			TestablePackageManagementOptions.CreateSettingsProvider(fakeSettings, projectService);
		}
		
		[Test]
		public void LoadSettings_NoSolutionOpen_NullFileSystemAndNullConfigFileAndNullMachineSettingsUsed()
		{
			fileSystemUsedToLoadSettings = new FakeFileSystem();
			configFileUsedToLoadSettings = "configFile";
			
			ISettings settings = settingsProvider.LoadSettings();
			
			Assert.IsNull(fileSystemUsedToLoadSettings);
			Assert.IsNull(configFileUsedToLoadSettings);
			Assert.IsNull(machinesettingsUsedToLoadSettings);
			Assert.AreEqual(fakeSettings, settings);
		}
		
		[Test]
		public void LoadSettings_SolutionOpen_FileSystemWithRootSetToSolutionDotNuGetDirectoryUsedToLoadSettings()
		{
			string fileName = @"d:\projects\MyProject\MyProject.sln";
			OpenSolution(fileName);
			
			ISettings settings = settingsProvider.LoadSettings();
			
			Assert.AreEqual(@"d:\projects\MyProject\.nuget", fileSystemUsedToLoadSettings.Root);
			Assert.AreEqual(fakeSettings, settings);
		}
		
		[Test]
		public void LoadSettings_NuGetSettingsThrowsUnauthorizedAccessException_ExceptionHandledAndSettingsNullObjectReturned()
		{
			fileSystemUsedToLoadSettings = new FakeFileSystem();
			configFileUsedToLoadSettings = "configFile";
			SettingsProvider.LoadDefaultSettings = (fileSystem, configFile, machineSettings) => {
				throw new UnauthorizedAccessException();
			};

			ISettings settings = settingsProvider.LoadSettings();

			Assert.IsInstanceOf<NullSettings>(settings);
		}
	}
}
