// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Completion
{
	[TestFixture]
	public class NullCompletionItemsReturnedTestFixture
	{
		MockTextEditor textEditor;
		XmlSchemaCompletionDataCollection schemas;
		XmlCodeCompletionBinding completionBinding;
		
		[SetUp]
		public void Init()
		{
			schemas = new XmlSchemaCompletionDataCollection();

			XmlEditorOptions options = new XmlEditorOptions(new Properties(), new DefaultXmlSchemaFileAssociations(new AddInTreeNode()), schemas);
			
			textEditor = new MockTextEditor();
			textEditor.FileName = new FileName(@"c:\projects\test.xsd");
			textEditor.Document.Text = "";
			textEditor.Caret.Offset = 0;
			
			completionBinding = new XmlCodeCompletionBinding(options);
		}
		
		[Test]
		public void NullReferenceExceptionNotThrownWhenCallingHandleKeyPress()
		{
			Assert.DoesNotThrow(delegate { completionBinding.HandleKeyPress(textEditor, '='); } );
		}
	}
}
