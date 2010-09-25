// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
