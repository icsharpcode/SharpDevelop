// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.FindSchemaObject
{
	[TestFixture]
	public class FindSelectedElementInTextEditorTestFixture
	{
		SelectedXmlElement selectedElement;
		
		[SetUp]
		public void Init()
		{
			string xml = "<abc></abc>";
			int index = 2;
			
			MockTextEditor textEditor = new MockTextEditor();
			textEditor.Document.Text = xml;
			textEditor.Caret.Offset = index;
			
			selectedElement = new SelectedXmlElement(textEditor);
		}
		
		[Test]
		public void ElementPathContainsSingleRootElement()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("abc", String.Empty));
			
			Assert.AreEqual(path, selectedElement.Path);
		}
		
		[Test]
		public void HasNoSelectedAttribute()
		{
			Assert.IsFalse(selectedElement.HasSelectedAttribute);
		}
		
		[Test]
		public void HasNoSelectedAttributeValue()
		{
			Assert.IsFalse(selectedElement.HasSelectedAttributeValue);
		}
		
		[Test]
		public void SelectedAttributeIsEmptyString()
		{
			Assert.AreEqual(String.Empty, selectedElement.SelectedAttribute);
		}
		
		[Test]
		public void SelectedAttributeValueIsEmptyString()
		{
			Assert.AreEqual(String.Empty, selectedElement.SelectedAttributeValue);
		}
	}
}
