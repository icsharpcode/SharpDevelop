// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System.Windows.Media;

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
