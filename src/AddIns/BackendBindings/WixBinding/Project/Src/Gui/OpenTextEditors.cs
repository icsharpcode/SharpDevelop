// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WixBinding
{
	public class OpenTextEditors
	{
		IWorkbench workbench;
		
		public OpenTextEditors(IWorkbench workbench)
		{
			this.workbench = workbench;
		}

		public ITextEditor FindTextEditorForDocument(WixDocument document)
		{
			foreach (IViewContent view in workbench.ViewContentCollection) {
				ITextEditorProvider textEditorProvider = view as ITextEditorProvider;
				if (textEditorProvider != null) {
					if (AreFileNamesEqual(view.PrimaryFileName, document.FileName)) {
						return textEditorProvider.TextEditor;
					}
				}
			}
			return null;
		}
		
		bool AreFileNamesEqual(FileName lhs, string rhs)
		{
			return FileUtility.IsEqualFileName(lhs.ToString(), rhs);
		}
	}
}
