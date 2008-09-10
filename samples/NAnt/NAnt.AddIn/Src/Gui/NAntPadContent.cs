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
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.NAnt.Gui
{
	/// <summary>
	/// The NAnt pad.
	/// </summary>
	public class NAntPadContent : AbstractPadContent
	{
		Panel contentPanel;
		NAntPadTreeView treeView;
		TextBox textBox;
		ToolStrip toolStrip;
		bool disposed;
		
		public NAntPadContent()
		{
			LoggingService.Debug("NAntPadContent.ctor");
			// Create main panel.
			contentPanel = new Panel();
			
			// Initialise treeview.
			treeView = new NAntPadTreeView();
			treeView.Dock = DockStyle.Fill;
			
			// Create ToolStrip.
			toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/NAntPad/Toolbar");
			toolStrip.GripStyle = ToolStripGripStyle.Hidden;
			
			// Create text box.
			textBox = new TextBox();
			textBox.WordWrap = false;
			textBox.Dock = DockStyle.Bottom;
			
			// Tooltip.
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(textBox, StringParser.Parse("Enter NAnt properties."));
			
			contentPanel.Controls.Add(treeView);
			contentPanel.Controls.Add(toolStrip);
			contentPanel.Controls.Add(textBox);
			
			ProjectService.SolutionLoaded += SolutionLoaded;
			ProjectService.SolutionClosed += SolutionClosed;
			ProjectService.ProjectItemRemoved += ProjectItemRemoved;
			ProjectService.ProjectItemAdded += ProjectItemAdded;
			WorkbenchSingleton.Workbench.ViewOpened += WorkbenchViewOpened;
			WorkbenchSingleton.Workbench.ViewClosed += WorkbenchViewClosed;
			FileService.FileRenamed += FileRenamed;
			FileService.FileRemoved += FileRemoved;
			FileUtility.FileSaved += FileSaved;
			
			NAntRunnerSingleton.Runner.NAntStarted += NAntStarted;
			NAntRunnerSingleton.Runner.NAntStopped += NAntStopped;
			NAntRunnerSingleton.Runner.NAntExited += NAntExited;
			
			// Due to lazy loading we have missed the solution loaded event
			// so add it now.
			Refresh();
		}
		
		/// <summary>
		/// Gets any extra command line arguments entered in the pad's text box.
		/// </summary>
		public string Arguments {
			get {
				return textBox.Text;
			}
		}
		
		public static NAntPadContent Instance {
			get {
				PadDescriptor descriptor = WorkbenchSingleton.Workbench.GetPad(typeof(NAntPadContent));
				return (NAntPadContent)descriptor.PadContent;
			}
		}

		/// <summary>
		/// Refreshes the contents NAnt pad.
		/// </summary>
		public void Refresh()
		{
			treeView.Clear();
			
			Solution solution = ProjectService.OpenSolution;
			if (solution != null) {
				treeView.AddSolution(solution);
			}
			foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (IsStandaloneNAntBuildFile(view.PrimaryFileName)) {
					treeView.AddBuildFile(String.Empty, view.PrimaryFileName);
				}
			}
		}
		
		/// <summary>
		/// Gets the currently selected <see cref="NAntBuildFile"/>.
		/// </summary>
		/// <remarks>This will return a NAntBuildFile if
		/// a target node is selected.</remarks>
		public NAntBuildFile SelectedBuildFile {
			get {
				return treeView.SelectedBuildFile;
			}
		}
		
		/// <summary>
		/// Gets the current selected <see cref="NAntBuildTarget"/>
		/// </summary>
		public NAntBuildTarget SelectedTarget {
			get {
				return treeView.SelectedTarget;
			}
		}
		
		#region AbstractPadContent requirements
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override Control Control {
			get {
				return contentPanel;
			}
		}
		
		/// <summary>
		/// Refreshes the pad
		/// </summary>
		public override void RedrawContent()
		{
		}
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			if (!disposed) {
				disposed = true;
				
				treeView.Dispose();
				contentPanel.Dispose();
				
				ProjectService.SolutionLoaded -= SolutionLoaded;
				ProjectService.SolutionClosed -= SolutionClosed;
				ProjectService.ProjectItemRemoved -= ProjectItemRemoved;
				ProjectService.ProjectItemAdded -= ProjectItemAdded;
				WorkbenchSingleton.Workbench.ViewOpened -= WorkbenchViewOpened;
				WorkbenchSingleton.Workbench.ViewClosed -= WorkbenchViewClosed;
				FileService.FileRenamed -= FileRenamed;
				FileService.FileRemoved -= FileRemoved;
				FileUtility.FileSaved -= FileSaved;
				
				NAntRunnerSingleton.Runner.NAntStarted -= NAntStarted;
				NAntRunnerSingleton.Runner.NAntStopped -= NAntStopped;
				NAntRunnerSingleton.Runner.NAntExited -= NAntExited;

				base.Dispose();
			}
		}
		
		#endregion
		
		void SolutionClosed(object sender, EventArgs e)
		{
			LoggingService.Debug("SolutionClosed.");
			treeView.Clear();
		}
		
		void SolutionLoaded(object sender, SolutionEventArgs e)
		{
			LoggingService.Debug("SolutionLoaded.");
			AddSolutionToPad(e.Solution);
		}
		
		void AddSolutionToPad(Solution solution)
		{
			if (solution != null) {
				treeView.AddSolution(solution);
			}
		}
		
		void UpdateToolbar()
		{
			ToolbarService.UpdateToolbar(toolStrip);
		}
		
		void FileRenamed(object sender, FileRenameEventArgs e)
		{
			if (!e.IsDirectory) {
				// source and target are populated.
				
				if (NAntBuildFile.IsBuildFile(e.SourceFile) && NAntBuildFile.IsBuildFile(e.TargetFile)) {
					treeView.RenameBuildFile(e.SourceFile, e.TargetFile);
				} else if (NAntBuildFile.IsBuildFile(e.SourceFile)) {
					treeView.RemoveBuildFile(e.SourceFile);
				} else {
					AddBuildFile(e.TargetFile);
				}
			}
		}
		
		void AddBuildFile(string fileName)
		{
			if (ProjectService.OpenSolution == null) return;
			IProject project = ProjectService.OpenSolution.FindProjectContainingFile(fileName);
			if (project != null) {
				treeView.AddBuildFile(project.Name, fileName);
			}
		}
		
		void ProjectItemAdded(object sender, ProjectItemEventArgs e)
		{
			LoggingService.Debug("ProjectItemAdded.");
			if (e.ProjectItem.ItemType != ItemType.Folder) {
				if (NAntBuildFile.IsBuildFile(e.ProjectItem.FileName)) {
					treeView.AddBuildFile(e.Project.Name, e.ProjectItem.FileName);
				}
			}
		}
		
		void ProjectItemRemoved(object sender, ProjectItemEventArgs e)
		{
			LoggingService.Debug("ProjectItemRemoved.");
			if (e.ProjectItem.ItemType != ItemType.Folder) {
				if (NAntBuildFile.IsBuildFile(e.ProjectItem.FileName)) {
					treeView.RemoveBuildFile(e.ProjectItem.FileName);
				}
			}
		}
		
		void WorkbenchViewOpened(object sender, ViewContentEventArgs e)
		{
			if (IsStandaloneNAntBuildFile(e.Content.PrimaryFileName)) {
				treeView.UpdateBuildFile(e.Content.PrimaryFileName);
			}
		}
		
		void WorkbenchViewClosed(object sender, ViewContentEventArgs e)
		{
			if (IsStandaloneNAntBuildFile(e.Content.PrimaryFileName)) {
				treeView.RemoveBuildFile(e.Content.PrimaryFileName);
			}
		}
		
		bool IsStandaloneNAntBuildFile(string fileName)
		{
			if (fileName != null) {
				return NAntBuildFile.IsBuildFile(fileName) && !IsInProject(fileName);
			}
			return false;
		}
		
		bool IsInProject(string fileName)
		{
			Solution solution = ProjectService.OpenSolution;
			if (solution != null) {
				foreach (IProject project in solution.Projects) {
					if (project.IsFileInProject(fileName)) {
						return true;
					}
				}
			}
			return false;
		}
		
		void FileSaved(object sender, FileNameEventArgs e)
		{
			LoggingService.Debug("FileSaved.");
			if (NAntBuildFile.IsBuildFile(e.FileName)) {
				treeView.UpdateBuildFile(e.FileName);
			}
		}
		
		void FileRemoved(object sender, FileEventArgs e)
		{
			LoggingService.Debug("FileRemoved.");
			if (NAntBuildFile.IsBuildFile(e.FileName)) {
				treeView.RemoveBuildFile(e.FileName);
			}
		}
		
		void NAntStarted(object sender, EventArgs e)
		{
			UpdateToolbar();
		}
		
		void NAntStopped(object sender, EventArgs e)
		{
			UpdateToolbar();
		}
		
		void NAntExited(object sender, NAntExitEventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(UpdateToolbar);
		}
	}
}
