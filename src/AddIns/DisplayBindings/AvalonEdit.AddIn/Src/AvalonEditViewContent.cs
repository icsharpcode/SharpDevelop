// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Document;
using System;
using System.IO;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class AvalonEditViewContent : AbstractViewContent
	{
		TextEditor textEditor = new TextEditor {
			Background = Brushes.White,
			FontFamily = new FontFamily("Consolas")
		};
		
		public AvalonEditViewContent(OpenedFile file)
		{
			this.Files.Add(file);
			file.ForceInitializeView(this);
			textEditor.Document.Changed += textEditor_Document_Changed;
		}

		void textEditor_Document_Changed(object sender, DocumentChangeEventArgs e)
		{
			PrimaryFile.IsDirty = true;
		}
		
		public override object Content {
			get { return textEditor; }
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			if (file != PrimaryFile)
				return;
			textEditor.Save(stream);
		}
		
		public override void Load(OpenedFile file, Stream stream)
		{
			if (file != PrimaryFile)
				return;
			textEditor.SyntaxHighlighting =
				HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(file.FileName));
			textEditor.Load(stream);
		}
	}
}
