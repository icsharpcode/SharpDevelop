// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Threading;
using System.Windows.Forms;

using NUnit.Util;
using NUnit.Core;
using NUnit.Framework;
using NUnit.Extensions;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.NUnitPad
{
	/// <summary>
	/// Description of the pad content
	/// </summary>
	public class NUnitPadContent : AbstractPadContent
	{
		TestTreeView testTreeView;
		Panel        contentPanel;
		bool         autoLoadItems = false;
		ArrayList testDomains = new ArrayList();
		
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
		/// Creates a new NUnitPadContent object
		/// </summary>
		public NUnitPadContent()
		{
			testTreeView = new TestTreeView();
			testTreeView.Dock = DockStyle.Fill;
			ToolStrip toolStrip = new ToolStrip();
			toolStrip.Dock = DockStyle.Top;
			toolStrip.Stretch   = true;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			
			ToolStripButton refreshItem = new ToolStripButton();
			refreshItem.ToolTipText  = StringParser.Parse("${res:NUnitPad.NUnitPadContent.RefreshItem}");
			refreshItem.Image = IconService.GetBitmap("Icons.16x16.BrowserRefresh");
			refreshItem.Click += new EventHandler(RefreshItemClick);
			toolStrip.Items.Add(refreshItem);
			
			ToolStripButton cancelItem = new ToolStripButton();
			cancelItem.ToolTipText  = StringParser.Parse("${res:NUnitPad.NUnitPadContent.CancelItem}");
			cancelItem.Image = IconService.GetBitmap("Icons.16x16.BrowserCancel");
			cancelItem.Click += new EventHandler(CancelItemClick);
			
			toolStrip.Items.Add(cancelItem);
			
			toolStrip.Items.Add(new ToolStripSeparator());
			
			ToolStripButton referenceItem = new ToolStripButton();
			referenceItem.ToolTipText  = StringParser.Parse("${res:NUnitPad.NUnitPadContent.ReferenceItem}");
			referenceItem.Image = IconService.GetBitmap("Icons.16x16.Reference");
			referenceItem.Click += new EventHandler(AddNUnitReference);
			toolStrip.Items.Add(referenceItem);
			
			toolStrip.Items.Add(new ToolStripSeparator());
			
			ToolStripButton runItem = new ToolStripButton();
			runItem.ToolTipText  = StringParser.Parse("${res:NUnitPad.NUnitPadContent.RunItem}");
			runItem.Image = IconService.GetBitmap("Icons.16x16.RunProgramIcon");
			runItem.Click += new EventHandler(RunItemClick);
			toolStrip.Items.Add(runItem);
			
			contentPanel = new Panel();
			contentPanel.Controls.Add(testTreeView);
			contentPanel.Controls.Add(toolStrip);
			
			ProjectService.SolutionLoaded += CombineEventHandler;
			ProjectService.SolutionClosed += ProjectServiceCombineClosed;
			ProjectService.StartBuild += ProjectServiceStartBuild;
			ProjectService.EndBuild   += ProjectServiceEndBuild;
			testTreeView.SetAutoLoadState(autoLoadItems);
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
			base.Dispose();
			UnloadAppDomains();
			testTreeView.Dispose();
			contentPanel.Dispose();
			ProjectService.StartBuild -= new EventHandler(ProjectServiceStartBuild);
			ProjectService.EndBuild   -= new EventHandler(ProjectServiceEndBuild);
		}
		#endregion
		
		void ProjectServiceStartBuild(object sender, EventArgs e)
		{
		}
		
		void CombineEventHandler(object sender, SolutionEventArgs e)
		{
			if (autoLoadItems) {
				RefreshProjectAssemblies();
			}
		}
		
		void ProjectServiceEndBuild(object sender, EventArgs e)
		{
			if (autoLoadItems) {
				RefreshProjectAssemblies();
			}
		}
		
		void AddNUnitReference(object sender, EventArgs e)
		{
			if (ProjectService.CurrentProject != null) {
				ProjectService.AddProjectItem(ProjectService.CurrentProject, new ReferenceProjectItem(ProjectService.CurrentProject, "nunit.framework"));
				ProjectService.CurrentProject.Save();
			}
		}
		
		void RunItemClick(object sender, EventArgs e)
		{
			RunTests();
		}
		
		void RefreshItemClick(object sender, EventArgs e)
		{
			autoLoadItems = true;
			RefreshProjectAssemblies();
		}
		
		void CancelItemClick(object sender, EventArgs e)
		{
			autoLoadItems = false;
			UnloadAppDomains();
			testTreeView.SetAutoLoadState(autoLoadItems);
		}
		
		void ProjectServiceCombineClosed(object sender, EventArgs e)
		{
			UnloadAppDomains();
		}
		
		void UnloadAppDomains()
		{
			/*foreach (TestDomain domain in testDomains) {
				try {
					domain.Unload();
				} catch (Exception) {}
			}*/
			testDomains.Clear();
			testTreeView.ClearTests();
		}
		
		public void RunTests()
		{
			if (ProjectService.OpenSolution == null) return;
			
			if (!autoLoadItems) {
				autoLoadItems = true;
				RefreshProjectAssemblies();
			}
			
			testTreeView.RunTests();
		}
		
		public void RefreshProjectAssemblies()
		{
			UnloadAppDomains();
			
			if (ProjectService.OpenSolution == null) {
				return;
			}
			
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				bool referenceFound = false;
				foreach (ProjectItem item in project.Items) {
					ReferenceProjectItem reference = item as ReferenceProjectItem;
					if (reference != null) {
						string include = reference.Include;
						if (include.IndexOf(',') > 0) {
							include = include.Substring(0, include.IndexOf(','));
						}
						if (string.Equals(include, "nunit.framework", StringComparison.InvariantCultureIgnoreCase)
						    || string.Equals(include, "nunit.framework.dll", StringComparison.InvariantCultureIgnoreCase))
						{
							referenceFound = true;
							break;
						}
					}
				}
				if (referenceFound) {
					string outputAssembly = project.OutputAssemblyFullPath;
					try {
						TestDomain testDomain = new TestDomain();
//					NUnitProject prj = NUnitProject.LoadProject(outputAssembly);
						Test test = testDomain.Load(outputAssembly);
						
//					TestSuiteBuilder builder = new TestSuiteBuilder();
//					Console.WriteLine("Try to load '" + outputAssembly +"'");
//					Test testDomain = builder.Build(outputAssembly);
						testTreeView.PrintTests(outputAssembly, test, project);
					} catch (Exception e) {
						testTreeView.PrintTestErrors(outputAssembly, e);
					}
				}
			}
		}
	}
}
