// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
