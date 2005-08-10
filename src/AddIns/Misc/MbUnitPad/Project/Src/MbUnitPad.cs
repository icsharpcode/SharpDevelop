// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using MbUnit.Forms;

namespace MbUnitPad
{
	public class MbUnitPadContent : AbstractPadContent
	{
		public static MbUnitPadContent Instance {
			get {
				PadDescriptor descriptor = WorkbenchSingleton.Workbench.GetPad(typeof(MbUnitPadContent));
				return (MbUnitPadContent)descriptor.PadContent;
			}
		}
		
		public static void BringToFront()
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(MbUnitPadContent)).BringPadToFront();
		}
		
		TestTreeView treeView;
		Control ctl;
		
		public TestTreeView TreeView {
			get {
				return treeView;
			}
		}
		
		/// <summary>
		/// Creates a new TestPad object
		/// </summary>
		public MbUnitPadContent()
		{
			ctl = new Panel();
			treeView = new TestTreeView();
			treeView.Dock = DockStyle.Fill;
			
			ctl.Controls.Add(treeView);
			ToolStrip toolStrip = ToolbarService.CreateToolStrip(this, "/MbUnitPad/Toolbar");
			toolStrip.GripStyle = ToolStripGripStyle.Hidden;
			ctl.Controls.Add(toolStrip);
			
			ProjectService.SolutionClosed += OnSolutionClosed;
		}
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			ProjectService.SolutionClosed -= OnSolutionClosed;
			ctl.Dispose();
		}
		
		void OnSolutionClosed(object sender, EventArgs e)
		{
			LoggingService.Info("Solution closed");
			treeView.RemoveAssemblies();
		}
		
		public void ReloadAssemblyList()
		{
			treeView.RemoveAssemblies();
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				bool referenceFound = false;
				foreach (ProjectItem item in project.Items) {
					ReferenceProjectItem reference = item as ReferenceProjectItem;
					if (reference != null) {
						string include = reference.Include;
						if (include.IndexOf(',') > 0) {
							include = include.Substring(0, include.IndexOf(','));
						}
						if (include.Length > 5) {
							if (include.Substring(include.Length - 4).Equals(".dll", StringComparison.OrdinalIgnoreCase))
								include = include.Substring(0, include.Length - 4);
						}
						if (string.Equals(include, "nunit.framework", StringComparison.InvariantCultureIgnoreCase)
						    || string.Equals(include, "mbunit.framework", StringComparison.InvariantCultureIgnoreCase))
						{
							referenceFound = true;
							break;
						}
					}
				}
				if (referenceFound) {
					string outputAssembly = project.OutputAssemblyFullPath;
					LoggingService.Debug("MbUnitPad: Load " + outputAssembly);
					try {
						treeView.AddAssembly(outputAssembly);
					} catch (Exception e) {
						LoggingService.Warn("MbUnitPad load error", e);
					}
				}
			}
		}
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override Control Control {
			get {
				return ctl;
			}
		}
		
		/// <summary>
		/// Refreshes the pad
		/// </summary>
		public override void RedrawContent()
		{
			
		}
	}
}
