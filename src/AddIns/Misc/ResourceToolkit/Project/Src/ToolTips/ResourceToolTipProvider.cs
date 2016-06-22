// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
				e.SetToolTip(ResourceResolverService.FormatResourceDescription(result.ResourceFileContent, result.Key));
			}
		}
		
	}
}
