// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

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
			: this(SD.Workbench)
		{
		}
		
		public override void Run()
		{
			RemoveXPathNodeTextMarkers();
		}
		
		public void RemoveXPathNodeTextMarkers()
		{
			foreach (IViewContent view in workbench.ViewContentCollection) {
				ITextEditor textEditor = view.GetService<ITextEditor>();
				if (textEditor != null) {
					XPathNodeTextMarker.RemoveMarkers(textEditor);
				}
			}
		}
	}
}
