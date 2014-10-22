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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Description of TextMarkerToolTipProvider.
	/// </summary>
	public class TextMarkerToolTipProvider : ITextAreaToolTipProvider
	{
		public void HandleToolTipRequest(ToolTipRequestEventArgs args)
		{
			if (args.InDocument) {
				int offset = args.Editor.Document.GetOffset(args.LogicalPosition);
				
				FoldingManager foldings = args.Editor.GetService(typeof(FoldingManager)) as FoldingManager;
				if (foldings != null) {
					var foldingsAtOffset = foldings.GetFoldingsAt(offset);
					FoldingSection collapsedSection = foldingsAtOffset.FirstOrDefault(section => section.IsFolded);
					
					if (collapsedSection != null) {
						args.SetToolTip(GetTooltipTextForCollapsedSection(args, collapsedSection));
					}
				}
				
				TextMarkerService textMarkerService = args.Editor.GetService(typeof(ITextMarkerService)) as TextMarkerService;
				if (textMarkerService != null) {
					var markersAtOffset = textMarkerService.GetMarkersAtOffset(offset);
					ITextMarker markerWithToolTip = markersAtOffset.FirstOrDefault(marker => marker.ToolTip != null);
					
					if (markerWithToolTip != null) {
						args.SetToolTip(markerWithToolTip.ToolTip);
					}
				}
			}
		}
		
		string GetTooltipTextForCollapsedSection(ToolTipRequestEventArgs args, FoldingSection foldingSection)
		{
			return ToolTipUtils.GetAlignedText(args.Editor.Document, foldingSection.StartOffset, foldingSection.EndOffset);
		}
	}
}
