// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PropertyGrid
{
	[TestFixture]
	public class GuidPropertyDescriptorTestFixture
	{
		WixXmlAttributePropertyDescriptor propertyDescriptor;
		EditorAttribute editorAttribute;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixXmlAttribute attribute = new WixXmlAttribute("Id", WixXmlAttributeType.ComponentGuid);
			propertyDescriptor = new WixXmlAttributePropertyDescriptor(attribute);
			editorAttribute = WixBindingTestsHelper.GetEditorAttribute(propertyDescriptor.Attributes);
		}
		
		[Test]
		public void EditorAttributeAdded()
		{
			Assert.IsNotNull(editorAttribute);
		}
		
		[Test]
		public void EditorAttributeBaseTypeName()
		{
			Assert.AreEqual(typeof(UITypeEditor).AssemblyQualifiedName, editorAttribute.EditorBaseTypeName);
		}
		
		[Test]
		public void EditorAttributeTypeName()
		{
			Assert.AreEqual(typeof(GuidEditor).AssemblyQualifiedName, editorAttribute.EditorTypeName);
		}
		
		[Test]
		public void AutogenUuid()
		{
			WixXmlAttribute attribute = new WixXmlAttribute("Id", WixXmlAttributeType.AutogenGuid);
			propertyDescriptor = new WixXmlAttributePropertyDescriptor(attribute);
			EditorAttribute editorAttribute = WixBindingTestsHelper.GetEditorAttribute(propertyDescriptor.Attributes);
			
			Assert.IsNotNull(editorAttribute);
			Assert.AreEqual(typeof(GuidEditor).AssemblyQualifiedName, editorAttribute.EditorTypeName);
		}
		
		[Test]
		public void Uuid()
		{
			WixXmlAttribute attribute = new WixXmlAttribute("Id", WixXmlAttributeType.Guid);
			propertyDescriptor = new WixXmlAttributePropertyDescriptor(attribute);
			EditorAttribute editorAttribute = WixBindingTestsHelper.GetEditorAttribute(propertyDescriptor.Attributes);
			
			Assert.IsNotNull(editorAttribute);
			Assert.AreEqual(typeof(GuidEditor).AssemblyQualifiedName, editorAttribute.EditorTypeName);
		}
	}
}
