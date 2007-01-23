// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WixBinding
{
	public class WixDialogDesignerDisplayBinding : ISecondaryDisplayBinding
	{
		public WixDialogDesignerDisplayBinding()
		{
		}
		
		public bool ReattachWhenParserServiceIsReady {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Wix dialog designer can attach to Wix source files (.wxs) and 
		/// Wix include files (.wxi).
		/// </summary>
		public bool CanAttachTo(IViewContent content)
		{
			ITextEditorControlProvider textAreaControlProvider = content as ITextEditorControlProvider;
			if (textAreaControlProvider == null) {
				return false;
			}
			
			string fileName = GetViewContentFileName(content);
			if (fileName == null) {
				return false;
			}
				
			return WixDocument.IsWixFileName(fileName);
		}
		
		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			return new IViewContent[] {new WixDialogDesigner(viewContent)};
		}
		
		static string GetViewContentFileName(IViewContent viewContent)
		{
			return viewContent.PrimaryFileName;
		}
	}
}
