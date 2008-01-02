// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Xml;

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class RootDirectoryRefTestFixture
	{
		WixDirectoryRefElement directoryRef;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			directoryRef = doc.RootDirectoryRef;
		}
		
		[Test]
		public void DirectoryRefFound()
		{
			Assert.IsNotNull(directoryRef);
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Fragment> \r\n" +
				"\t\t<DirectoryRef Id=\"TARGETDIR\">\r\n" +
				"\t\t\t<Directory Id=\"ProgramFiles\" SourceName=\"PFiles\"/>\r\n" +
				"\t\t</DirectoryRef>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
