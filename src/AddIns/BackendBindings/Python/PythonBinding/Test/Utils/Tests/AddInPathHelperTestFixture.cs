// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class AddInPathHelperTestFixture
	{
		AddIn pythonAddIn;
		
		[SetUp]
		public void Init()
		{
			pythonAddIn = AddInPathHelper.CreateDummyPythonAddInInsideAddInTree();
		}
		
		[Test]
		public void AddInFileNameIsCDriveSharpDevelopAddInsPythonBindingPythonBindingAddIn()
		{
			string expectedFileName = @"C:\SharpDevelop\AddIns\PythonBinding\PythonBinding.addin";
			Assert.AreEqual(expectedFileName, pythonAddIn.FileName);
		}
		
		[Test]
		public void StringParserAddInPathIsCDriveSharpDevelopAddInsPythonBindingForPythonBindingAddIn()
		{
			string directory = StringParser.Parse("${addinpath:ICSharpCode.PythonBinding}");
			string expectedDirectory = @"C:\SharpDevelop\AddIns\PythonBinding";
			Assert.AreEqual(expectedDirectory, directory);
		}
		
		[Test]
		public void ChangingAddInFileNameReturnsExpectedFileNameFromStringParserAddInPath()
		{
			pythonAddIn.FileName = @"c:\def\pythonbinding.addin";
			string expectedDirectory = @"c:\def";
			string actualDirectory = StringParser.Parse("${addinpath:ICSharpCode.PythonBinding}");
			Assert.AreEqual(expectedDirectory, actualDirectory);
		}
	}
}
