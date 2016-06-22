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
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NUnit.Framework;
using PackageManagement.Cmdlets.Tests.Helpers;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests
{
	[TestFixture]
	public class InvokeProcessPackageActionsCmdletTests
	{
		TestableInvokeProcessPackageActionsCmdlet cmdlet;
		PackageActionsToRun actionsToRun;		
		
		void CreateCmdlet()
		{
			cmdlet = new TestableInvokeProcessPackageActionsCmdlet();
			actionsToRun = cmdlet.ActionsToRun;
		}
		
		void RunCmdlet()
		{
			cmdlet.CallProcessRecord();
		}
		
		FakeInstallPackageAction AddInstallAction()
		{
			var project = new FakePackageManagementProject();
			var action = new FakeInstallPackageAction(project);
			actionsToRun.AddAction(action);
			return action;
		}
		
		[Test]
		public void ProcessRecord_OnePackageActionToRun_PackageActionIsRun()
		{
			CreateCmdlet();
			FakeInstallPackageAction action = AddInstallAction();
			RunCmdlet();
			
			bool run = action.IsExecuteCalled;
			
			Assert.IsTrue(run);
		}
		
		[Test]
		public void ProcessRecord_TwoPackageActionsToRun_SecondPackageActionIsRun()
		{
			CreateCmdlet();
			AddInstallAction();
			FakeInstallPackageAction action = AddInstallAction();
			RunCmdlet();
			
			bool run = action.IsExecuteCalled;
			
			Assert.IsTrue(run);
		}
		
		[Test]
		public void ProcessRecord_OnePackageActionToRun_PackageActionScriptRunnerIsCmdlet()
		{
			CreateCmdlet();
			FakeInstallPackageAction action = AddInstallAction();
			RunCmdlet();
			
			IPackageScriptRunner runner = action.PackageScriptRunner;
			
			Assert.AreEqual(cmdlet, runner);
		}
		
		[Test]
		public void ProcessRecord_TwoPackageActionsToRun_TwoPackageActionScriptRunnerIsCmdlet()
		{
			CreateCmdlet();
			AddInstallAction();
			FakeInstallPackageAction action = AddInstallAction();
			RunCmdlet();
			
			IPackageScriptRunner runner = action.PackageScriptRunner;
			
			Assert.AreEqual(cmdlet, runner);
		}
		
		[Test]
		public void ProcessRecord_NoPackageActionsToRun_NullReferenceExceptionIsNotThrown()
		{
			CreateCmdlet();
			Assert.DoesNotThrow(() => RunCmdlet());
		}
	}
}
