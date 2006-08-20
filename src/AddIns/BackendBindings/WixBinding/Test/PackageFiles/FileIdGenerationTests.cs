// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class FileIdGenerationTests
	{
		[Test]
		public void ShortFileName()
		{
			string fileName = "test.txt";
			Assert.AreEqual(fileName, WixFileElement.GenerateId(fileName));
		}
		
		[Test]
		public void NullFileName()
		{
			Assert.AreEqual(String.Empty, WixFileElement.GenerateId(null));
		}
		
		
		[Test]
		public void TruncatedShortFileName()
		{
			string fileName = "TESTAP~1.TXT";
			Assert.AreEqual("TESTAP_1.TXT", WixFileElement.GenerateId(fileName));
		}

		[Test]
		public void FirstCharIsDigit()
		{
			string fileName = "8ESTAP.TXT";
			Assert.AreEqual("_ESTAP.TXT", WixFileElement.GenerateId(fileName));
		}
		
		[Test]
		public void FirstCharIsDot()
		{
			string fileName = ".ESTAP.TXT";
			Assert.AreEqual("_ESTAP.TXT", WixFileElement.GenerateId(fileName));
		}
	
		[Test]
		public void CharIsNotStandardAscii()
		{
			string fileName = "AEèSTAP.TXT";
			Assert.AreEqual("AE_STAP.TXT", WixFileElement.GenerateId(fileName));
		}
	}
}
