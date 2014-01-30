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
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Testing
{
	[TestFixture]
	public class PythonConsoleApplicationTests
	{
		PythonConsoleApplication app;
		PythonAddInOptions options;
		
		[SetUp]
		public void Init()
		{
			options = new PythonAddInOptions(new Properties());
			options.PythonFileName = @"C:\IronPython\ipy.exe";
			app = new PythonConsoleApplication(options);
		}
		
		[Test]
		public void FileName_NewInstance_FileNameIsPythonFileNameFromAddInOptions()
		{
			string fileName = app.FileName;
			string expectedFileName = @"C:\IronPython\ipy.exe";
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetArguments_DebugIsTrue_ReturnsDebugOption()
		{
			app.Debug = true;
			string args = app.GetArguments();
			string expectedCommandLine = "-X:Debug";
			
			Assert.AreEqual(expectedCommandLine, args);
		}
		
		[Test]
		public void GetArguments_ScriptFileNameSet_ReturnsQuotedPythonScriptFileName()
		{
			app.ScriptFileName = @"d:\projects\my ipy\test.py";
			string args = app.GetArguments();
			string expectedCommandLine = "\"d:\\projects\\my ipy\\test.py\"";
			
			Assert.AreEqual(expectedCommandLine, args);
		}
		
		[Test]
		public void GetArguments_ScriptFileNameAndScriptCommandLineArgsSet_ReturnsQuotedPythonScriptFileNameAndItsCommandLineArguments()
		{
			app.Debug = true;
			app.ScriptFileName = @"d:\projects\my ipy\test.py";
			app.ScriptCommandLineArguments = "@responseFile.txt -def";
			string args = app.GetArguments();
			string expectedCommandLine =
				"-X:Debug \"d:\\projects\\my ipy\\test.py\" @responseFile.txt -def";
			
			Assert.AreEqual(expectedCommandLine, args);
		}
		
		[Test]
		public void GetProcessStartInfo_NewInstance_ReturnsProcessStartInfoWithFileNameThatEqualsIronPythonConsoleApplicationExeFileName()
		{
			ProcessStartInfo startInfo = app.GetProcessStartInfo();
			string expectedFileName = @"C:\IronPython\ipy.exe";
			
			Assert.AreEqual(expectedFileName, startInfo.FileName);
		}
		
		[Test]
		public void GetProcessStartInfo_DebugIsTrue_ReturnsProcessStartInfoWithDebugFlagSetInArguments()
		{
			app.Debug = true;
			ProcessStartInfo startInfo = app.GetProcessStartInfo();
			string expectedCommandLine = "-X:Debug";
			
			Assert.AreEqual(expectedCommandLine, startInfo.Arguments);
		}
		
		[Test]
		public void GetProcessStartInfo_WorkingDirectorySet_ReturnsProcessStartInfoWithMatchingWorkingDirectory()
		{
			app.WorkingDirectory = @"d:\temp";
			ProcessStartInfo startInfo = app.GetProcessStartInfo();
			string expectedDirectory = @"d:\temp";
			Assert.AreEqual(expectedDirectory, startInfo.WorkingDirectory);
		}
		
		[Test]
		public void GetProcessStartInfo_ChangingPythonOptionsFileName_ProcessStartInfoFileNameMatchesChange()
		{
			options.PythonFileName = @"d:\temp\test\ipy.exe";
			ProcessStartInfo startInfo = app.GetProcessStartInfo();
			string expectedFileName = @"d:\temp\test\ipy.exe";
			Assert.AreEqual(expectedFileName, startInfo.FileName);
		}
	}
}
