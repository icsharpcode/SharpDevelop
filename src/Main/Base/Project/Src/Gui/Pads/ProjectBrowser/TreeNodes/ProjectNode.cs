// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ProjectNode : DirectoryNode
	{
		IProject project;
		
		public override bool Visible {
			get {
				return true;
			}
		}
		
		public override IProject Project {
			get {
				return project;
			}
		}
		
		public override string RelativePath {
			get {
				return "";
			}
		}
		public override string Directory {
			get {
				return project.Directory;
			}
			set {
				throw new System.NotSupportedException();
			}
		}
		
		public ProjectNode(IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			sortOrder = 1;
			
			this.ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ProjectNode";
			this.project = project;
			
			Text = project.Name;
			if (project.ReadOnly) {
				Text += StringParser.Parse(" (${res:Global.ReadOnly})");
			}
			
			autoClearNodes = false;
			
			if (project is MissingProject) {
				OpenedImage = ClosedImage = "ProjectBrowser.MissingProject";
				this.ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/MissingProjectNode";
			} else if (project is UnknownProject) {
				OpenedImage = ClosedImage = "ProjectBrowser.ProjectWarning";
				this.ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/UnknownProjectNode";
			} else {
				OpenedImage = ClosedImage = IconService.GetImageForProjectType(project.Language);
			}
			Tag = project;
			
			if (project.ParentSolution != null) {
				project.ParentSolution.Preferences.StartupProjectChanged += OnStartupProjectChanged;
				OnStartupProjectChanged(null, null);
			}
		}
		
		public override void Dispose()
		{
			base.Dispose();
			if (project.ParentSolution != null) {
				project.ParentSolution.Preferences.StartupProjectChanged -= OnStartupProjectChanged;
			}
		}
		
		bool isStartupProject;
		
		void OnStartupProjectChanged(object sender, EventArgs e)
		{
			bool newIsStartupProject = (this.project == project.ParentSolution.Preferences.StartupProject);
			if (newIsStartupProject != isStartupProject) {
				isStartupProject = newIsStartupProject;
				drawDefault = !isStartupProject;
				if (this.TreeView != null) {
					this.TreeView.Invalidate(this.Bounds);
				}
			}
		}
		
		protected override int MeasureItemWidth(DrawTreeNodeEventArgs e)
		{
			if (isStartupProject) {
				return MeasureTextWidth(e.Graphics, this.Text, BoldDefaultFont);
			} else {
				return base.MeasureItemWidth(e);
			}
		}
		
		protected override void DrawForeground(DrawTreeNodeEventArgs e)
		{
			if (isStartupProject) {
				DrawText(e, this.Text, SystemBrushes.WindowText, BoldDefaultFont);
			}
		}
		
		public override void ActivateItem()
		{
			if (project is MSBuildFileProject) {
				FileService.OpenFile(project.FileName);
			}
		}
		
		public override void ShowProperties()
		{
			Commands.ViewProjectOptions.ShowProjectOptions(project);
		}
		
		#region Drag & Drop
		public override DataObject DragDropDataObject {
			get {
				return new DataObject(this);
			}
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
			ProjectService.RemoveSolutionFolder(Project.IdGuid);
			ProjectService.SaveSolution();
		}
		
		public override bool EnableCopy {
			get {
				return false;
			}
		}
		public override void Copy()
		{
			throw new System.NotSupportedException();
		}
		
		public override bool EnableCut {
			get {
				if (IsEditing) {
					return false;
				}
				return true;
			}
		}
		
		public override void Cut()
		{
			DoPerformCut = true;
			ClipboardWrapper.SetDataObject(new DataObject(typeof(ISolutionFolder).ToString(), project.IdGuid));
		}
		// Paste is inherited from DirectoryNode.
		
		#endregion
		
		public override void AfterLabelEdit(string newName)
		{
			RenameProject(project, newName);
			Text = project.Name;
		}
		
		public static void RenameProject(IProject project, string newName)
		{
			if (project.Name == newName)
				return;
			if (!FileService.CheckFileName(newName))
				return;
			// multiple projects with the same name shouldn't be a problem
//			foreach (IProject p in ProjectService.OpenSolution.Projects) {
//				if (string.Equals(p.Name, newName, StringComparison.OrdinalIgnoreCase)) {
//					MessageService.ShowMessage("There is already a project with this name.");
//					return;
//				}
//			}
			string newFileName = Path.Combine(project.Directory, newName + Path.GetExtension(project.FileName));
			
			// see issue #2 on http://community.sharpdevelop.net/forums/t/15800.aspx:
			// The name of the project and the file name might differ. So if the FileName is
			// already the same as the new project file name, just update the name in the solution.
			if (FileUtility.IsEqualFileName(newFileName, project.FileName)) {
				project.Name = newName;
				ProjectService.SaveSolution();
				return;
			}
			
			if (!FileService.RenameFile(project.FileName, newFileName, false)) {
				return;
			}
			if (project.AssemblyName == project.Name)
				project.AssemblyName = newName;
			if (File.Exists(project.FileName + ".user"))
				FileService.RenameFile(project.FileName + ".user", newFileName + ".user", false);
			foreach (IProject p in ProjectService.OpenSolution.Projects) {
				foreach (ProjectItem item in p.Items) {
					if (item.ItemType == ItemType.ProjectReference) {
						ProjectReferenceProjectItem refItem = (ProjectReferenceProjectItem)item;
						if (refItem.ReferencedProject == project) {
							refItem.ProjectName = newName;
							refItem.Include = FileUtility.GetRelativePath(p.Directory, newFileName);
						}
					}
				}
			}
			project.FileName = newFileName;
			project.Name = newName;
			ProjectService.SaveSolution();
		}
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public virtual void AddNewItemsToProject()
		{
			new Project.Commands.AddNewItemsToProject().Run();
			return;
		}

		public override AbstractProjectBrowserTreeNode GetNodeByRelativePath(string relativePath)
		{
			foreach (AbstractProjectBrowserTreeNode node in Nodes)
			{
				if (node != null) {
					AbstractProjectBrowserTreeNode returnedNode = node.GetNodeByRelativePath(relativePath);
					if (returnedNode != null)
						return returnedNode;
				}
			}

			return this;
		}
	}
}
