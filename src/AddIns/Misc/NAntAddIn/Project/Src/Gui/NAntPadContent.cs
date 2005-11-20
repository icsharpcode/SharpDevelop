// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.NAntAddIn;
using ICSharpCode.NAntAddIn.Commands;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop;
using System;
using System.Windows.Forms;

namespace ICSharpCode.NAntAddIn.Gui
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
			toolTip.SetToolTip(textBox, StringParser.Parse("${res:ICSharpCode.NAntAddIn.NAntPadContent.NAntPropertiesToolTip}"));			
			
			contentPanel.Controls.Add(treeView);
			contentPanel.Controls.Add(toolStrip);
			contentPanel.Controls.Add(textBox);
			
			ProjectService.SolutionLoaded += SolutionLoaded;
			ProjectService.SolutionClosed += SolutionClosed;
			ProjectService.ProjectItemRemoved += ProjectItemRemoved;
			ProjectService.ProjectItemAdded += ProjectItemAdded;
			FileService.FileRenamed += FileRenamed;
			FileService.FileRemoved += FileRemoved;
			FileUtility.FileSaved += FileSaved;
			
			NAntRunnerSingleton.Runner.NAntStarted += NAntStarted;
			NAntRunnerSingleton.Runner.NAntStopped += NAntStopped;
			NAntRunnerSingleton.Runner.NAntExited += NAntExited;
			
			// Due to lazy loading we have missed the solution loaded event
			// so add it now.
			
			AddSolutionToPad(ProjectService.OpenSolution);
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
			UpdateToolbar();
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
				UpdateToolbar();
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
			IProject project = ProjectService.GetProject(fileName);
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
			WorkbenchSingleton.SafeThreadAsyncCall(this, "UpdateToolbar");
		}
	}
}
