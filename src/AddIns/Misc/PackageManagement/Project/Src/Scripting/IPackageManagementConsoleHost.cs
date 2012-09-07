// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public interface IPackageManagementConsoleHost : IDisposable
	{
		IProject DefaultProject { get; set; }
		PackageSource ActivePackageSource { get; set; }
		IScriptingConsole ScriptingConsole { get; set; }
		IPackageManagementSolution Solution { get; }
		bool IsRunning { get; }
		
		void Clear();
		void WritePrompt();
		void Run();
		void ShutdownConsole();
		void ExecuteCommand(string command);
		
		void SetDefaultRunspace();
		
		IPackageManagementProject GetProject(string packageSource, string projectName);
		IPackageManagementProject GetProject(IPackageRepository sourceRepository, string projectName);
		PackageSource GetActivePackageSource(string source);
		
		IPackageRepository GetPackageRepository(PackageSource packageSource);
	}
}
