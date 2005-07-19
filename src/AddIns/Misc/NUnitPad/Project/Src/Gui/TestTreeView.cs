// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

using NUnit.Core;
using NUnit.Framework;

namespace ICSharpCode.NUnitPad
{
	/// <summary>
	/// Description of TestTreeView.	
	/// </summary>
	public class TestTreeView : System.Windows.Forms.UserControl, EventListener, IOwnerState
	{
		const string ContextMenuAddInTreePath = "/NUnitPad/TestTreeView/ContextMenu";
		
		[Flags]
		public enum TestTreeViewState {
			Nothing                = 0,
			SourceCodeItemSelected = 1,
			TestItemSelected       = 2,
		}
		
		sealed class TestItemTag {
			public readonly ITest Test;
			public readonly IProject Project;
			public TestItemTag(ITest test, IProject project) {
				this.Test = test;
				this.Project = project;
			}
			public void Run(EventListener listener) {
				if (Test is Test)
					(Test as Test).Run(listener);
			}
			public Position GetPosition() {
				return ParserService.GetProjectContent(Project).GetPosition(Test.FullName.Replace('+', '.'));
			}
		}
		
		protected TestTreeViewState internalState = TestTreeViewState.Nothing;

		public System.Enum InternalState {
			get {
				return internalState;
			}
		}
		
		TreeView  treeView;
		Hashtable treeNodeHash = new Hashtable();
		MessageViewCategory testRunnerCategory;
		
		
		bool IsSourceCodeItemSelected {
			get {
				if (treeView.SelectedNode == null) {
					return false;
				}
				TestItemTag test = treeView.SelectedNode.Tag as TestItemTag;
				if (test != null) {
					
					Position position = test.GetPosition();
					return position != null && position.Cu != null;
				}
				return false;
			}
		}
		
		bool IsTestItemSelected {
			get {
				if (treeView.SelectedNode == null) {
					return false;
				}
				TestItemTag test = treeView.SelectedNode.Tag as TestItemTag;
				return test != null;
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				treeView.Dispose();
			}
			base.Dispose(disposing);
		}
		
		public TestTreeView()
		{
			
			ImageList imageList = new ImageList();
			imageList.ColorDepth = ColorDepth.Depth32Bit;
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.TestRunner.Gray"));
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.TestRunner.Green"));
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.TestRunner.Yellow"));
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.TestRunner.Red"));
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.AboutIcon"));
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.Error"));
			
			this.treeView = new TreeView();
			treeView.ImageList = imageList;
			treeView.Dock = DockStyle.Fill;
			treeView.DoubleClick += new EventHandler(ActivateItem);
			treeView.KeyPress += new KeyPressEventHandler(TestTreeViewKeyPress);
			treeView.MouseDown += new MouseEventHandler(TreeViewMouseDown);
			treeView.HideSelection = false;
			Controls.Add(treeView);
			treeView.ContextMenuStrip = MenuService.CreateContextMenu(this, ContextMenuAddInTreePath);
		}
		void TreeViewMouseDown(object sender, MouseEventArgs e)
		{
			TreeNode node = treeView.GetNodeAt(e.X, e.Y);

			treeView.SelectedNode = node;
			
			internalState = TestTreeViewState.Nothing;
			if (IsSourceCodeItemSelected) {
				internalState |= TestTreeViewState.SourceCodeItemSelected;
			} 
			
			if (IsTestItemSelected) {
				internalState |= TestTreeViewState.TestItemSelected;
			}
		}
		
		public void SetAutoLoadState(bool state)
		{
			if (!state) {
				ClearTests();
				
				TreeNode noAutoLoad = new TreeNode(StringParser.Parse("${res:NUnitPad.NUnitPadContent.TestTreeView.ClickOnRunInformationNode}"));
				noAutoLoad.ImageIndex = noAutoLoad.SelectedImageIndex = 4;
				treeView.Nodes.Add(noAutoLoad);
			}
		}
		public void ClearTests()
		{
			if (!treeView.IsDisposed) {
				treeView.Nodes.Clear();
			}
		}
		
