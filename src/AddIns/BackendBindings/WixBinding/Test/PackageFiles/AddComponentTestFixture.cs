// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	/// Adds a component to a package.
	/// </summary>
	[TestFixture]
	public class AddComponentTestFixture : PackageFilesTestFixtureBase
	{
		XmlElement componentElement;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			XmlElement programFilesElement = (XmlElement)editor.Document.GetRootDirectory().ChildNodes[0];
			XmlElement installDirElement = (XmlElement)programFilesElement.ChildNodes[0];
			view.SelectedElement = installDirElement;
			editor.AddElement("Component");
			componentElement = (XmlElement)installDirElement.ChildNodes[0];
			
			// Try to add a component when there is no selected element.
			view.SelectedElement = null;
			editor.AddElement("Component");
		}
		
		[Test]
		public void ComponentAdded()
		{
			Assert.IsNotNull(componentElement);
			Assert.AreSame(componentElement, view.ElementsAdded[0]);
		}
		
		[Test]
		public void ComponentId()
		{
			Assert.AreEqual("NewComponent", componentElement.GetAttribute("Id"));
		}
		
		[Test]
		public void GuidGenerated()
		{
			Assert.IsTrue(componentElement.HasAttribute("Guid"));
		}
		
		/// <summary>
		/// Check that the string is a proper guid. The Guid constructor should
		/// throw an exception if the string is not of the correct format.
		/// </summary>
		[Test]
		public void Guid()
		{
			Guid g = new Guid(componentElement.GetAttribute("Guid"));
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
				"\t\t\t\t\t<Directory Id=\"INSTALLDIR\" Name=\"MyApp\">\r\n" +
				"\t\t\t\t\t</Directory>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
	}
}
