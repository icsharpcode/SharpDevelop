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

using ICSharpCode.SharpDevelop.Widgets.DesignTimeSupport;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PropertyGrid
{
	/// <summary>
	/// Tests that the DropDownEditor is specified as the editor for a
	/// WixAttributePropertyDescriptor that has possible attribute values.
	/// </summary>
	[TestFixture]
	public class DropDownPropertyDescriptorTestFixture
	{
		WixXmlAttributePropertyDescriptor propertyDescriptor;
		EditorAttribute editorAttribute;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixXmlAttribute attribute = new WixXmlAttribute("Id", WixXmlAttributeType.Text, new string[] {"a", "b"}, null);
			propertyDescriptor = new WixXmlAttributePropertyDescriptor(attribute);
			editorAttribute = WixBindingTestsHelper.GetEditorAttribute(propertyDescriptor.Attributes);
		}
		
		[Test]
		public void EditorAttributeAdded()
		{
			Assert.IsNotNull(editorAttribute);
		}
		
		[Test]
		public void EditorAttributeTypeName()
		{
			Assert.AreEqual(typeof(WixDropDownEditor).AssemblyQualifiedName, editorAttribute.EditorTypeName);
		}
	}
}
