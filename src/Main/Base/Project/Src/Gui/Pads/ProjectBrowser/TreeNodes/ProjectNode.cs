// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project.Dialogs;

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
			sortOrder = 1;
			
			this.ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ProjectNode";
			this.project   = project;
			
			Text = project.Name;
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
		}
		
		public override void ActivateItem()
		{
			if (project is UnknownProject && Nodes.Count == 0) {
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
			if (File.Exists(newFileName)) {
				MessageService.ShowError("The file " + newFileName + " already exists.");
				return;
			}
			if (project.AssemblyName == project.Name)
				project.AssemblyName = newName;
			FileService.RenameFile(project.FileName, newFileName, false);
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
	}
}
