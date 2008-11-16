// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Xml.Schema;

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class WixSchemaTests
	{
		WixSchemaCompletionData schema;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			schema = new WixSchemaCompletionData();
		}
		
		[Test]
		public void DirectoryChildElements()
		{
			string[] childElements = schema.GetChildElements("Directory");
			Assert.AreEqual(4, childElements.Length);
			Assert.Contains("Component", childElements);
			Assert.Contains("Directory", childElements);
			Assert.Contains("Merge", childElements);
			Assert.Contains("SymbolPath", childElements);
		}
		
		[Test]
		public void DirectoryElementAttributes()
		{
			string[] attributes = schema.GetAttributeNames("Directory");
			Assert.IsTrue(attributes.Length > 0);
			Assert.Contains("Id", attributes);
			Assert.Contains("FileSource", attributes);
		}
		
		[Test]
		public void SrcAttributeExcluded()
		{
			string[] attributes = schema.GetAttributeNames("Directory");
			Assert.IsTrue(attributes.Length > 0);
			foreach (string attribute in attributes) {
				Assert.IsFalse(attribute == "src");
			}
		}
		
		[Test]
		public void UpgradeImageDeprecatedAttributes()
		{
			string[] attributes = schema.GetDeprecatedAttributes("UpgradeImage");
			Assert.Contains("src", attributes);
			Assert.Contains("srcPatch", attributes);
		}
		
		[Test]
		public void ProductAutogenuuidAttributeType()
		{
			QualifiedName attributeName = schema.GetAttributeType("Product", "Id");
			Assert.AreEqual("AutogenGuid", attributeName.Name);
		}
		
		[Test]
		public void ComponentKeyPathAttributeValues()
		{
			string[] values = schema.GetAttributeValues("Component", "KeyPath");
			Assert.AreEqual(2, values.Length);
			Assert.Contains("yes", values);
			Assert.Contains("no", values);
		}
	}
}
