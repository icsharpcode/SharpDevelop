// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using MbUnit.Forms;
using MbUnit.Core;
using MbUnit.Core.Graph;
using MbUnit.Core.Remoting;
using MbUnit.Core.Reports.Serialization;

namespace ICSharpCode.MbUnitPad
{
	public class TestTreeView : ReflectorTreeView, IOwnerState
	{
		Hashtable pipeNodes;
		
		[Flags]
		public enum TestTreeViewState {
			Nothing                = 0,
			SourceCodeItemSelected = 1,
			TestItemSelected       = 2
		}
		
		protected TestTreeViewState internalState = TestTreeViewState.Nothing;

		public System.Enum InternalState {
			get {
				return internalState;
			}
		}
		
		public TestTreeView()
		{
			TypeTree.HideSelection = false;
			TypeTree.ContextMenu = null;
			TypeTree.ContextMenuStrip = MenuService.CreateContextMenu(this, "/SharpDevelop/Pads/MbUnitPad/ContextMenu");
			TypeTree.MouseDown += TreeViewMouseDown;
			this.StartTests  += OnTestsStarted;
			this.FinishTests += delegate { BeginInvoke(new MethodInvoker(ExpandAllFailures)); };
			this.Facade.Updated += OnFacadeUpdated;
			// I don't see another good way to get a pipe node by GUID but stealing the facade's hashtable
			pipeNodes = (Hashtable)typeof(TestTreeNodeFacade).InvokeMember("pipeNodes", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic, null, Facade, null);
		}
		
		public TreeNode SelectedNode {
			get {
				return TypeTree.SelectedNode;
			}
		}
		
		public void ExpandChildNode(TreeNode node)
		{
			TypeTree.BeginUpdate();
			if (node != null) {
           		node.Expand();
           		foreach(TreeNode child in node.Nodes) {
					ExpandChildNode(child);
           		}
			}
			TypeTree.EndUpdate();
		}
		
		public void CollapseChildNode(TreeNode node)
		{
			TypeTree.BeginUpdate();
			if (node != null) {
				node.Collapse();
				foreach(TreeNode child in node.Nodes) {
					CollapseChildNode(child);
				}
			}
			TypeTree.EndUpdate();
		}
		
		public void ExpandAll()
		{
			TypeTree.ExpandAll();
		}
		
		public void CollapseAll()
		{
			TypeTree.CollapseAll();
		}
		
		public void GotoDefinition()
		{
			MessageBox.Show("Not implemented.");
//			UnitTreeNode node = SelectedNode as UnitTreeNode;
//			if (node != null) {
//				string fullMemberName = null;
//				Fixture fixture = null;
//				switch (node.TestNodeType) {
//					case TestNodeType.Test:
//						fixture = FindFixture(node.DomainIdentifier, node.Parent.Text);
//						if (fixture != null)
//							fullMemberName = String.Concat(fixture.Type.FullName, ".", node.Text);						
//						break;
//						
//					case TestNodeType.Fixture:
//						fixture = FindFixture(node.DomainIdentifier, node.Text);
//						if (fixture != null) 
//							fullMemberName = fixture.Type.FullName;
//						break;
//				}
//				
//				LoggingService.Debug("MemberName=" + fullMemberName);
//				if (fullMemberName != null) {
//					foreach (IProjectContent projectContent in ParserService.AllProjectContents) {
//						LoggingService.Debug("Checking project content...");
//						Position pos = projectContent.GetPosition(fullMemberName);
//						if (pos != null && pos.Cu != null) {
//							LoggingService.Debug("Pos=" + pos.Line + ", " + pos.Column);
//							FileService.JumpToFilePosition(pos.Cu.FileName, Math.Max(0, pos.Line - 1), Math.Max(0, pos.Column - 1));
//							break;
//						}
//					}
//				}
//			}
		}
		
		static MessageViewCategory testRunnerCategory;
		
		void OnTestsStarted(object sender, EventArgs e)
		{
			BeginInvoke(new EventHandler(OnTestsStartedInvoked));
		}
		
		void OnTestsStartedInvoked(object sender, EventArgs e)
		{
			if (testRunnerCategory == null) {
				testRunnerCategory = new MessageViewCategory("MbUnit");
				CompilerMessageView cmv = (CompilerMessageView)WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).PadContent;
				cmv.AddCategory(testRunnerCategory);
			} else {
				testRunnerCategory.ClearText();
			}
		}
		
