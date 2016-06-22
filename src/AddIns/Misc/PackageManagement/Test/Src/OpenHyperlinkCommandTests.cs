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
