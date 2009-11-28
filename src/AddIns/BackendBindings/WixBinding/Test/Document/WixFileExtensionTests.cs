// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
