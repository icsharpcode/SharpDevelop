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
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Debugging;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Gui
{
	[TestFixture]
	public class DebugPythonCommandTestFixture
	{
		MockDebugger debugger;
		RunDebugPythonCommand command;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			MockWorkbench workbench = MockWorkbench.CreateWorkbenchWithOneViewContent(@"C:\Projects\test.py");

			Properties p = new Properties();
			PythonAddInOptions options = new PythonAddInOptions(p);
			options.PythonFileName = @"C:\IronPython\ipy.exe";
		
			debugger = new MockDebugger();
			command = new RunDebugPythonCommand(workbench, options, debugger);
			command.Run();
		}
				
		[Test]
		public void Run_PythonFileOpen_DebuggerStartMethodCalled()
		{
			bool result = debugger.StartMethodCalled;
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Run_PythonFileOpen_IronPythonConsoleFileNamePassedToDebugger()
		{
			string fileName = debugger.ProcessStartInfo.FileName;
			string expectedFileName = @"C:\IronPython\ipy.exe";
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void Run_PythonFileOpen_DebugOptionsPassedToIronPythonConsole()
		{
			string args = debugger.ProcessStartInfo.Arguments;
			string expectedArgs = "-X:Debug \"test.py\"";
			Assert.AreEqual(expectedArgs, args);
		}
	}
}
