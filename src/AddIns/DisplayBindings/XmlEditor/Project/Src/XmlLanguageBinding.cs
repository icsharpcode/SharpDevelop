// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XmlEditor
{
	public class XmlLanguageBinding : DefaultLanguageBinding
	{
		XmlFoldingManager foldingManager;
		AvalonEdit.AddIn.CodeEditorView codeEditorView;
		
		public override IFormattingStrategy FormattingStrategy {
			get { return new XmlFormattingStrategy(); }
		}
		
		public override void Attach(ITextEditor editor)
		{
			// HACK: disable SharpDevelop's built-in folding
			codeEditorView = editor.GetService(typeof(AvalonEdit.TextEditor)) as AvalonEdit.AddIn.CodeEditorView;
			if (codeEditorView != null)
				codeEditorView.DisableParseInformationFolding = true;
			
			foldingManager = new XmlFoldingManager(editor);
			foldingManager.UpdateFolds();
			foldingManager.Start();
			
			base.Attach(editor);
		}
		
		public override void Detach()
		{
			foldingManager.Stop();
			foldingManager.Dispose();
			
			if (codeEditorView != null)
				codeEditorView.DisableParseInformationFolding = false;
			
			base.Detach();
		}
	}
}
