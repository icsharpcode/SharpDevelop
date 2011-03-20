// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting.Tests.Utils;

namespace PackageManagement.Tests.Helpers
{
	public class TestablePackageManagementConsoleHost : PackageManagementConsoleHost
	{
		public FakeScriptingConsoleWithLinesToRead FakeScriptingConsole;
		public FakeThread FakeThread = new FakeThread();
		public ThreadStart ThreadStartPassedToCreateThread;
		public FakePowerShellHostFactory FakePowerShellHostFactory;
		
		public TestablePackageManagementConsoleHost()
			: this(new FakeScriptingConsoleWithLinesToRead(), new FakePowerShellHostFactory())
		{
		}
		
		public TestablePackageManagementConsoleHost(
			FakeScriptingConsoleWithLinesToRead scriptingConsole,
			FakePowerShellHostFactory powerShellHostFactory)
			: base(powerShellHostFactory)
		{
			this.FakeScriptingConsole = scriptingConsole;
			this.ScriptingConsole = scriptingConsole;
			this.FakePowerShellHostFactory = powerShellHostFactory;
		}
		
		protected override IThread CreateThread(ThreadStart threadStart)
		{
			ThreadStartPassedToCreateThread = threadStart;
			return FakeThread;
		}
	}
}
