// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace XmlEditor.Tests.Utils
{
	public class MockXmlViewContent : AbstractViewContent, IFileDocumentProvider
	{
		OpenedFile file;
		IDocument document;
		
		public MockXmlViewContent(OpenedFile file)
		{
			this.file = file;
			this.Files.Add(file);
			this.document = new TextDocument();
		}
		
		public override object Control {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IDocument GetDocumentForFile(OpenedFile file)
		{
			return document;
		}
	}
}
