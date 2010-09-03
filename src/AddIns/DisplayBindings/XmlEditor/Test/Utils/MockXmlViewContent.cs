// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Gui;

namespace XmlEditor.Tests.Utils
{
	public class MockXmlViewContent : AbstractViewContent, IFileDocumentProvider
	{
		OpenedFile file;
		AvalonEditDocumentAdapter document;
		
		public MockXmlViewContent(OpenedFile file)
		{
			this.file = file;
			this.Files.Add(file);
			this.document = new AvalonEditDocumentAdapter();
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
