// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
