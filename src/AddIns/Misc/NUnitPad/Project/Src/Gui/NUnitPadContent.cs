// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Threading;
using System.Windows.Forms;

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
			
			ProjectService.SolutionLoaded += new SolutionEventHandler(CombineEventHandler);
			ProjectService.SolutionClosed += new EventHandler(ProjectServiceCombineClosed);
			ProjectService.StartBuild += new EventHandler(ProjectServiceStartBuild);
			ProjectService.EndBuild += new EventHandler(ProjectServiceEndBuild);
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
				testTreeView.Invoke(new ThreadStart(RefreshProjectAssemblies));
			}
		}
		
		void AddNUnitReference(object sender, EventArgs e)
		{
			if (ProjectService.CurrentProject != null) {
				Console.WriteLine("Add reference!");
				ProjectService.AddReference(ProjectService.CurrentProject, new ReferenceProjectItem(ProjectService.CurrentProject, "nunit.framework"));
				ProjectService.CurrentProject.Save();
			} else {
				Console.WriteLine("prj == null");
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
			if (testDomains.Count > 0) {
				UnloadAppDomains();
			}
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
			if (!autoLoadItems) {
				autoLoadItems = true;
				RefreshProjectAssemblies();
			}
			testTreeView.RunTests();
		}
		
		public void RefreshProjectAssemblies()
		{
			UnloadAppDomains();
			
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				string outputAssembly = project.OutputAssemblyFullPath;
				try {
					TestSuiteBuilder builder = new TestSuiteBuilder();
					Test testDomain = builder.Build(outputAssembly);
					testTreeView.PrintTests(outputAssembly, testDomain);
				} catch (Exception e) {
					testTreeView.PrintTestErrors(outputAssembly, e);
				}
				
			}
		}
	}
}
