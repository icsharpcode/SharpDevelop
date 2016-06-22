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
using ICSharpCode.SharpDevelop;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class MockViewContentTests
	{
		MockViewContent view;
		
		[SetUp]
		public void Init()
		{
			SD.InitializeForUnitTests();
			view = new MockViewContent();
		}
		
		[Test]
		public void SetFileNameMethodCreatesNewPrimaryFile()
		{
			string fileName = @"d:\projects\a.txt";
			view.SetFileName(fileName);
			Assert.AreEqual(fileName, view.PrimaryFile.FileName.ToString());
		}
		
		[Test]
		public void SetFileNameMethodCreatesNewPrimaryFileName()
		{
			string fileName = @"d:\projects\a.txt";
			view.SetFileName(fileName);
			Assert.AreEqual(fileName, view.PrimaryFileName.ToString());
		}
			
		[Test]
		public void SetFileNameMethodCreatesNewPrimaryFileThatIsNotUntitled()
		{
			view.SetFileName(@"d:\test.txt");
			Assert.IsFalse(view.PrimaryFile.IsUntitled);
		}
		
		[Test]
		public void SetUntitledFileNameMethodCreatesNewPrimaryFile()
		{
			string fileName = @"d:\projects\a.txt";
			view.SetUntitledFileName(fileName);
			Assert.AreEqual(fileName, view.PrimaryFile.FileName.ToString());
		}
		
		[Test]
		public void SetUntitledFileNameMethodCreatesNewPrimaryFileName()
		{
			string fileName = @"d:\projects\a.txt";
			view.SetUntitledFileName(fileName);
			Assert.AreEqual(fileName, view.PrimaryFileName.ToString());
		}	
		
		[Test]
		public void SetUntitledFileNameMethodCreatesNewPrimaryFileThatIsUntitled()
		{
			view.SetUntitledFileName(@"d:\test.txt");
			Assert.IsTrue(view.PrimaryFile.IsUntitled);
		}
		
		[Test]
		public void DefaultPrimaryFileNameIsDummyName()
		{
			Assert.AreEqual("dummy.name", view.PrimaryFile.FileName.ToString());
		}
	}
}
