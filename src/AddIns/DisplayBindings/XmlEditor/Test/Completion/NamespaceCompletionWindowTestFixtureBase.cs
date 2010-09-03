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
	public class NamespaceCompletionWindowTestFixtureBase
	{
		protected MockTextEditor textEditor;
		protected CodeCompletionKeyPressResult keyPressResult;
		protected XmlSchemaCompletionCollection schemas;
		protected XmlSchemaFileAssociations associations;
		
		protected void InitBase()
		{
			schemas = new XmlSchemaCompletionCollection();
			AddSchemas();

			associations = new XmlSchemaFileAssociations(new Properties(), new DefaultXmlSchemaFileAssociations(new AddInTreeNode()), schemas);
			
			textEditor = new MockTextEditor();
			textEditor.Document.Text = "<a xmlns></a>";
			textEditor.FileName = new FileName(@"c:\projects\test.xml");
			
			// Put caret just after "xmlns".
			textEditor.Caret.Offset = 8;			
		}		
		
		void AddSchemas()
		{
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='c' />";
			schemas.Add(new XmlSchemaCompletion(new StringReader(xml)));
			
			xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='b' />";
			schemas.Add(new XmlSchemaCompletion(new StringReader(xml)));
			
			xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='a' />";
			schemas.Add(new XmlSchemaCompletion(new StringReader(xml)));			
		}		
	}
}
