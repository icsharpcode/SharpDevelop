// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace XmlEditor.Tests.Utils
{
	/// <summary>
	/// Description of MockXmlViewContent.
	/// </summary>
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
