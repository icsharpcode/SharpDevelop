// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using ICSharpCode.PackageManagement.Design;
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
		public FakePackageManagementAddInPath FakePackageManagementAddInPath;
		public FakePackageManagementSolution FakeSolution;
		public FakeRegisteredPackageRepositories FakeRegisteredPackageRepositories;
		public Version NuGetVersionToReturn;
		
		public TestablePackageManagementConsoleHost()
			: this(
				new FakePackageManagementSolution(),
				new FakeRegisteredPackageRepositories(),
				new FakeScriptingConsoleWithLinesToRead(),
				new FakePowerShellHostFactory(),
				new FakePackageManagementAddInPath())
		{
		}
		
		public TestablePackageManagementConsoleHost(
			FakePackageManagementSolution solution,
			FakeRegisteredPackageRepositories registeredPackageRepositories,
			FakeScriptingConsoleWithLinesToRead scriptingConsole,
			FakePowerShellHostFactory powerShellHostFactory,
			FakePackageManagementAddInPath addinPath)
			: base(solution, registeredPackageRepositories, powerShellHostFactory, addinPath)
		{
			this.FakeSolution = solution;
			this.FakeScriptingConsole = scriptingConsole;
			this.ScriptingConsole = scriptingConsole;
			this.FakePowerShellHostFactory = powerShellHostFactory;
			this.FakeRegisteredPackageRepositories = registeredPackageRepositories;
			this.FakePackageManagementAddInPath = addinPath;
		}
		
		protected override IThread CreateThread(ThreadStart threadStart)
		{
			ThreadStartPassedToCreateThread = threadStart;
			return FakeThread;
		}
		
		public string TextToReturnFromGetHelpInfo = String.Empty;
		
		protected override string GetHelpInfo()
		{
			return TextToReturnFromGetHelpInfo;
		}
		
		protected override Version GetNuGetVersion()
		{
			return NuGetVersionToReturn;
		}
	}
}
