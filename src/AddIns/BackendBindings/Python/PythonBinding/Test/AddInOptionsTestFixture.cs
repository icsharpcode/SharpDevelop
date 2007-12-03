// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests
{
	/// <summary>
	/// Tests the AddInOptions class.
	/// </summary>
	[TestFixture]
	public class AddInOptionsTestFixture
	{
		[Test]
		public void DefaultPythonConsoleFileName()
		{
			Properties p = new Properties();
			DerivedAddInOptions options = new DerivedAddInOptions(p);
			options.AddInPath = @"C:\Projects\SD\AddIns\Python";

			string expectedFileName = Path.Combine(options.AddInPath, "ipy.exe");
			Assert.AreEqual(expectedFileName, options.PythonFileName);
			Assert.AreEqual("${addinpath:ICSharpCode.PythonBinding}", options.AddInPathRequested);
		}
				
		[Test]
		public void SetPythonConsoleFileNameToNull()
		{
			Properties p = new Properties();
			DerivedAddInOptions options = new DerivedAddInOptions(p);
			options.AddInPath = @"C:\Projects\SD\AddIns\Python";
			options.PythonFileName = null;

			string expectedFileName = Path.Combine(options.AddInPath, "ipy.exe");
			Assert.AreEqual(expectedFileName, options.PythonFileName);
		}
		
		[Test]
		public void SetPythonConsoleFileNameToEmptyString()
		{
			Properties p = new Properties();
			DerivedAddInOptions options = new DerivedAddInOptions(p);
			options.AddInPath = @"C:\Projects\SD\AddIns\Python";
			options.PythonFileName = String.Empty;
			
			string expectedFileName = Path.Combine(options.AddInPath, "ipy.exe");
			Assert.AreEqual(expectedFileName, options.PythonFileName);
		}
		
		[Test]
		public void SetPythonConsoleFileName()
		{
			Properties p = new Properties();
			AddInOptions options = new AddInOptions(p);
			string fileName = @"C:\IronPython\ipy.exe";
			options.PythonFileName = fileName;
			
			Assert.AreEqual(fileName, options.PythonFileName);
			Assert.AreEqual(fileName, p["PythonFileName"]);
		}
	}
}
