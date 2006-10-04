// <file>
//     <copyright see="prj:///Doc/copyright.txt"/>
//     <license see="prj:///Doc/license.txt"/>
//     <owner name="Christian Hornung" email="c-hornung@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using Hornung.ResourceToolkit.Resolver;

namespace Hornung.ResourceToolkit.ToolTips
{
	/// <summary>
	/// Provides tooltips for resources.
	/// </summary>
	public class ResourceToolTipProvider : ITextAreaToolTipProvider
	{
		
		public ToolTipInfo GetToolTipInfo(TextArea textArea, ToolTipRequestEventArgs e)
		{
			Point logicPos = e.LogicalPosition;
			IDocument doc = textArea.Document;
			if (logicPos.X > doc.GetLineSegment(logicPos.Y).Length - 1) {
				return null;
			}
			
			ResourceResolveResult result = ResourceResolverService.Resolve(textArea.MotherTextEditorControl.FileName, doc, logicPos.Y, logicPos.X);
			
			if (result != null && result.ResourceFileContent != null) {
				return new ToolTipInfo(ResourceResolverService.FormatResourceDescription(result.ResourceFileContent, result.Key));
			}
			
			return null;
		}
		
	}
}
