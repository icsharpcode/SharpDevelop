// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
