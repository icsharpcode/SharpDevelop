// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project.Dialogs;

namespace ICSharpCode.SharpDevelop.Project
{
	public class SolutionItemNode : CustomFolderNode
	{
		Solution     solution;
		SolutionItem item;
		
		public SolutionItem SolutionItem {
			get {
				return item;
			}
		}
		
		public string FileName {
			get {
				return Path.Combine(Path.GetDirectoryName(solution.FileName), item.Location);
			}
		}
		
		public SolutionItemNode(Solution solution, SolutionItem item)
		{
			sortOrder = 2;
			
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/SolutionItemNode";
			
			this.solution = solution;
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
			ISolutionFolderNode folderNode = Parent as ISolutionFolderNode;
			folderNode.Container.SolutionItems.Items.Remove(item);
			base.Remove();
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
			Clipboard.SetDataObject(new DataObject(typeof(SolutionItemNode).ToString(), new FileOperationClipboardObject(Path.Combine(Solution.Directory, item.Name), false)), true);
		}
		
		public override bool EnableCut {
			get {
				return true;
			}
		}
		
		public override void Cut()
		{
			DoPerformCut = true;
			Clipboard.SetDataObject(new DataObject(typeof(SolutionItemNode).ToString(), new FileOperationClipboardObject(Path.Combine(Solution.Directory, item.Name), true)), true);
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
			Text = newName;
			
			string newFileName = Path.Combine(solution.Directory, newName);
			FileService.RenameFile(FileUtility.GetAbsolutePath(solution.Directory, item.Name), newFileName, false);
			ProjectService.SaveSolution();
		}
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
