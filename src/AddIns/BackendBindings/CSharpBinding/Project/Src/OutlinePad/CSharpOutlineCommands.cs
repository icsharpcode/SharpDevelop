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
using ICSharpCode.SharpDevelop;
using ICSharpCode.AvalonEdit.Folding;
using System.Linq;
	
namespace CSharpBinding.OutlinePad
{
	/// <summary>
	/// RemoveNodeCommand.
	/// </summary>
	public class RemoveNodeCommand : SimpleCommand
	{
		public override bool CanExecute(object parameter) {
			var node = parameter as CSharpOutlineNode;
			if (node == null)
				return false;
			if (node.StartMarker == null || node.EndMarker == null)
				return false;			
			if (node.StartMarker.IsDeleted || node.EndMarker.IsDeleted)
				return false;
			if (node.EndMarker.Offset == 0)
				return false;
			if (node.EndMarker.Offset < node.StartMarker.Offset)
				return false;
			return true;
		}
		
		public override void Execute(object parameter) {
			var node = parameter as CSharpOutlineNode;
			node.Editor.Document.Remove(node.StartMarker.Offset, node.EndMarker.Offset - node.StartMarker.Offset);
		}
	}
	
	/// <summary>
	/// SelectRegionCommand.
	/// </summary>
	public class SelectRegionCommand : SimpleCommand
	{
		public override bool CanExecute(object parameter) {
			var node = parameter as CSharpOutlineNode;
			if (node == null)
				return false;
			if (node.StartMarker == null || node.EndMarker == null)
				return false;			
			if (node.StartMarker.IsDeleted || node.EndMarker.IsDeleted)
				return false;
			if (node.EndMarker.Offset == 0)
				return false;
			if (node.EndMarker.Offset < node.StartMarker.Offset)
				return false;
			return true;
		}
		
		public override void Execute(object parameter) {
			var node = parameter as CSharpOutlineNode;
			node.Editor.Select(node.StartMarker.Offset, node.EndMarker.Offset - node.StartMarker.Offset);
		}
	}	
	
	/// <summary>
	/// HandleFoldingCommand
	/// </summary>
	public class HandleFoldingCommand : SimpleCommand
	{
		public override bool CanExecute(object parameter) {
			var node = parameter as CSharpOutlineNode;
			if (node == null)
				return false;
			if (node.StartMarker == null || node.EndMarker == null)
				return false;			
			if (node.StartMarker.IsDeleted || node.EndMarker.IsDeleted)
				return false;
			if (node.EndMarker.Offset == 0)
				return false;
			if (node.EndMarker.Offset < node.StartMarker.Offset)
				return false;
			return true;
		}
		
		public override void Execute(object parameter) {
			var node = parameter as CSharpOutlineNode;
			
			if (node == null || node.Editor == null)
				return;
			
			FoldingManager foldingManager = node.Editor.GetService(typeof(FoldingManager)) as FoldingManager;
			
			if (foldingManager == null) 
				return;
			
			// The endline is, where the first child starts. The folding has to be 
			// between the StartMarker.Line of the current node and the StartMarker.Line of the first child.	
			int endLine = node.StartMarker.Line;				
			var firstChild = node.Children.FirstOrDefault();
			if (firstChild != null) {
				endLine = (firstChild as CSharpOutlineNode).StartMarker.Line;
			}
			
			FoldingSection folding = foldingManager.GetNextFolding(node.StartMarker.Offset);
			
			if (folding == null)
				return;	
			// is next folding below the endline, the it belongs to a other node			
			if (node.Editor.Document.GetLineForOffset(folding.StartOffset).LineNumber >= endLine)
				return;
			
			folding.IsFolded = !node.IsExpanded;					
		}
	}
}
