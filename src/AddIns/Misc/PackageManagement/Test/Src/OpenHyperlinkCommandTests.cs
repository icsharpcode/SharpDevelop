// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using PackageManagement.Tests.Helpers;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class OpenHyperlinkCommandTests
	{
		TestableOpenHyperlinkCommand command;
		
		void CreateOpenHyperlinkCommand()
		{
			command = new TestableOpenHyperlinkCommand();
		}
		
		[Test]
		public void CanExecute_NullParameterPassed_ReturnsTrue()
		{
			CreateOpenHyperlinkCommand();
			bool result = command.CanExecute(null);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Execute_HyperlinkStringPassedAsParameter_StartProcessPassedHyperlink()
		{
			CreateOpenHyperlinkCommand();
			string hyperlink = "http://sharpdevelop.com";
			command.Execute(hyperlink);
			
			Assert.AreEqual(hyperlink, command.FileNamePassedToStartProcessMethod);
		}
		
		[Test]
		public void Execute_UriPassedAsParameter_StartProcessPassedHyperlink()
		{
			CreateOpenHyperlinkCommand();
			string hyperlink = "http://sharpdevelop.com/";
			Uri uri = new Uri(hyperlink);
			command.Execute(uri);
			
			Assert.AreEqual(hyperlink, command.FileNamePassedToStartProcessMethod);
		}
	}
}
