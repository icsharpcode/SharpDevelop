// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using Microsoft.Win32;

namespace EmbeddedImageAddIn
{
	/// <summary>
	/// "Edit > Insert > Image" menu command.
	/// </summary>
	public class InsertImageCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (SD.Workbench.ActiveViewContent == null)
				return;
			ITextEditor editor = SD.Workbench.ActiveViewContent.GetService(typeof(ITextEditor)) as ITextEditor;
			if (editor == null)
				return;
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Image files|*.png;*.jpg;*.gif;*.bmp;*.jpeg|All files|*.*";
			dlg.CheckFileExists = true;
			dlg.DereferenceLinks = true;
			string baseDirectory = Path.GetDirectoryName(editor.FileName);
			dlg.InitialDirectory = baseDirectory;
			if (dlg.ShowDialog() == true) {
				string relativePath = FileUtility.GetRelativePath(baseDirectory, dlg.FileName);
				if (!Path.IsPathRooted(relativePath))
					relativePath = relativePath.Replace('\\', '/');
				editor.Document.Insert(editor.Caret.Offset, "<<IMAGE:" + relativePath + ">>");
			}
		}
	}
}
