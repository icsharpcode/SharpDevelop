// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Runtime.Versioning;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PackageUninstallScriptTests
	{
		PackageUninstallScriptFileName scriptFileName;
		FakePackageScriptSession fakeSession;
		PackageUninstallScript script;
		FakeFileSystem fakeFileSystem;
		FakePackage fakePackage;
		FakePackageManagementProject fakeProject;
		
		void CreateScript()
		{
			fakeFileSystem = new FakeFileSystem();
			fakeFileSystem.FileExistsReturnValue = true;
			fakeFileSystem.DirectoryExistsReturnValue = true;
			scriptFileName = new PackageUninstallScriptFileName(fakeFileSystem);
			
			fakeSession = new FakePackageScriptSession();
			fakePackage = new FakePackage();
			
			fakeProject = new FakePackageManagementProject();
			
			script = new PackageUninstallScript(fakePackage, scriptFileName);
			script.Project = fakeProject;
		}
		
		void ExecuteScript()
		{
			script.Run(fakeSession);
		}
		
		[Test]
		public void Run_PackageHasTargetFrameworkSpecificUnistallPowerShellScript_UninstallPowerShellScriptIsRun()
		{
			CreateScript();
			fakeFileSystem.Root = @"d:\projects\MyProject\packages\MyPackage.1.0";
			fakePackage.AddFile(@"tools\net40\uninstall.ps1");
			fakeProject.TargetFramework = new FrameworkName(".NETFramework", new Version("4.0"));
			
			script.Run(fakeSession);
			
			bool result = fakeSession.ScriptPassedToInvokeScript.StartsWith(
				@"& 'd:\projects\MyProject\packages\MyPackage.1.0\tools\net40\uninstall.ps1'");
			Assert.IsTrue(result, fakeSession.ScriptPassedToInvokeScript);
		}
	}
}
