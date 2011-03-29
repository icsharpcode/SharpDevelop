// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageManagementConsoleHost : IPackageManagementConsoleHost
	{
		public IProject DefaultProject { get; set; }
		public PackageSource ActivePackageSource { get; set; }
		public IScriptingConsole ScriptingConsole { get; set; }
		
		public bool IsDisposeCalled;
		public bool IsClearCalled;
		public bool IsRunCalled;
		
		public void Dispose()
		{
			IsDisposeCalled = true;
		}
		
		public void Clear()
		{
			IsClearCalled = true;
		}
		
		public void Run()
		{
			IsRunCalled = true;
		}
		
		public TestableProject AddFakeDefaultProject()
		{
			var project = ProjectHelper.CreateTestProject();
			DefaultProject = project;
			return project;
		}
		
		public PackageSource AddTestPackageSource()
		{
			var source = new PackageSource("Test");
			ActivePackageSource = source;
			return source;
		}
	}
}
