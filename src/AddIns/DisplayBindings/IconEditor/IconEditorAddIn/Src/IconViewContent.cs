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
		
		public IconViewContent()
		{
			this.IsViewOnly = true;
		}
		
		public override void Load(string fileName)
		{
			this.FileName = fileName;
			this.TitleName = Path.GetFileName(fileName);
			editor.ShowFile(new IconFile(fileName));
		}
	}
}
