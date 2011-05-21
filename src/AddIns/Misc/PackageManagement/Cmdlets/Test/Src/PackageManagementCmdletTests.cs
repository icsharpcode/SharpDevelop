// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using NUnit.Framework;
using PackageManagement.Cmdlets.Tests.Helpers;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests
{
	[TestFixture]
	public class PackageManagementCmdletTests
	{
		TestablePackageManagementCmdlet cmdlet;
		
		void CreateCmdletWithNullTerminatingError()
		{
			cmdlet = new TestablePackageManagementCmdlet(null);
		}
		
		void CreateCmdlet()
		{
			cmdlet = new TestablePackageManagementCmdlet();
		}
		
		[Test]
		public void ThrowProjectNotOpenTerminatingError_TerminatingErrorIsNull_NullReferenceExceptionIsNotThrown()
		{
			CreateCmdletWithNullTerminatingError();
			
			Assert.DoesNotThrow(() => cmdlet.CallThrowProjectNotOpenTerminatingError());
		}
		
		[Test]
		public void GetEnvironmentPath_MethodCalled_ReturnsEnvPathFromSessionState()
		{
			CreateCmdlet();
			object expectedPath = "Test";
			cmdlet.GetSessionVariableReturnValue = expectedPath;
			
			object path = cmdlet.GetEnvironmentPath();
			
			Assert.AreEqual(expectedPath, path);
		}
		
		[Test]
		public void GetEnvironmentPath_MethodCalled_EnvPathVariableRetrievedFromSessionState()
		{
			CreateCmdlet();
			cmdlet.GetSessionVariableReturnValue = "Test";
			
			cmdlet.GetEnvironmentPath();
			
			string variableName = cmdlet.NamePassedToGetSessionVariable;
			
			Assert.AreEqual("env:path", variableName);
		}
		
		[Test]
		public void SetEnvironmentPath_PathSet_EnvPathVariableUpdated()
		{
			CreateCmdlet();
			cmdlet.SetEnvironmentPath("Test");
			
			string variableName = cmdlet.NamePassedToSetSessionVariable;
			
			Assert.AreEqual("env:path", variableName);
		}
		
		[Test]
		public void SetEnvironmentPath_PathSet_EnvPathUpdated()
		{
			CreateCmdlet();
			cmdlet.SetEnvironmentPath("Test");
			
			object path = cmdlet.ValuePassedToSetSessionVariable;
			
			Assert.AreEqual("Test", path);
		}
		
		[Test]
		public void AddVariable_NameAndValuePassed_VariableWithNameIsUpdated()
		{
			CreateCmdlet();
			cmdlet.AddVariable("Test", "Value");
			
			string variableName = cmdlet.NamePassedToSetSessionVariable;
			
			Assert.AreEqual("Test", variableName);
		}
		
		[Test]
		public void AddVariable_NameAndValuePassed_VariableValueUpdated()
		{
			CreateCmdlet();
			cmdlet.AddVariable("Test", "Value");
			
			object value = cmdlet.ValuePassedToSetSessionVariable;
			
			Assert.AreEqual("Value", value);
		}
		
		[Test]
		public void RemoveVariable_NamePassed_VariableRemoved()
		{
			CreateCmdlet();
			cmdlet.RemoveVariable("Test");
			
			string name = cmdlet.NamePassedToRemoveVariable;
			
			Assert.AreEqual("Test", name);
		}
		
		[Test]
		public void Run_PackageScriptPassed_RunsScriptUsingCmdletAsSession()
		{
			CreateCmdlet();
			
			var script = new FakePackageScript();
			cmdlet.Run(script);
			
			IPackageScriptSession session = script.SessionPassedToRun;
			
			Assert.AreEqual(cmdlet, session);
		}
		
		[Test]
		public void Run_PackageScriptDoesNotExist_ScriptIsNotRun()
		{
			CreateCmdlet();
			
			var script = new FakePackageScript();
			script.ExistsReturnValue = false;
			cmdlet.Run(script);
			
			bool run = script.IsRun;
			
			Assert.IsFalse(run);
		}
	}
}
