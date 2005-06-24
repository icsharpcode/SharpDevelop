using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class SolutionNode : AbstractProjectBrowserTreeNode, ISolutionFolderNode
	{
		Solution solution;
		public ISolutionFolder Folder {
			get {
				return solution;
			}
		}
		
		public override Solution Solution {
			get {
				return solution;
			}
		}
		public ISolutionFolderContainer Container {
			get {
				return solution;
			}
		}
		
		public SolutionNode(Solution solution)
		{
			sortOrder = -1;
			this.solution = solution;
			Text = "Solution " + solution.Name;
			autoClearNodes = false;
			
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/SolutionNode";
			
			SetIcon("ProjectBrowser.Solution");
			Tag = solution;
		}
		
		public void AddItem(string fileName)
		{
			throw new NotImplementedException();
//			string relativeFileName = FileUtility.GetRelativePath(Path.GetDirectoryName(solution.FileName), fileName);
//			folder.SolutionItems.Items.Add(new SolutionItem(relativeFileName, relativeFileName));
//			new FileNode(fileName, FileNodeStatus.InProject).AddTo(this);
		}
		
//		protected override void Initialize()
//		{
//			base.Initialize();
//		}
		
		#region Drag & Drop
		public override DragDropEffects GetDragDropEffect(IDataObject dataObject, DragDropEffects proposedEffect)
		{
			if (dataObject.GetDataPresent(typeof(SolutionFolderNode))) {
				SolutionFolderNode folderNode = (SolutionFolderNode)dataObject.GetData(typeof(SolutionFolderNode));
				
				if (folderNode.Folder.Parent != solution) {
					return DragDropEffects.All;
				}
			}
			
			if (dataObject.GetDataPresent(typeof(ProjectNode))) {
				ProjectNode projectNode = (ProjectNode)dataObject.GetData(typeof(ProjectNode));
				
				if (projectNode.Parent != this) {
					return DragDropEffects.Move;
				}
			}
			return DragDropEffects.None;
			
		}
		
		public override void DoDragDrop(IDataObject dataObject, DragDropEffects effect)
		{
			AbstractProjectBrowserTreeNode parentNode = null;
			
			if (dataObject.GetDataPresent(typeof(SolutionFolderNode))) {
				SolutionFolderNode folderNode = (SolutionFolderNode)dataObject.GetData(typeof(SolutionFolderNode));
				parentNode = folderNode.Parent as AbstractProjectBrowserTreeNode;
				
				folderNode.Remove();
				folderNode.AddTo(this);
				
				this.solution.AddFolder(folderNode.Folder);
			}
			if (dataObject.GetDataPresent(typeof(ProjectNode))) {
				ProjectNode projectNode = (ProjectNode)dataObject.GetData(typeof(ProjectNode));
				parentNode = projectNode.Parent as AbstractProjectBrowserTreeNode;
				
				projectNode.Remove();
				projectNode.AddTo(this);
				projectNode.EnsureVisible();
				this.solution.AddFolder(projectNode.Project);
			}
			
			if (parentNode != null) {
				parentNode.Refresh();
			}
			
			
			solution.Save();
		}
		#endregion
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		#region Cut&Paste
		public override bool EnablePaste {
			get {
				return SolutionFolderNode.DoEnablePaste(this);
			}
		}
		
		public override void Paste()
		{
			SolutionFolderNode.DoPaste(this);
		}
		#endregion
	}
}