		void OnFacadeUpdated(ResultEventArgs e)
		{
			try {
				if (e.State == TestState.Failure) {
					IList list = (IList)pipeNodes[e.PipeIdentifier];
					for (int i = 0; i < list.Count; i++) {
						UnitTreeNode node = list[i] as UnitTreeNode;
						if (node != null) {
							AppendResult(this.TestDomains.GetResult(node));
							break;
						}
					}
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		void AppendResult(ReportRun result)
		{
			ReportException ex = result.Exception;
			string message = (ex != null) ? ex.Message : "-";
			string outputMessage = StringParser.Parse("${res:NUnitPad.NUnitPadContent.TestTreeView.TestFailedMessage}", new string[,] {
			                                          	{"TestCase", result.Name},
			                                          	{"Message", message}
			                                          });
			
			testRunnerCategory.AppendText(outputMessage + Environment.NewLine);
			testRunnerCategory.AppendText(result.Description + Environment.NewLine);
			if (ex != null) {
				testRunnerCategory.AppendText(ex.StackTrace + Environment.NewLine);
				
				FileLineReference LineRef = OutputTextLineParser.GetNUnitOutputFileLineReference(ex.StackTrace, true);
				if (LineRef != null) {
					Task task = new Task(Path.GetFullPath(LineRef.FileName),
					                     outputMessage,
					                     LineRef.Column,
					                     LineRef.Line,
					                     TaskType.Error);
					
					BeginInvoke(new AddTaskInvoker(TaskService.Add), new object[] { task });
				}
			}
		}
		
		delegate void AddTaskInvoker(Task task);
		
		/// <summary>
		/// Default MbUnit-GUI doesn't use shadow copy, we have to override that behaviour.
		/// </summary>
		public new void AddAssembly(string file)
		{
			if (this.TestDomains.ContainsTestAssembly(file)) {
				throw new ApplicationException(string.Format("The file {0} is already loaded.", file));
			} else {
				TreeTestDomain domain = this.TestDomains.Add(file);
				domain.ShadowCopyFiles = true;
				this.TestDomains.Watcher.Start();
			}
		}
		
		protected override void MessageOnStatusBar(string message, object[] args)
		{
			if (message.Length == 0) {
				StatusBarService.SetMessage(null);
			} else {
				string msg = string.Format(message, args);
				LoggingService.Debug(msg);
				StatusBarService.SetMessage("MbUnit: " + msg);
			}
		}
		
		void TreeViewMouseDown(object sender, MouseEventArgs e)
		{
			TreeNode node = TypeTree.GetNodeAt(e.X, e.Y);

			TypeTree.SelectedNode = node;
			
			internalState = TestTreeViewState.Nothing;
			if (IsSourceCodeItemSelected) {
				internalState |= TestTreeViewState.SourceCodeItemSelected;
			} 
			
			if (IsTestItemSelected) {
				internalState |= TestTreeViewState.TestItemSelected;
			}
		}
		
		bool IsSourceCodeItemSelected {
			get {
				UnitTreeNode node = TypeTree.SelectedNode as UnitTreeNode;
				if (node != null) {
					return (node.TestNodeType == TestNodeType.Test) || (node.TestNodeType == TestNodeType.Fixture);
				}
				return false;
			}
		}
		
		bool IsTestItemSelected {
			get {
				UnitTreeNode node = TypeTree.SelectedNode as UnitTreeNode;
				return node != null;
			}
		}
		
//		TreeTestDomain FindTestDomain(Guid id)
//		{
//			foreach (TreeTestDomain d in TestDomains) {
//				if (d.Identifier == id) {
//					return d;
//				}
//			}
//			return null;
//		}
//		
//		Fixture FindFixture(Guid testDomainId, string name)
//		{
//			TestDomain testDomain = FindTestDomain(testDomainId);
//			if (testDomain != null) {
//				FixtureDependencyGraph graph = testDomain.TestEngine.Explorer.FixtureGraph;
//				foreach (Fixture fixture in graph.Fixtures) {
//					if (fixture.Name == name) {
//						return fixture;
//					}
//				}
//			}
//			return null;
//		}
	}
}
