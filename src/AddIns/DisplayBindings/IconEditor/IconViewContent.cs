// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.IconEditor
{
	public class IconViewContent : AbstractViewContent
	{
		EditorPanel editor = new EditorPanel();
		
		public override object Control {
			get {
				return editor;
			}
		}
		
		public IconViewContent(OpenedFile file) : base(file)
		{
			editor.IconWasEdited += editor_IconWasEdited;
		}
		
		void editor_IconWasEdited(object sender, EventArgs e)
		{
			PrimaryFile.MakeDirty();
		}
		
		public override void Load(OpenedFile file, Stream stream)
		{
			try {
				editor.ShowFile(new IconFile(stream));
			} catch (InvalidIconException ex) {
				// call with a delay to work around a re-entrancy bug
				// when closing a workbench window while it is getting activated
				WorkbenchSingleton.SafeThreadAsyncCall(
					delegate {
						MessageService.ShowHandledException(ex);
						if (WorkbenchWindow != null)
							WorkbenchWindow.CloseWindow(true);
					});
			}
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			editor.SaveIcon(stream);
		}
	}
}
