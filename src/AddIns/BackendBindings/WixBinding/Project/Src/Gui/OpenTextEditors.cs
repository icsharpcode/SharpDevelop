// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
