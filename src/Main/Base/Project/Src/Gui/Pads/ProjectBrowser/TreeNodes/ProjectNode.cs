// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
			} else if (project is UnknownProject) {
				OpenedImage = ClosedImage = "ProjectBrowser.ProjectWarning";
			} else {
				OpenedImage = ClosedImage = IconService.GetImageForProjectType(project.Language);
			}
			Tag = project;
			foreach (ProjectItem item in project.Items) {
				subItems.AddLast(item);
			}
		}
		
		public override void ShowProperties()
		{
			try {
				AddInTreeNode projectOptionsNode = AddInTree.GetTreeNode("/SharpDevelop/BackendBindings/ProjectOptions/" + project.Language);
				ProjectOptionsView projectOptions = new ProjectOptionsView(projectOptionsNode, project);
				WorkbenchSingleton.Workbench.ShowView(projectOptions);
			} catch (TreePathNotFoundException) {
				// TODO: Translate me!
				MessageService.ShowError("No installed project options panels were found.");
			}
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
				return true;
			}
		}
		
		public override void Cut()
		{
			DoPerformCut = true;
			Clipboard.SetDataObject(new DataObject(typeof(ISolutionFolder).ToString(), project.IdGuid), true);
		}
		// Paste is inherited from DirectoryNode.
		
		#endregion
		
		public override void AfterLabelEdit(string newName)
		{
			throw new System.NotImplementedException();
		}
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

	}
}