		public void PrintTestErrors(string assembly, Exception e)
		{
			TreeNode assemblyNode = new TreeNode(Path.GetFileName(assembly));
			
			TreeNode failedNode = new TreeNode(StringParser.Parse("${res:NUnitPad.NUnitPadContent.TestTreeView.LoadingErrorNode}") + ":" + e.Message);
			failedNode.ImageIndex = failedNode.SelectedImageIndex = 5;
			assemblyNode.Nodes.Add(failedNode);
			
			treeView.Nodes.Add(assemblyNode);
		}
		
		public void PrintTests(string assembly, Test test, IProject project)
		{
			TreeNode assemblyNode = new TreeNode(Path.GetFileName(assembly));
			assemblyNode.Tag = new TestItemTag(test, project);
			treeView.Nodes.Add(assemblyNode);
			if (test != null) {
				AddTests(assemblyNode, test, project);
			}
			assemblyNode.Expand();
		}
		
		public void AddTests(TreeNode node, ITest test, IProject project)
		{
			foreach (ITest childTest in test.Tests) {
				TreeNode newNode = new TreeNode(childTest.Name);
				treeNodeHash[childTest.UniqueName] = newNode;
				newNode.ImageIndex = newNode.SelectedImageIndex = 0;
				newNode.Tag = new TestItemTag(childTest, project);
				if (childTest.IsSuite) {
					AddTests(newNode, childTest, project);
					node.Expand();
				}
				node.Nodes.Add(newNode);
			}
		}
		
		public void GotoDefinition()
		{
			if (treeView.SelectedNode != null) {
				TestItemTag testTag = treeView.SelectedNode.Tag as TestItemTag;
				if (testTag != null) {
					
					Position position = testTag.GetPosition();
					
					if (position != null && position.Cu != null) {
						
						FileService.JumpToFilePosition(position.Cu.FileName, Math.Max(0, position.Line - 1), Math.Max(0, position.Column - 1));
					}
				}
			}
		}
		
		public void RunTests()
		{
			ResetNodeIcons(treeView.Nodes);
			
			if (testRunnerCategory == null) {
				testRunnerCategory = new MessageViewCategory("NUnit");
				CompilerMessageView cmv = (CompilerMessageView)WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).PadContent;
				cmv.AddCategory(testRunnerCategory);
			} else {
				testRunnerCategory.ClearText();
			}
			
			TaskService.Clear();
			
			TreeNode selectedNode = treeView.SelectedNode;
			
			if (selectedNode != null) {
				TestItemTag test = selectedNode.Tag as TestItemTag;
				if (test != null) {
					test.Run(this);
				} else {
					selectedNode.ImageIndex = selectedNode.SelectedImageIndex = 2;
				}
			} else {
				foreach (TreeNode node in treeView.Nodes) {
					TestItemTag test = node.Tag as TestItemTag;
					if (test != null) {
						test.Run(this);
					} else {
						node.ImageIndex = node.SelectedImageIndex = 2;
					}
				}
			}
			
			// If any tasks bring task view to front.
			if (TaskService.TaskCount > 0) {
				IWorkbench Workbench = ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench;
				PadDescriptor padDescriptor = Workbench.GetPad(typeof(ErrorList));
				
				if (padDescriptor != null) {
					padDescriptor.BringPadToFront();
				}
			}
			
			treeView.Focus();
		}
		
		void ResetNodeIcons(ICollection col) 
		{
			foreach (TreeNode node in col) {
				if (node.ImageIndex <= 3) {
					node.ImageIndex = node.SelectedImageIndex = 0;
				}
				ResetNodeIcons(node.Nodes);
			}
		}
		
		void SetResultIcon(TestResult testResult)
		{
			TreeNode node = (TreeNode)treeNodeHash[testResult.Test.UniqueName];
			if (node != null) {
				if (testResult.IsSuccess && testResult.Executed) {
					node.ImageIndex = node.SelectedImageIndex = 1;
				} else if (testResult.IsFailure) {
					node.ImageIndex = node.SelectedImageIndex = 3;
				} else {
					node.ImageIndex = node.SelectedImageIndex = 2;
				}
				if (node.Parent != null || node.Parent.Parent == null) {
					node.Parent.ImageIndex = node.Parent.SelectedImageIndex = node.ImageIndex;
				}
			}
		}
		
		
		void ActivateItem(object sender, EventArgs e)
		{
			GotoDefinition();
		}
		
