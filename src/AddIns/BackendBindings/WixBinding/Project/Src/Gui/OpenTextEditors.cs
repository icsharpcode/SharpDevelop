// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Workbench;

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
				ITextEditor textEditor = view.GetService<ITextEditor>();
				if (AreFileNamesEqual(view.PrimaryFileName, document.FileName)) {
					return textEditor;
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
