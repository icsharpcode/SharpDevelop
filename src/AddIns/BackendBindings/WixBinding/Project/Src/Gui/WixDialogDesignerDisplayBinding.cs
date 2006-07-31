// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using System;
using System.IO;

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
		
		public ISecondaryViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			return new ISecondaryViewContent[] {new WixDialogDesigner(viewContent)};
		}
		
		static string GetViewContentFileName(IViewContent viewContent)
		{
			if (viewContent.IsUntitled) {
				return viewContent.UntitledName;
			} 
			return viewContent.FileName;
		}
	}
}
