// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
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
		
		public bool IsPopulated {
			get {
				return TypeTree.Nodes.Count > 0;
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
			UnitTreeNode node = SelectedNode as UnitTreeNode;
			if (node != null) {
				string methodName = null;
				string fixtureName = null;
				switch (node.TestNodeType) {
					case TestNodeType.Test:
						methodName = GetTestMethodName(node.Text);
						fixtureName = node.Parent.Text;
						break;
						
					case TestNodeType.Fixture:
						fixtureName = node.Text;
						break;
				}
				
				if (fixtureName != null) {
					List<IClass> fixtures = FindTestFixtures(fixtureName);
					if (fixtures.Count > 0) {
						if (methodName == null) {
							// Go to fixture definition.
							IClass c = fixtures[0];
							if (c.CompilationUnit != null) {
								FileService.JumpToFilePosition(c.CompilationUnit.FileName, c.Region.BeginLine - 1, c.Region.BeginColumn - 1);
							}
						} else {
							foreach (IClass c in fixtures) {
								IMethod method = FindTestMethod(c, methodName);
								if (method != null) {
									FileService.JumpToFilePosition(c.CompilationUnit.FileName, method.Region.BeginLine - 1, method.Region.BeginColumn - 1);
									break;
								}
							}
						}
					}
				}
			}
		}
		
		static MessageViewCategory testRunnerCategory;

		string GetTestMethodName(string methodName)
		{
			int index = methodName.IndexOf(".");
			if (index >= 0) {
				return methodName.Substring(index + 1);
			}

			return methodName;
		}
		
		void OnTestsStarted(object sender, EventArgs e)
		{
			BeginInvoke(new EventHandler(OnTestsStartedInvoked));
		}
		
		void OnTestsStartedInvoked(object sender, EventArgs e)
		{
			PadDescriptor outputPad = WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView));
			
			if (testRunnerCategory == null) {
				testRunnerCategory = new MessageViewCategory("MbUnit");
				((CompilerMessageView)outputPad.PadContent).AddCategory(testRunnerCategory);
			} else {
				testRunnerCategory.ClearText();
			}
			
			outputPad.BringPadToFront();
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
		
		List<IClass> FindTestFixtures(string name)
		{
			List<IClass> fixtures = new List<IClass>();
			if (ProjectService.OpenSolution != null) {
				foreach (IProject project in ProjectService.OpenSolution.Projects) {
					IProjectContent projectContent = ParserService.GetProjectContent(project);
					if (projectContent != null) {
						foreach (IClass c in projectContent.Classes) {
							if (c.Name == name) {
								if (HasAttribute(c.Attributes, "TestFixture")) {
									fixtures.Add(c);
								}
							}
						}
					}
				}
			}
			return fixtures;
		}
		
		IMethod FindTestMethod(IClass c, string name)
		{
			foreach (IMethod method in c.Methods) {
				if (method.Name == name) {
					if (HasAttribute(method.Attributes, "Test")) {
					    return method;
					}
				}
			}
			return null;
		}
		
		bool HasAttribute(IList<IAttribute> attributes, string name)
		{
			foreach (IAttribute attr in attributes) {
				if (attr.Name == name) {
					return true;
				}
			}
			return false;
		}
	}
}
