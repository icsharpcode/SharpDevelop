// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		XmlSchemaCompletionCollection schemas;
		XmlCodeCompletionBinding completionBinding;
		
		[SetUp]
		public void Init()
		{
			schemas = new XmlSchemaCompletionCollection();

			XmlSchemaFileAssociations associations = new XmlSchemaFileAssociations(new Properties(), new DefaultXmlSchemaFileAssociations(new AddInTreeNode()), schemas);
			
			textEditor = new MockTextEditor();
			textEditor.FileName = new FileName(@"c:\projects\test.xsd");
			textEditor.Document.Text = "";
			textEditor.Caret.Offset = 0;
			
			completionBinding = new XmlCodeCompletionBinding(associations);
		}
		
		[Test]
		public void NullReferenceExceptionNotThrownWhenCallingHandleKeyPress()
		{
			Assert.DoesNotThrow(delegate { completionBinding.HandleKeyPress(textEditor, '='); } );
		}
	}
}
