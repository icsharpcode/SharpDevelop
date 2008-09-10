// SharpDevelop samples
// Copyright (c) 2007, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.NAnt.Gui
{
	/// <summary>
	/// NAnt pad's tree view.  Shows a high level view of build file in
	/// a set of projects.
	/// </summary>
	public class NAntPadTreeView : System.Windows.Forms.UserControl, IOwnerState
	{
		private System.Windows.Forms.TreeView treeView;
		const string ContextMenuAddInTreePath = "/SharpDevelop/Pads/NAntPad/ContextMenu";
		
		/// <summary>
		/// The possible states of the tree view.
		/// </summary>
		public enum NAntPadTreeViewState {
			Nothing = 0,
			BuildFileSelected = 1,
			TargetSelected = 2,
			ErrorSelected = 4
		}
		
		/// <summary>
		/// The current state of the tree view.
		/// </summary>
		NAntPadTreeViewState state = NAntPadTreeViewState.Nothing;
		
		delegate void AddSolutionInvoker(Solution solution);
		
		public NAntPadTreeView()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			treeView.Sorted = true;
			treeView.HideSelection = false;
			treeView.ImageList = NAntPadTreeViewImageList.GetImageList();
			treeView.ContextMenuStrip = MenuService.CreateContextMenu(this, ContextMenuAddInTreePath);
			treeView.DoubleClick += new EventHandler(TreeViewDoubleClick);
		}
		
		/// <summary>
		/// Gets the "ownerstate" condition.
		/// </summary>
		public Enum InternalState {
			get {
				return state;
			}
		}
		
		/// <summary>
		/// Clears all items from the tree view.
		/// </summary>
		public void Clear()
		{
			if (InvokeRequired) {
				MethodInvoker invoker = new MethodInvoker(Clear);
				Invoke(invoker);
			} else {
				treeView.Nodes.Clear();
			}
		}
		
		/// <summary>
		/// Adds items to the tree view for each build file that exists
		/// in all the solution's subprojects.
		/// </summary>
		/// <param name="solution">A solution containing projects.</param>
		public void AddSolution(Solution solution)
		{
			if (InvokeRequired) {
				AddSolutionInvoker invoker = new AddSolutionInvoker(AddSolution);
				Invoke(invoker);
			} else {
				foreach (IProject project in solution.Projects) {
					AddProject(project);
				}
			}
		}
		
		/// <summary>
		/// Adds items to the tree view for each build file that exist
		/// in a project.
		/// </summary>
		/// <param name="project">A SharpDevelop project.</param>
		public void AddProject(IProject project)
		{
			Debug.Assert(!InvokeRequired, "AddProject InvokeRequired");
			
			foreach (ProjectItem projectItem in project.Items) {
				if (NAntBuildFile.IsBuildFile(projectItem.FileName)) {
					AddBuildFile(project.Name, projectItem.FileName);
				}
			}
		}
		
		/// <summary>
		/// Removes the specified build file from the
		/// tree view.</summary>
		public void RemoveBuildFile(string fileName)
		{
			Debug.Assert(!InvokeRequired, "RemoveBuildFile InvokeRequired");
			
			NAntBuildFileTreeNode node = FindMatchingNode(fileName);
			if (node != null) {
				node.Remove();
			}
		}
		
		/// <summary>
		/// Renames the build file.
		/// </summary>
		/// <param name="oldFileName">The filename to update.</param>
		/// <param name="newFileName">The updated filename.</param>
		public void RenameBuildFile(string oldFileName, string newFileName)
		{
			Debug.Assert(!InvokeRequired, "RenameBuildFile InvokeRequired");

			NAntBuildFileTreeNode node = FindMatchingNode(oldFileName);
			
			if (node != null) {
				node.FileName = Path.GetFileName(newFileName);
			}
		}
		
		/// <summary>
		/// Updates the build file in the tree view.
		/// </summary>
		/// <param name="fileName">The build file name.</param>
		public void UpdateBuildFile(string fileName)
		{
			Debug.Assert(!InvokeRequired, "UpdateBuildFile InvokeRequired");
			
			NAntBuildFileTreeNode node = FindMatchingNode(fileName);
			
			if (node != null) {
				NAntBuildFile buildFile = new NAntBuildFile(fileName);
				node.BuildFile = buildFile;
			} else {
				AddBuildFile(String.Empty, fileName);
			}
		}
		
		/// <summary>
		/// Adds a build file to the tree.
		/// </summary>
		/// <param name="projectName">The name of the project.</param>
		/// <param name="fileName">The build file name.</param>
		/// <param name="debug"><see langword="true"/> if the project's
		/// active configuration is debug; <see langword="false"/>
		/// otherwise.</param>
		public void AddBuildFile(string projectName, string fileName)
		{
			Debug.Assert(!InvokeRequired, "AddBuildFile InvokeRequired");

			if (File.Exists(fileName)) {
				NAntBuildFile buildFile = new NAntBuildFile(fileName);
				NAntBuildFileTreeNode node = new NAntBuildFileTreeNode(projectName, buildFile);
				treeView.Nodes.Add(node);
			}
		}
		
		/// <summary>
		/// Gets the currently selected <see cref="NAntBuildFile"/>.
		/// </summary>
		/// <remarks>This will return a NAntBuildFile if
		/// a target node is selected.</remarks>
		public NAntBuildFile SelectedBuildFile {
			get {
				NAntBuildFile buildFile = null;
				
				TreeNode selectedNode = treeView.SelectedNode;
				if (selectedNode is NAntBuildFileTreeNode) {
					NAntBuildFileTreeNode buildNode = (NAntBuildFileTreeNode)selectedNode;
					buildFile = buildNode.BuildFile;
				} else if(selectedNode is NAntBuildTargetTreeNode) {
					NAntBuildTargetTreeNode targetNode = (NAntBuildTargetTreeNode)selectedNode;
					NAntBuildFileTreeNode buildNode = (NAntBuildFileTreeNode)targetNode.Parent;
					buildFile = buildNode.BuildFile;
				} else if(selectedNode is NAntBuildFileErrorTreeNode) {
					NAntBuildFileErrorTreeNode errorNode = (NAntBuildFileErrorTreeNode)selectedNode;
					NAntBuildFileTreeNode buildNode = (NAntBuildFileTreeNode)errorNode.Parent;
					buildFile = buildNode.BuildFile;
				}
				
				return buildFile;
			}
		}
		
		/// <summary>
		/// Gets the current selected <see cref="NAntBuildTarget"/>
		/// </summary>
		public NAntBuildTarget SelectedTarget {
			get {
				NAntBuildTarget target = null;
				
				NAntBuildTargetTreeNode targetNode = treeView.SelectedNode as NAntBuildTargetTreeNode;
				
				if (targetNode != null) {
					target = targetNode.Target;
				}
				
				return target;
			}
		}
		
		/// <summary>
		/// Gets the current selected <see cref="NAntBuildFileError"/>
		/// </summary>
		public NAntBuildFileError SelectedError {
			get {
				NAntBuildFileError error = null;
				
				NAntBuildFileErrorTreeNode errorNode = treeView.SelectedNode as NAntBuildFileErrorTreeNode;
				
				if (errorNode != null) {
					error = errorNode.Error;
				}
				
				return error;
			}
		}
		
		/// <summary>
		/// Gets whether a target is selected.
		/// </summary>
		public bool IsTargetSelected {
			get {
				bool isSelected = false;
				
				if (SelectedTarget != null) {
					isSelected = true;
				}
				
				return isSelected;
			}
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.treeView = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// treeView
			// 
			this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView.ImageIndex = -1;
			this.treeView.Location = new System.Drawing.Point(0, 0);
			this.treeView.Name = "treeView";
			this.treeView.SelectedImageIndex = -1;
			this.treeView.Size = new System.Drawing.Size(292, 266);
			this.treeView.TabIndex = 0;
			this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeViewMouseDown);
			// 
			// NAntPadTreeView
			// 
			this.Controls.Add(this.treeView);
			this.Name = "NAntPadTreeView";
			this.Size = new System.Drawing.Size(292, 266);
			this.ResumeLayout(false);
		}
		#endregion
		
		/// <summary>
		/// User clicked the tree view.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The event arguments.</param>
		void TreeViewMouseDown(object sender, MouseEventArgs e)
		{
			TreeNode node = treeView.GetNodeAt(e.X, e.Y);

			treeView.SelectedNode = node;
			
			state = NAntPadTreeViewState.Nothing;
			if (IsBuildFileNodeSelected) {
				state = NAntPadTreeViewState.BuildFileSelected;
			}
			
			if (IsBuildTargetNodeSelected) {
				state = NAntPadTreeViewState.TargetSelected;
			}
			
			if (IsBuildFileErrorNodeSelected) {
				state = NAntPadTreeViewState.ErrorSelected;
			}
		}
		
		/// <summary>
		/// Gets whether a build file is selected.
		/// </summary>
		bool IsBuildFileNodeSelected {
			get {
				return treeView.SelectedNode is NAntBuildFileTreeNode;
			}
		}
		
		/// <summary>
		/// Gets whether a target is selected.
		/// </summary>
		bool IsBuildTargetNodeSelected {
			get {
				return treeView.SelectedNode is NAntBuildTargetTreeNode;
			}
		}
		
		/// <summary>
		/// Gets whether a build file error is selected.
		/// </summary>
		bool IsBuildFileErrorNodeSelected {
			get {
				return treeView.SelectedNode is NAntBuildFileErrorTreeNode;
			}
		}
		
		/// <summary>
		/// Double clicking a node on the tree view opens the corresponding
		/// file.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The event arguments.</param>
		void TreeViewDoubleClick(object sender, EventArgs e)
		{
			NAntBuildFile buildFile = SelectedBuildFile;
			if (buildFile != null) {
				
				string fileName = Path.Combine(buildFile.Directory, buildFile.FileName);
				
				if (IsBuildTargetNodeSelected) {
					FileService.JumpToFilePosition(fileName, SelectedTarget.Line, SelectedTarget.Column);
				} else if (IsBuildFileErrorNodeSelected) {
					FileService.JumpToFilePosition(fileName, SelectedError.Line, SelectedError.Column);
				} else {
					FileService.OpenFile(fileName);
				}
			}
		}
		
		/// <summary>
		/// Looks for the tree node that is displaying the specified
		/// build file.
		/// </summary>
		/// <param name="fileName">The build file to look for.</param>
		/// <returns>The matching tree node if the build file exists
		/// in the tree; otherwise <see langword="null"/>.</returns>
		NAntBuildFileTreeNode FindMatchingNode(string fileName)
		{
			foreach (NAntBuildFileTreeNode node in treeView.Nodes) {
				string nodeFileName = Path.Combine(node.BuildFile.Directory, node.BuildFile.FileName);
				if (String.Compare(Path.GetFullPath(fileName), Path.GetFullPath(nodeFileName), true) == 0) {
					return node;
				}
			}
			return null;
		}
	}
}
