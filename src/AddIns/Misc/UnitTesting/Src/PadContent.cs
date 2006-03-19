// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class PadContent : AbstractPadContent
	{
		public static PadContent Instance {
			get {
				PadDescriptor descriptor = WorkbenchSingleton.Workbench.GetPad(typeof(PadContent));
				return (PadContent)descriptor.PadContent;
			}
		}
		
		public static void BringToFront()
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(PadContent)).BringPadToFront();
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
		public PadContent()
		{
			ctl = new Panel();
			treeView = new TestTreeView();
			treeView.Dock = DockStyle.Fill;
			treeView.RunStarted  += ThreadedUpdateToolbar;
			treeView.RunFinished += ThreadedUpdateToolbar;
			treeView.RunFinished += delegate {
				WorkbenchSingleton.SafeThreadAsyncCall((MethodInvoker)ShowErrorList);
			};
			
			ctl.Controls.Add(treeView);
			toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/UnitTestingPad/Toolbar");
			toolStrip.GripStyle = ToolStripGripStyle.Hidden;
			ctl.Controls.Add(toolStrip);
			
			ProjectService.SolutionLoaded += OnSolutionLoaded;
			ProjectService.SolutionClosed += OnSolutionClosed;
			ProjectService.EndBuild += OnEndBuild;
		}
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			ProjectService.SolutionLoaded -= OnSolutionLoaded;
			ProjectService.SolutionClosed -= OnSolutionClosed;
			ProjectService.EndBuild -= OnEndBuild;
			ctl.Dispose();
		}
		
		void OnSolutionLoaded(object sender, EventArgs e)
		{
			UpdateToolbar();
		}
		
		void OnSolutionClosed(object sender, EventArgs e)
		{
			treeView.UnloadTestAssemblies();
			UpdateToolbar();
		}
		
		void OnEndBuild(object sender, EventArgs e)
		{
			if (autoLoadItems) {
				ReloadAssemblyList(null);
			}
		}
		
		bool autoLoadItems;
		
		public void RunTests()
		{
			LoadAssemblyList(treeView.RunTests);
		}
		
		public void UnloadTests()
		{
			autoLoadItems = false;
			treeView.UnloadTestAssemblies();
			UpdateToolbar();
		}
		
		public bool IsRunningTests {
			get {
				return treeView.IsTestRunning;
			}
		}
		
		public void StopTests()
		{
			treeView.StopTests();
			UpdateToolbar();
		}
		
		/// <summary>
		/// Loads the assemblies if they are not already loaded.
		/// </summary>
		public void LoadAssemblyList(MethodInvoker callback)
		{
			if (autoLoadItems) {
				if (callback != null)
					WorkbenchSingleton.SafeThreadAsyncCall(callback);
			} else {
				ReloadAssemblyList(callback);
			}
		}
		
		public void ReloadAssemblyList(MethodInvoker callback)
		{
			autoLoadItems = true;
			treeView.UnloadTestAssemblies();
			
			treeView.StartAddingAssemblies();
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				bool referenceFound = false;
				foreach (ProjectItem item in project.Items) {
					ReferenceProjectItem reference = item as ReferenceProjectItem;
					if (reference != null) {
						string include = reference.Include;
						if (reference is ProjectReferenceProjectItem) {
							include = ((ProjectReferenceProjectItem)reference).ProjectName;
						}
						if (include.IndexOf(',') > 0) {
							include = include.Substring(0, include.IndexOf(','));
						}
						if (include.Length > 5) {
							if (include.Substring(include.Length - 4).Equals(".dll", StringComparison.OrdinalIgnoreCase))
								include = include.Substring(0, include.Length - 4);
						}
						if (string.Equals(include, "nunit.framework", StringComparison.OrdinalIgnoreCase))
							//|| string.Equals(include, "mbunit.framework", StringComparison.OrdinalIgnoreCase))
						{
							referenceFound = true;
							break;
						}
					}
				}
				if (referenceFound) {
					string outputAssembly = project.OutputAssemblyFullPath;
					LoggingService.Debug("UnitTestingPad: Load " + outputAssembly);
					treeView.AddAssembly(project, outputAssembly);
				}
			}
			treeView.FinishAddingAssemblies();
			if (callback != null)
				WorkbenchSingleton.SafeThreadAsyncCall(callback);
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
		
		void ThreadedUpdateToolbar(object sender, EventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall((MethodInvoker)UpdateToolbar);
		}
		
		void UpdateToolbar()
		{
			ToolbarService.UpdateToolbar(toolStrip);
		}
		
		void ShowErrorList()
		{
			if (TaskService.SomethingWentWrong && ErrorListPad.ShowAfterBuild) {
				WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
			}
		}
		
		public void RunTest(IProject project, IClass fixture, IMember test)
		{
			LoadAssemblyList(delegate {
			                 	treeView.SelectTest(project, fixture, test);
			                 	treeView.RunTests();
			                 });
		}
	}
}
