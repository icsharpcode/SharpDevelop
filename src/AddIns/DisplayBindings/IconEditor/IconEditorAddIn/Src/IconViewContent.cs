// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using IconEditor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.IconEditorAddIn
{
	public class IconViewContent : AbstractViewContent
	{
		EditorPanel editor = new EditorPanel();
		
		public override Control Control {
			get {
				return editor;
			}
		}
		
		public override bool IsViewOnly {
			get { return true; }
		}
		
		public IconViewContent(OpenedFile file) : base(file)
		{
		}
		
		public override void Load(OpenedFile file, Stream stream)
		{
			editor.ShowFile(new IconFile(stream));
		}
	}
}
