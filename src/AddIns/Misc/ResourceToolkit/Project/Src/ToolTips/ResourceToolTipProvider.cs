// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using Hornung.ResourceToolkit.Resolver;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace Hornung.ResourceToolkit.ToolTips
{
	/// <summary>
	/// Provides tooltips for resources.
	/// </summary>
	public class ResourceToolTipProvider : ITextAreaToolTipProvider
	{
		
		public ToolTipInfo GetToolTipInfo(TextArea textArea, ToolTipRequestEventArgs e)
		{
			TextLocation logicPos = e.LogicalPosition;
			IDocument doc = textArea.Document;
			if (logicPos.X > doc.GetLineSegment(logicPos.Y).Length - 1) {
				return null;
			}
			
			ResourceResolveResult result = ResourceResolverService.Resolve(textArea.MotherTextEditorControl.FileName, doc, logicPos.Y, logicPos.X, null);
			
			if (result != null && result.ResourceFileContent != null) {
				return new ToolTipInfo(ResourceResolverService.FormatResourceDescription(result.ResourceFileContent, result.Key));
			}
			
			return null;
		}
		
	}
}
