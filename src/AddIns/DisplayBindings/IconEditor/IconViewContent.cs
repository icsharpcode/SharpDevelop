// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public override object Control {
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
