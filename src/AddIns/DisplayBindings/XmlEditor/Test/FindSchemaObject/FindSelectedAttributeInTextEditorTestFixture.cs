// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.FindSchemaObject
{
	[TestFixture]
	public class FindSelectedAttributeInTextEditorTestFixture
	{
		SelectedXmlElement selectedElement;
		
		[SetUp]
		public void Init()
		{
			string xml = "<root attribute1=''></root>";
			int index = 10;
			
			MockTextEditor textEditor = new MockTextEditor();
			textEditor.Document.Text = xml;
			textEditor.Caret.Offset = index;
			
			selectedElement = new SelectedXmlElement(textEditor);
		}
		
		[Test]
		public void ElementPathContainsSingleRootElement()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("root", String.Empty));
			
			Assert.AreEqual(path, selectedElement.Path);
		}
		
		[Test]
		public void HasSelectedAttribute()
		{
			Assert.IsTrue(selectedElement.HasSelectedAttribute);
		}
		
		[Test]
		public void HasNoSelectedAttributeValue()
		{
			Assert.IsFalse(selectedElement.HasSelectedAttributeValue);
		}
		
		[Test]
		public void SelectedAttributeIsFirstAttribute()
		{
			Assert.AreEqual("attribute1", selectedElement.SelectedAttribute);
		}
		
		[Test]
		public void SelectedAttributeValueIsEmptyString()
		{
			Assert.AreEqual(String.Empty, selectedElement.SelectedAttributeValue);
		}
	}
}
