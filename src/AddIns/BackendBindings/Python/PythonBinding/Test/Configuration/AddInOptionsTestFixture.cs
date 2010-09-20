// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
