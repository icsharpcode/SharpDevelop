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

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	/// <summary>
	/// The package item (e.g. directory) is selected in the view.
	/// </summary>
	[TestFixture]
	public class ElementSelectedTestFixture : PackageFilesTestFixtureBase
	{
		WixXmlAttribute idAttribute;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			XmlElement rootDirectoryElement = editor.Document.GetRootDirectory();
			XmlElement directoryElement = (XmlElement)rootDirectoryElement.ChildNodes[0];
			view.SelectedElement = directoryElement;
			editor.SelectedElementChanged();
			idAttribute = base.GetAttribute(view.Attributes, "Id");
		}
		
		[Test]
		public void SelectedElementAccessed()
		{
			Assert.IsTrue(view.SelectedElementAccessed);
		}
		
		[Test]
		public void IdAttributeValue()
		{
			Assert.AreEqual("ProgramFilesFolder", idAttribute.Value);
		}
		
		[Test]
		public void IdAttributeType()
		{
			Assert.AreEqual(WixXmlAttributeType.Text, idAttribute.AttributeType);
		}
	
		/// <summary>
		/// Make sure that unspecified attributes are added.
		/// </summary>
		[Test]
		public void UnspecifiedFileSourceAttributeExists()
		{
			Assert.AreEqual(String.Empty, base.GetAttribute(view.Attributes, "FileSource").Value);
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsFalse(view.IsDirty);
		}
		
		[Test]
		public void AttributesChangedCalled()
		{
			Assert.IsTrue(view.IsAttributesChangedCalled);
		}
		
		protected override string GetWixXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Product Name=\"MySetup\" \r\n" +
				"\t         Manufacturer=\"\" \r\n" +
				"\t         Id=\"F4A71A3A-C271-4BE8-B72C-F47CC956B3AA\" \r\n" +
				"\t         Language=\"1033\" \r\n" +
				"\t         Version=\"1.0.0.0\">\r\n" +
				"\t\t<Package Id=\"6B8BE64F-3768-49CA-8BC2-86A76424DFE9\"/>\r\n" +
				"\t\t<Directory Id=\"TARGETDIR\" SourceName=\"SourceDir\">\r\n" +
				"\t\t\t<Directory Id=\"ProgramFilesFolder\" Name=\"PFiles\">\r\n" +
				"\t\t\t\t<Directory Id=\"MyCompany\" Name=\"MyComp\">\r\n" +
				"\t\t\t\t\t<Directory Id=\"INSTALLDIR\" Name=\"MyApp\">\r\n" +
				"\t\t\t\t\t</Directory>\r\n" +
				"\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
