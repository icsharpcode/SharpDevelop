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
