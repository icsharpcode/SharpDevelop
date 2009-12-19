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

namespace ICSharpCode.XmlEditor
{
	public class RemoveXPathHighlightingCommand : AbstractMenuCommand
	{
		IWorkbench workbench;
		
		public RemoveXPathHighlightingCommand(IWorkbench workbench)
		{
			this.workbench = workbench;
		}
		
		public RemoveXPathHighlightingCommand()
			: this(WorkbenchSingleton.Workbench)
		{
		}
		
		public override void Run()
		{
			RemoveXPathNodeTextMarkers();
		}
		
		public void RemoveXPathNodeTextMarkers()
		{
			foreach (IViewContent view in workbench.ViewContentCollection) {
				ITextEditorProvider textEditorProvider = view as ITextEditorProvider;
				if (textEditorProvider != null) {
					XPathNodeTextMarker marker = new XPathNodeTextMarker(textEditorProvider.TextEditor.Document);
					marker.RemoveMarkers();
				}
			}
		}
	}
}
