// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Christian Hornung"/>
//     <version>$Revision$</version>
// </file>

using System;
using Hornung.ResourceToolkit.Resolver;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor;

namespace Hornung.ResourceToolkit.ToolTips
{
	/// <summary>
	/// Provides tooltips for resources.
	/// </summary>
	public class ResourceToolTipProvider : ITextAreaToolTipProvider
	{
		public void HandleToolTipRequest(ToolTipRequestEventArgs e)
		{
			if (!e.InDocument)
				return;
			
			Location logicPos = e.LogicalPosition;
			IDocument doc = e.Editor.Document;
			if (logicPos.X > doc.GetLine(logicPos.Y).Length) {
				return;
			}
			
			ResourceResolveResult result = ResourceResolverService.Resolve(e.Editor.FileName, doc, logicPos.Y - 1, logicPos.X - 1, null);
			
			if (result != null && result.ResourceFileContent != null) {
				e.ShowToolTip(ResourceResolverService.FormatResourceDescription(result.ResourceFileContent, result.Key));
			}
		}
		
	}
}
