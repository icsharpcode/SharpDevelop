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
		IPackageManagementProjectService ProjectService { get; }
		
		IEnumerable<IProject> GetOpenProjects();
		
		void Clear();
		void Run();
		
		IPackageManagementProject GetProject(string packageSource, string projectName);
		IPackageManagementProject GetProject(IPackageRepository sourceRepository, string projectName);
		PackageSource GetActivePackageSource(string source);
	}
}
