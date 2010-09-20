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
	public class FileNamePropertyDescriptorTestFixture
	{
		WixXmlAttributePropertyDescriptor propertyDescriptor;
		EditorAttribute editorAttribute;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixXmlAttribute attribute = new WixXmlAttribute("Id", WixXmlAttributeType.FileName);
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
			Assert.AreEqual(typeof(RelativeFileNameEditor).AssemblyQualifiedName, editorAttribute.EditorTypeName);
		}
	}
}
