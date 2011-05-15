// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting.Tests.Utils;

namespace PackageManagement.Tests.Helpers
{
	public class TestablePackageInitializationScriptsConsole : PackageInitializationScriptsConsole
	{
		public FakePackageManagementConsoleHost FakeConsoleHost;
		public FakeScriptingConsole FakeScriptingConsole;
		
		public TestablePackageInitializationScriptsConsole()
			: this(new FakePackageManagementConsoleHost())
		{
		}
		
		public TestablePackageInitializationScriptsConsole(FakePackageManagementConsoleHost consoleHost)
			: base(consoleHost)
		{
			FakeConsoleHost = consoleHost;
			FakeScriptingConsole = new FakeScriptingConsole();
			FakeConsoleHost.ScriptingConsole = FakeScriptingConsole;
		}
		
		public bool IsCreateConsolePadCalled;
		
		protected override void CreateConsolePad()
		{
			IsCreateConsolePadCalled = true;
		}
	}
}
