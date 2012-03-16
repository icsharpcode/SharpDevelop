// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Completion
{
	[TestFixture]
	public class CtrlSpaceAttributeValueCompletionTestFixture
	{
		bool result;
		MockTextEditor textEditor;
		XmlCodeCompletionBinding completionBinding;
		XmlSchemaFileAssociations associations;
		XmlSchemaCompletion xsdSchema;
		
		[SetUp]
		public void Init()
		{
			XmlSchemaCompletionCollection schemas = new XmlSchemaCompletionCollection();
			xsdSchema = new XmlSchemaCompletion(ResourceManager.ReadXsdSchema());
			schemas.Add(xsdSchema);

			associations = new XmlSchemaFileAssociations(new Properties(), new DefaultXmlSchemaFileAssociations(new AddInTreeNode()), schemas);
			associations.SetSchemaFileAssociation(new XmlSchemaFileAssociation(".xsd", "http://www.w3.org/2001/XMLSchema", "xs"));
			
			textEditor = new MockTextEditor();
			textEditor.FileName = new FileName(@"c:\projects\test.xsd");
			textEditor.Document.Text = "<xs:schema elementFormDefault=\"\"></xs:schema>";
			
			// Put cursor inside the double quotes following the elementFormDefault attribute
			textEditor.Caret.Offset = 31;	
			
			completionBinding = new XmlCodeCompletionBinding(associations);
			result = completionBinding.CtrlSpace(textEditor);
		}
		
		[Test]
		public void CtrlSpaceMethodResultIsTrueWhenCursorIsInsideAttributeValue()
		{
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ShowCompletionWindowCalledWithCompletionItems()
		{
			ICompletionItem[] items = textEditor.CompletionItemsDisplayedToArray();
			ICompletionItem[] expectedItems = GetXsdSchemaElementFormDefaultAttributeValues();
			
			Assert.AreEqual(expectedItems, items);
		}
		
		ICompletionItem[] GetXsdSchemaElementFormDefaultAttributeValues()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("schema", "http://www.w3.org/2001/XMLSchema", "xs"));
			return xsdSchema.GetAttributeValueCompletion(path, "elementFormDefault").ToArray();
		}
		
		[Test]
		public void ElementFormDefaultAttributeHasAttributeValues()
		{
			Assert.IsTrue(GetXsdSchemaElementFormDefaultAttributeValues().Length > 0);
		}
	}
}
