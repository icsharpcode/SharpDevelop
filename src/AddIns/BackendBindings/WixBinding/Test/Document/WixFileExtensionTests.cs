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

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests that the WixFileName.IsWixFileName method correctly detects
	/// Wix document file extensions.
	/// </summary>
	[TestFixture]
	public class WixFileExtensionTests
	{
		[Test]
		public void IsWixFileNameReturnsTrueIfFileHasWxsFileExtension()
		{
			Assert.IsTrue(WixFileName.IsWixFileName("foo.wxs"));
		}
		
		[Test]
		public void IsWixSourceFileNameReturnsTrueIfFileHasWxsFileExtension()
		{
			Assert.IsTrue(WixFileName.IsWixSourceFileName("foo.wxs"));
		}
		
		[Test]
		public void IsWixSFileNameReturnsTrueIfWxsFileExtensionIsUppercase()
		{
			Assert.IsTrue(WixFileName.IsWixFileName(@"src\FOO.WXS"));
		}
		
		[Test]
		public void IsWixSourceFileNameReturnsTrueIfWxsFileExtensionIsUppercase()
		{
			Assert.IsTrue(WixFileName.IsWixSourceFileName(@"src\FOO.WXS"));
		}
		
		[Test]
		public void IsWixFileNameReturnsTrueIfFileHasWxiFileExtension()
		{
			Assert.IsTrue(WixFileName.IsWixFileName("foo.wxi"));
		}
		
		[Test]
		public void IsWixFileNameReturnsFalseIfFileHasWxiFileExtension()
		{
			Assert.IsFalse(WixFileName.IsWixSourceFileName("foo.wxi"));
		}
		
		[Test]
		public void IsWixFileNameReturnsFalseIfFileNameIsNull()
		{
			Assert.IsFalse(WixFileName.IsWixFileName(null));
		}
		
		[Test]
		public void IsWixSourceFileNameReturnsFalseIfFileNameIsNull()
		{
			Assert.IsFalse(WixFileName.IsWixSourceFileName(null));
		}
	}
}
