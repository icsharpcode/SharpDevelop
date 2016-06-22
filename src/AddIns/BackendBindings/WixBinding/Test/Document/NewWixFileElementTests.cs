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
using ICSharpCode.WixBinding;
using NUnit.Framework;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Checks the filenames created when a new WixFileElement is created.
	/// </summary>
	[TestFixture]
	public class NewWixFileElementTests
	{
		WixDocument doc;
		
		[SetUp]
		public void Init()
		{
			doc = new WixDocument();
			doc.FileName = @"C:\Projects\Setup\Setup.wxs";
		}
		
		[Test]
		public void SubFolderFile()
		{
			WixFileElement fileElement = new WixFileElement(doc, @"C:\Projects\Setup\Files\readme.txt");
			Assert.AreEqual("readme.txt", fileElement.FileName);
			Assert.AreEqual("readme.txt", fileElement.FileName);
			Assert.AreEqual(@"Files\readme.txt", fileElement.Source);
		}

		[Test]
		public void DifferentParentFolderFile()
		{
			WixFileElement fileElement = new WixFileElement(doc, @"C:\Projects\AnotherSetup\Files\readme.txt");
			Assert.AreEqual("readme.txt", fileElement.FileName);
			Assert.AreEqual("readme.txt", fileElement.Id);
			Assert.AreEqual(@"..\AnotherSetup\Files\readme.txt", fileElement.Source);
		}		
	}
}
