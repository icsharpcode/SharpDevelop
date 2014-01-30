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
using System.IO;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public class SolutionItemNode : CustomFolderNode
	{
		ISolution     solution;
		ISolutionFileItem item;
		
		public ISolutionFileItem SolutionItem {
			get {
				return item;
			}
		}
		
		public FileName FileName {
			get {
				return item.FileName;
			}
		}
		
		public SolutionItemNode(ISolutionFileItem item)
		{
			sortOrder = 2;
			canLabelEdit = true;
			
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/SolutionItemNode";
			
			this.solution = item.ParentSolution;
			this.item = item;
			this.Text = Path.GetFileName(FileName);
			SetIcon(IconService.GetImageForFile(FileName));
		}
		
		public override void ActivateItem()
		{
			FileService.OpenFile(FileName);
		}
		
		#region Drag & Drop
		public override DataObject DragDropDataObject {
			get {
				return new DataObject(this);
			}
		}
		
		public override DragDropEffects GetDragDropEffect(IDataObject dataObject, DragDropEffects proposedEffect)
		{
			return ((ExtTreeNode)Parent).GetDragDropEffect(dataObject, proposedEffect);
		}
		
		public override void DoDragDrop(IDataObject dataObject, DragDropEffects effect)
		{
			((ExtTreeNode)Parent).DoDragDrop(dataObject, effect);
		}
		#endregion
		
		#region Cut & Paste
		public override bool EnableDelete {
			get {
				return true;
			}
		}
		
		public override void Delete()
		{
			var parentFolder = ((ISolutionFolderNode)Parent).Folder;
			parentFolder.Items.Remove(item);
			base.Remove();
			parentFolder.ParentSolution.Save();
		}
		
		public override bool EnablePaste {
			get {
				return ((ExtTreeNode)Parent).EnablePaste;
			}
		}
		
		public override void Paste()
		{
			((ExtTreeNode)Parent).Paste();
		}
		
		public override bool EnableCopy {
			get {
				return true;
			}
		}
		
		public override void Copy()
		{
			DoPerformCut = true;
			SD.Clipboard.SetDataObject(FileOperationClipboardObject.CreateDataObject(this, false));
		}
		
		public override bool EnableCut {
			get {
				return true;
			}
		}
		
		public override void Cut()
		{
			DoPerformCut = true;
			SD.Clipboard.SetDataObject(FileOperationClipboardObject.CreateDataObject(this, true));
		}
		#endregion
		
		public override void AfterLabelEdit(string newName)
		{
			if (newName == null) {
				return;
			}
			if (!FileService.CheckFileName(newName)) {
				return;
			}
			
			string newFileName = Path.Combine(Path.GetDirectoryName(this.FileName), newName);
			if (!FileService.RenameFile(this.FileName, newFileName, false)) {
				return;
			}
			solution.Save();
		}
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
