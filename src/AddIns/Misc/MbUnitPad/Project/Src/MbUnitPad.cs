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

namespace ICSharpCode.MbUnitPad
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
		ToolStrip toolStrip;
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
			toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/MbUnitPad/Toolbar");
			toolStrip.GripStyle = ToolStripGripStyle.Hidden;
			ctl.Controls.Add(toolStrip);
			
			ProjectService.SolutionLoaded += OnSolutionLoaded;
			ProjectService.SolutionClosed += OnSolutionClosed;
		}
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			ProjectService.SolutionLoaded -= OnSolutionLoaded;
			ProjectService.SolutionClosed -= OnSolutionClosed;
			ctl.Dispose();
		}
		
		void OnSolutionLoaded(object sender, EventArgs e)
		{
			ToolbarService.UpdateToolbar(toolStrip);
		}
		
		void OnSolutionClosed(object sender, EventArgs e)
		{
			ToolbarService.UpdateToolbar(toolStrip);
			treeView.NewConfig();
		}
		
		public void RunTests()
		{
			if (treeView.TypeTree.Nodes.Count == 0) {
				treeView.TreePopulated += StartTestsAfterTreePopulation;
				ReloadAssemblyList();
			} else {
				treeView.ThreadedRunTests();
			}
		}
		
		void StartTestsAfterTreePopulation(object sender, EventArgs e)
		{
			treeView.TreePopulated -= StartTestsAfterTreePopulation;
			// we cannot run the tests on this thread because we have to wait for the worker thread to exit
			WorkbenchSingleton.SafeThreadAsyncCall(treeView, "ThreadedRunTests");
		}
		
		public void ReloadAssemblyList()
		{
			treeView.TestDomains.Clear();
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
						if (string.Equals(include, "nunit.framework", StringComparison.OrdinalIgnoreCase)
						    || string.Equals(include, "mbunit.framework", StringComparison.OrdinalIgnoreCase))
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
			treeView.ThreadedPopulateTree(true);
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
