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
	/// Tests that the WixDocument.IsWixFileName method correctly detects
	/// Wix document file extensions.
	/// </summary>
	[TestFixture]
	public class WixFileExtensionTests
	{
		[Test]
		public void WxsFile()
		{
			Assert.IsTrue(WixDocument.IsWixFileName("foo.wxs"));
			Assert.IsTrue(WixDocument.IsWixSourceFileName("foo.wxs"));
		}
		
		[Test]
		public void WxsFileUppercase()
		{
			Assert.IsTrue(WixDocument.IsWixFileName(@"src\FOO.WXS"));
			Assert.IsTrue(WixDocument.IsWixSourceFileName(@"src\FOO.WXS"));
		}
		
		[Test]
		public void WxiFile()
		{
			Assert.IsTrue(WixDocument.IsWixFileName("foo.wxi"));
			Assert.IsFalse(WixDocument.IsWixSourceFileName("foo.wxi"));
		}
		
		[Test]
		public void NullFileName()
		{
			Assert.IsFalse(WixDocument.IsWixFileName(null));
			Assert.IsFalse(WixDocument.IsWixSourceFileName(null));
		}
	}
}