		void TestTreeViewKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r') {
				GotoDefinition();
			} else if (e.KeyChar == ' ') {
				RunTests();
			}
		}
		
		#region NUnit.Core.EventListener interface implementation
		public void RunStarted(NUnit.Core.Test[] tests)
		{
			
		}
		public void RunFinished(NUnit.Core.TestResult[] tests)
		{
			
		}
		public void RunFinished(Exception exception)
		{
			
		}
		public void UnhandledException(Exception exception)
		{
			
		}
		
		public void SuiteStarted(NUnit.Core.TestSuite suite)
		{
//			testRunnerCategory.AppendText(suite.FullName + " started.\n");
		}
		
		public void SuiteFinished(NUnit.Core.TestSuiteResult result)
		{
			SetResultIcon(result);
//			testRunnerCategory.AppendText(result.Test.FullName + " finished.\n");
		}
		
		public void TestStarted(NUnit.Core.TestCase testCase)
		{
//			testRunnerCategory.AppendText(testCase.FullName + " started.\n");
		}
		
		public void TestFinished(NUnit.Core.TestCaseResult result)
		{
			
			if (!result.IsSuccess) {
				
				string outputMessage = StringParser.Parse("${res:NUnitPad.NUnitPadContent.TestTreeView.TestFailedMessage}", new string[,] {
				                          	{"TestCase", result.Test.FullName},
				                          	{"Message", result.Message.Replace("\t", " ").Trim()}
				                          }
				                         );
				
				testRunnerCategory.AppendText(outputMessage + Environment.NewLine);
				testRunnerCategory.AppendText(result.Description + Environment.NewLine);
				testRunnerCategory.AppendText(result.StackTrace + Environment.NewLine);
//				if (result.StackTrace != null ) {
//					Console.WriteLine("result.StackTrace=" + result.StackTrace);
//				}
//				else {
//					Console.WriteLine("result.StackTrace=null");
//				}
					
				FileLineReference LineRef = OutputTextLineParser.GetNUnitOutputFileLineReference(result.StackTrace, true);
				if (LineRef != null) {
//					Console.WriteLine("Adding NUnit error task.");
					Task Task = new Task(Path.GetFullPath(LineRef.FileName),
					                     outputMessage,
										 LineRef.Column,
										 LineRef.Line,
										 TaskType.Error);
					                     
					TaskService.Add(Task);
				}
			} else if (!result.Executed) {
				
				string outputMessage = StringParser.Parse("${res:NUnitPad.NUnitPadContent.TestTreeView.TestNotExecutedMessage}", new string[,] {
				                                                 	{"TestCase", result.Test.FullName}
				                                                 }
				                         );
				
				testRunnerCategory.AppendText(outputMessage + Environment.NewLine);
				testRunnerCategory.AppendText(result.Message + Environment.NewLine);
				testRunnerCategory.AppendText(result.Description + Environment.NewLine);
				testRunnerCategory.AppendText(result.StackTrace + Environment.NewLine);
//				if (result.StackTrace != null ) {
//					Console.WriteLine("result.StackTrace=" + result.StackTrace);
//				}
//				else {
//					Console.WriteLine("result.StackTrace=null");
//				}
				
				FileLineReference LineRef = OutputTextLineParser.GetNUnitOutputFileLineReference(result.StackTrace, true);
				if (LineRef != null) {
					
//					Console.WriteLine("Adding NUnit warning task.");
					Task Task = new Task(Path.GetFullPath(LineRef.FileName),
										 outputMessage,
										 LineRef.Column,
										 LineRef.Line,
										 TaskType.Warning);
					                     
					TaskService.Add(Task);
				}				
			}
			
			SetResultIcon(result);
		}
		#endregion
	}
}
