// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WixBinding
{
	public class WixDialogDesignerDisplayBinding : ISecondaryDisplayBinding
	{
		public WixDialogDesignerDisplayBinding()
		{
		}
		
		public bool ReattachWhenParserServiceIsReady {
			get { return false; }
		}
		
		/// <summary>
		/// Wix dialog designer can attach to Wix source files (.wxs) and 
		/// Wix include files (.wxi).
		/// </summary>
		public bool CanAttachTo(IViewContent view)
		{
			if (IsViewTextEditorProvider(view)) {
				return WixFileName.IsWixFileName(view.PrimaryFileName);
			}
			return false;
		}
		
		public IViewContent[] CreateSecondaryViewContent(IViewContent view)
		{
			return new IViewContent[] {new WixDialogDesigner(view)};
		}
		
		bool IsViewTextEditorProvider(IViewContent view)
		{
			ITextEditorProvider textEditorProvider = view as ITextEditorProvider;
			return textEditorProvider != null;
		}
	}
}
