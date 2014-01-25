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
		
		IConsoleHostFileConflictResolver CreateFileConflictResolver(FileConflictAction fileConflictAction);
		
		IPackageManagementProject GetProject(string packageSource, string projectName);
		IPackageManagementProject GetProject(IPackageRepository sourceRepository, string projectName);
		PackageSource GetActivePackageSource(string source);
		
		IPackageRepository GetPackageRepository(PackageSource packageSource);
	}
}
