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
			: base(solution, registeredPackageRepositories, null, powerShellHostFactory, addinPath)
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
