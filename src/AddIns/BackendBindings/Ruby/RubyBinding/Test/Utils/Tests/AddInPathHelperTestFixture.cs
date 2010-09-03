// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class AddInPathHelperTestFixture
	{
		AddIn rubyAddIn;
		
		[SetUp]
		public void Init()
		{
			rubyAddIn = AddInPathHelper.CreateDummyRubyAddInInsideAddInTree();
		}
		
		[Test]
		public void AddInFileNameIsCDriveSharpDevelopAddInsRubyBindingRubyBindingAddIn()
		{
			string expectedFileName = @"C:\SharpDevelop\AddIns\RubyBinding\RubyBinding.addin";
			Assert.AreEqual(expectedFileName, rubyAddIn.FileName);
		}
		
		[Test]
		public void StringParserAddInPathIsCDriveSharpDevelopAddInsRubyBindingForRubyBindingAddIn()
		{
			string directory = StringParser.Parse("${addinpath:ICSharpCode.RubyBinding}");
			string expectedDirectory = @"C:\SharpDevelop\AddIns\RubyBinding";
			Assert.AreEqual(expectedDirectory, directory);
		}
		
		[Test]
		public void ChangingAddInFileNameReturnsExpectedFileNameFromStringParserAddInPath()
		{
			rubyAddIn.FileName = @"c:\def\pythonbinding.addin";
			string expectedDirectory = @"c:\def";
			string actualDirectory = StringParser.Parse("${addinpath:ICSharpCode.RubyBinding}");
			Assert.AreEqual(expectedDirectory, actualDirectory);
		}
	}
}
