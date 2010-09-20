// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WixBinding
{
	public class WixDocumentWindow
	{
		IWorkbench workbench;
		
		public WixDocumentWindow(IWorkbench workbench)
		{
			this.workbench = workbench;
		}
		
		public bool IsActive(WixDocument document)
		{
			if (document != null) {
				IViewContent view = workbench.ActiveViewContent;
				if (view != null) {
					return FileUtility.IsEqualFileName(view.PrimaryFileName, document.FileName);
				}
			}
			return false;
		}
	}
}
