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
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Configuration
{
	/// <summary>
	/// Tests the AddInOptions class.
	/// </summary>
	[TestFixture]
	public class AddInOptionsTestFixture
	{
		PythonAddInOptions options;
		Properties properties;
		AddIn addin;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			AddInPathHelper helper = new AddInPathHelper("PythonBinding");
			addin = helper.CreateDummyAddInInsideAddInTree();
			addin.FileName = @"C:\Projects\SD\AddIns\Python\pythonbinding.addin";
		}
		
		[SetUp]
		public void Init()
		{
			properties = new Properties();
			options = new PythonAddInOptions(properties);
		}
		
		[Test]
		public void DefaultPythonConsoleFileNameIsPythonAddInPathCombinedWithIpyExe()
		{
			string expectedFileName = @"C:\Projects\SD\AddIns\Python\ipy.exe";
			Assert.AreEqual(expectedFileName, options.PythonFileName);
		}
		
		[Test]
		public void RequestingPythonFileNameWhenPythonConsoleFileNameSetToNullReturnsDefaultPythonConsoleFileName()
		{
			options.PythonFileName = null;
			string expectedFileName = @"C:\Projects\SD\AddIns\Python\ipy.exe";
			Assert.AreEqual(expectedFileName, options.PythonFileName);
		}
		
		[Test]
		public void RequestingPythonFileNameWhenPythonConsoleFileNameToEmptyStringReturnsDefaultPythonConsoleFileName()
		{
			options.PythonFileName = String.Empty;
			string expectedFileName = @"C:\Projects\SD\AddIns\Python\ipy.exe";
			Assert.AreEqual(expectedFileName, options.PythonFileName);
		}
		
		[Test]
		public void SetPythonConsoleFileNameUpdatesAddInOptionsPythonFileName()
		{
			string fileName = @"C:\IronPython\ipy.exe";
			options.PythonFileName = fileName;
			Assert.AreEqual(fileName, options.PythonFileName);
		}
		
		[Test]
		public void SetPythonConsoleFileNameUpdatesProperties()
		{
			string fileName = @"C:\IronPython\ipy.exe";
			options.PythonFileName = fileName;
			Assert.AreEqual(fileName, properties["PythonFileName"]);
		}
		
		[Test]
		public void PythonLibraryPathTakenFromProperties()
		{
			string expectedPythonLibraryPath = @"c:\python26\lib;c:\python26\lib\lib-tk";
			properties["PythonLibraryPath"] = expectedPythonLibraryPath;
			Assert.AreEqual(expectedPythonLibraryPath, options.PythonLibraryPath);
		}
		
		[Test]
		public void DefaultPythonLibraryPathIsEmptyString()
		{
			Assert.AreEqual(String.Empty, options.PythonLibraryPath);
		}
	}
}
