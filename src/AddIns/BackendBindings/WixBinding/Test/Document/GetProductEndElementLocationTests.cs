// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests that we can correctly locate the location of the Product end element.
	/// </summary>
	[TestFixture]
	public class GetProductEndElementLocationTests
	{
		[Test]
		public void EndElementOnOwnLine()
		{
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Product Name='DialogTest' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
			Location location = WixDocument.GetEndElementLocation(new StringReader(xml), "Product", "????????-????-????-????-????????????");
			Location expectedLocation = new Location(1, 6);
			Assert.AreEqual(expectedLocation, location);
		}
		
		/// <summary>
		/// Should not find an end element location since there is only the start tag.
		/// </summary>
		[Test]
		public void EmptyElement()
		{
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Product Name='DialogTest' \r\n" +
				"\t         Version='1.0' \r\n" +
				"\t         Language='1013' \r\n" +
				"\t         Manufacturer='#develop' \r\n" +
				"\t         Id='????????-????-????-????-????????????'/>\r\n" +
				"</Wix>";
			Location location = WixDocument.GetEndElementLocation(new StringReader(xml), "Product", "????????-????-????-????-????????????");
			Location expectedLocation = Location.Empty;
			Assert.AreEqual(expectedLocation, location);
		}
	}
}
