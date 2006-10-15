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

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests that the short filename generated takes into account existing
	/// short filenames in the document. Note that only those files inside the
	/// same directory are considered. It is perfectly valid to have the same
	/// short filename installed to a different directory.
	/// </summary>
	[TestFixture]
	public class ExistingShortNameTests
	{
		WixDocument doc;
		WixComponentElement component;
		WixComponentElement myAppComponent;
		
		[SetUp]
		public void Init()
		{
			doc = new WixDocument();
			doc.FileName = @"C:\Projects\Setup\Setup.wxs";
			doc.LoadXml(GetWixXml());
			component = (WixComponentElement)doc.SelectSingleNode("//w:Component[@Id='SharpDevelopDocFiles']", new WixNamespaceManager(doc.NameTable));
			myAppComponent = (WixComponentElement)doc.SelectSingleNode("//w:Component[@Id='SharpDevelopMyAppFiles']", new WixNamespaceManager(doc.NameTable));
		}
		
		[Test]
		public void ShortNameAlreadyExists()
		{
			component.AddFile(@"C:\Projects\Setup\doc\changelog.xml");
			WixFileElement fileElement = (WixFileElement)component.LastChild;
			Assert.AreEqual("CHANGE_2.XML", fileElement.ShortName);
		}
		
		[Test]
		public void ShortNameDoesNotExistInParentDirectory()
		{
			myAppComponent.AddFile(@"C:\Projects\Setup\doc\changelog.xml");
			WixFileElement fileElement = (WixFileElement)myAppComponent.LastChild;
			Assert.AreEqual("CHANGE_1.XML", fileElement.ShortName);
		}
		
		/// <summary>
		/// Tests that the short name generated is correct when
		/// the file extension has only two characters.
		/// </summary>
		[Test]
		public void TwoCharacterExtension()
		{
			myAppComponent.AddFile(@"C:\Projects\Setup\doc\ebcdic-it.so");
			WixFileElement fileElement = (WixFileElement)myAppComponent.LastChild;
			Assert.AreEqual("EBCDIC_1.SO", fileElement.ShortName);
		}
		
		/// <summary>
		/// Tests that the short name generated is correct when
		/// the file extension has more than three characters.
		/// </summary>
		[Test]
		public void FourCharacterExtension()
		{
			myAppComponent.AddFile(@"C:\Projects\Setup\doc\abc.text");
			WixFileElement fileElement = (WixFileElement)myAppComponent.LastChild;
			Assert.AreEqual("ABC.TEX", fileElement.ShortName);
		}		

		string GetWixXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2003/01/wi\">\r\n" +
				"\t<Product Name=\"MySetup\" \r\n" +
				"\t         Manufacturer=\"\" \r\n" +
				"\t         Id=\"F4A71A3A-C271-4BE8-B72C-F47CC956B3AA\" \r\n" +
				"\t         Language=\"1033\" \r\n" +
				"\t         Version=\"1.0.0.0\">\r\n" +
				"\t\t<Package Id=\"6B8BE64F-3768-49CA-8BC2-86A76424DFE9\"/>\r\n" +
				"\t\t<Directory Id=\"TARGETDIR\" SourceName=\"SourceDir\">\r\n" +
				"\t\t\t<Directory Id=\"ProgramFilesFolder\" Name=\"PFiles\">\r\n" +
				"\t\t\t\t\t\t<Component Guid=\"370DE542-C4A9-48DA-ACF8-09949CDCD808\" Id=\"SharpDevelopMyAppFiles\" DiskId=\"1\">\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\AssemblyBaseAddresses.txt\" Name=\"baseaddr.txt\" Id=\"AssemblyBaseAddresses.txt\" LongName=\"AssemblyBaseAddresses.txt\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\BuiltWithSharpDevelop.png\" Name=\"bw-sd.PNG\" Id=\"BuiltWithSharpDevelop.png\" LongName=\"BuiltWithSharpDevelop.png\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\ChangeLog.xml\" Name=\"CHANGE.XML\" Id=\"ChangeLog.xml\" LongName=\"ChangeLog.xml\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\copyright.txt\" Name=\"cpyright.txt\" Id=\"copyright.txt\" LongName=\"copyright.txt\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\license.txt\" Name=\"license.txt\" Id=\"license.txt\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\readme.rtf\" Name=\"readme2.rtf\" Id=\"readme.rtf\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\SharpDevelopInfoResources.txt\" Name=\"Resource.txt\" Id=\"SharpDevelopInfoResources.txt\" LongName=\"SharpDevelopInfoResources.txt\" />\r\n" +
				"\t\t\t\t\t\t</Component>\r\n" +
				"\t\t\t\t\t<Directory Id=\"INSTALLDIR\" Name=\"MyApp\">\r\n" +
				"\t\t\t\t\t\t<Component Guid=\"77777777-C4A9-48DA-ACF8-09949CDCD808\" Id=\"SharpDevelopDocFiles\" DiskId=\"1\">\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\AssemblyBaseAddresses.txt\" Name=\"baseaddr.txt\" Id=\"AssemblyBaseAddresses2.txt\" LongName=\"AssemblyBaseAddresses.txt\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\BuiltWithSharpDevelop.png\" Name=\"bw-sd.PNG\" Id=\"BuiltWithSharpDevelop2.png\" LongName=\"BuiltWithSharpDevelop.png\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\ChangeLog.xml\" Name=\"CHANGE_1.XML\" Id=\"ChangeLog2.xml\" LongName=\"ChangeLog.xml\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\copyright.txt\" Name=\"cpyright.txt\" Id=\"copyright2.txt\" LongName=\"copyright.txt\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\license.txt\" Name=\"license.txt\" Id=\"license2.txt\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\readme.rtf\" Name=\"readme.rtf\" Id=\"readme2.rtf\" />\r\n" +
				"\t\t\t\t\t\t\t<File Source=\"doc\\SharpDevelopInfoResources.txt\" Name=\"Resource.txt\" Id=\"SharpDevelopInfoResources.txt\" LongName=\"SharpDevelopInfoResources.txt\" />\r\n" +
				"\t\t\t\t\t\t</Component>\r\n" +
				"\t\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
