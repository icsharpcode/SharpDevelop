// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

using NUnit.Core;
using NUnit.UiKit;
using NUnit.Util;

namespace ICSharpCode.UnitTesting
{
	public class TestTreeView : TestSuiteTreeView, IOwnerState
	{
		TestLoader loader = new TestLoader();
		static MessageViewCategory testRunnerCategory;
		bool runningTests;
		
		[Flags]
		public enum TestTreeViewState {
			Nothing                 = 0,
			SourceCodeItemSelected  = 1,
			TestItemSelected        = 2
		}
		
		System.Enum IOwnerState.InternalState {
			get {
				TestTreeViewState state = TestTreeViewState.Nothing;
				UITestNode test = SelectedTest;
				if (test != null) {
					state |= TestTreeViewState.TestItemSelected;
					if (test.IsTestCase || test.IsFixture) {
						state |= TestTreeViewState.SourceCodeItemSelected;
					}
				}
				return state;
			}
		}
		
		public static MessageViewCategory TestRunnerCategory {
			get {
				if (testRunnerCategory == null) {
					testRunnerCategory = new MessageViewCategory("${res:ICSharpCode.NUnitPad.NUnitPadContent.PadName}");
					CompilerMessageView.Instance.AddCategory(testRunnerCategory);
				}
				return testRunnerCategory;
			}
		}
		
		void OnRunStarting()
		{
			TaskService.ClearExceptCommentTasks();
			TestRunnerCategory.ClearText();
			PadDescriptor outputPad = WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView));
			outputPad.BringPadToFront();
		}
		
		public TestTreeView()
		{
			loader.ReloadOnChange = false;
			loader.ReloadOnRun = false;
			loader.Events.TestLoadFailed += delegate(object sender, TestEventArgs e) {
				TestRunnerCategory.AppendText("Loading " + e.Name + " failed: " + e.Exception.ToString());
				if (e.Exception is InvalidCastException && e.Exception.Message.Contains(typeof(AssemblyResolver).FullName)) {
					MessageService.ShowMessage("Loading tests failed, make sure NUnit.Core is in the Global Assembly Cache.");
				} else {
					MessageService.ShowMessage("Loading " + e.Name + " failed: " + e.Exception.ToString());
				}
			};
			loader.Events.TestLoaded += delegate(object sender, TestEventArgs e) {
				WorkbenchSingleton.SafeThreadAsyncCall(UpdateProjectTitles);
			};
			loader.Events.RunStarting += delegate(object sender, TestEventArgs e) {
				WorkbenchSingleton.SafeThreadAsyncCall(OnRunStarting);
			};
			loader.Events.TestOutput += delegate(object sender, TestEventArgs e) {
				// This method interceps StdOut/StdErr from the tests
				TestRunnerCategory.AppendText(e.TestOutput.Text);
			};
			loader.Events.RunFinished += delegate(object sender, TestEventArgs e) {
				runningTests = false;
			};
			loader.Events.TestFinished += delegate(object sender, TestEventArgs e) {
				TestResult result = e.Result;
				if (!result.IsSuccess) {
					string outputMessage = StringParser.Parse("${res:NUnitPad.NUnitPadContent.TestTreeView.TestFailedMessage}", new string[,] {
					                                          	{"TestCase", result.Test.FullName},
					                                          	{"Message", result.Message.Replace("\t", " ").Trim()}
					                                          });
					
					TestRunnerCategory.AppendText(Environment.NewLine + outputMessage + Environment.NewLine);
					if (!string.IsNullOrEmpty(result.Description)) {
						TestRunnerCategory.AppendText(result.Description + Environment.NewLine);
					}
					TestRunnerCategory.AppendText(result.StackTrace + Environment.NewLine);
					
					AddTask(TaskType.Error, outputMessage, result.Test.FullName, result.StackTrace);
				} else if (!result.Executed) {
					string outputMessage = StringParser.Parse("${res:NUnitPad.NUnitPadContent.TestTreeView.TestNotExecutedMessage}", new string[,] {
					                                          	{"TestCase", result.Test.FullName}
					                                          });
					
					testRunnerCategory.AppendText(Environment.NewLine + outputMessage + Environment.NewLine);
					testRunnerCategory.AppendText(result.Message + Environment.NewLine);
					if (!string.IsNullOrEmpty(result.Description)) {
						TestRunnerCategory.AppendText(result.Description + Environment.NewLine);
					}
					testRunnerCategory.AppendText(result.StackTrace + Environment.NewLine);
					
					AddTask(TaskType.Warning, outputMessage, result.Test.FullName, result.StackTrace);
				}
			};
			Initialize(loader, loader.Events);
			this.ContextMenu = null;
			this.ContextMenuStrip = MenuService.CreateContextMenu(this, "/SharpDevelop/Pads/UnitTestingPad/ContextMenu");
		}
		
		void UpdateProjectTitles()
		{
			if (Nodes.Count == 0)
				return;
			TreeNode root = Nodes[0];
			root.Text = ProjectService.OpenSolution.Name;
			foreach (TreeNode project in root.Nodes) {
				if (Path.IsPathRooted(project.Text)) {
					project.Text = Path.GetFileNameWithoutExtension(project.Text);
				}
			}
		}
		
		static void AddTask(TaskType type, string message, string fullTestName, string stackTrace)
		{
			Task task = CreateTask(type, message, fullTestName, stackTrace);
			if (task != null) {
				WorkbenchSingleton.SafeThreadAsyncCall(TaskService.Add, task);
			}
		}
		
		internal static Task CreateTask(TaskType type, string message, string fullTestName, string stackTrace)
		{
			FileLineReference lineRef = OutputTextLineParser.GetNUnitOutputFileLineReference(stackTrace, true);
			if (lineRef == null) {
				lineRef = FindTest(fullTestName);
			}
			if (lineRef != null) {
				return new Task(Path.GetFullPath(lineRef.FileName),
				                message, lineRef.Column, lineRef.Line, type);
			} else {
				return null;
			}
		}
		
		public void StopTests()
		{
			loader.CancelTestRun();
		}
		
		public event TestEventHandler RunStarted {
			add    { loader.Events.RunStarting += value; }
			remove { loader.Events.RunStarting -= value; }
		}
		
		public event TestEventHandler RunFinished {
			add    { loader.Events.RunFinished += value; }
			remove { loader.Events.RunFinished -= value; }
		}
		
		public bool IsTestRunning {
			get {
				return runningTests;
			}
		}
		
		NUnitProject project;
		
		public void StartAddingAssemblies()
		{
			project = NUnitProject.NewProject();
			project.ProjectPath = ProjectService.OpenSolution.Directory;
			// assemblies outside the BasePath cannot be loaded by .NET, so use the root of the drive
			project.BasePath = Path.GetPathRoot(ProjectService.OpenSolution.Directory);
		}
		
		public void FinishAddingAssemblies()
		{
			loader.TestProject = project;
			loader.LoadTest();
			project = null;
		}
		
		public void AddAssembly(IProject sdProject, string assemblyFile)
		{
			if (!File.Exists(assemblyFile)) {
				TestRunnerCategory.AppendText(assemblyFile + " does not exist.");
				return;
			}
			
			#if DEBUG
			// NUnit uses cross thread calls.......
			System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
			#endif
			
			if (project != null) {
				project.ActiveConfig.Assemblies.Add(assemblyFile);
			} else {
				if (!loader.IsProjectLoaded) {
					StartAddingAssemblies();
					loader.TestProject = project;
					project = null;
				}
				
				loader.TestProject.ActiveConfig.Assemblies.Add(assemblyFile);
			}
		}
		
		public void UnloadTestAssemblies()
		{
			if (loader.IsProjectLoaded) {
				loader.UnloadProject();
			}
		}
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right) {
				// required to run context menu commands on the correct item:
				this.SelectedNode = GetNodeAt(e.Location);
			}
			base.OnMouseDown(e);
		}
		
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r') {
				e.Handled = true;
				GotoDefinition();
			} else if (e.KeyChar == ' ') {
				e.Handled = true;
				RunTests();
			} else {
				base.OnKeyPress(e);
			}
		}
		
		public void SelectTest(IProject project, IClass fixture, IMember test)
		{
			if (Nodes.Count == 0) {
				return;
			}
			TreeNode root = Nodes[0];
			TreeNode nodeToSelect = null;
			foreach (TreeNode projectNode in root.Nodes) {
				if (projectNode.Text == project.Name) {
					nodeToSelect = projectNode;
					break;
				}
			}
			if (nodeToSelect != null && fixture != null) {
				// find fixture node
				TestSuiteTreeNode node = FindTest(nodeToSelect, fixture);
				if (node != null) {
					nodeToSelect = node;
					if (test != null) {
						node = FindTest(node, test);
						if (node != null) {
							nodeToSelect = node;
						}
					}
				}
			}
			this.SelectedNode = nodeToSelect;
		}
		
		TestSuiteTreeNode FindTest(TestSuiteTreeNode fixtureNode, IMember test)
		{
			foreach (TestSuiteTreeNode node in fixtureNode.Nodes) {
				if (node.Text == test.Name)
					return node;
			}
			return null;
		}
		
		TestSuiteTreeNode FindTest(TreeNode projectNode, IClass fixture)
		{
			foreach (TreeNode node in projectNode.Nodes) {
				TestSuiteTreeNode testNode = node as TestSuiteTreeNode;
				if (testNode != null) {
					if (testNode.Test.IsFixture) {
						if (testNode.Test.FullName == fixture.FullyQualifiedName)
							return testNode;
					} else {
						testNode = FindTest(testNode, fixture);
						if (testNode != null)
							return testNode;
					}
				}
			}
			return null;
		}
		
		public new void RunTests()
		{
			if (Nodes.Count > 0) {
				runningTests = true;
				base.RunTests();
			}
		}
		
		protected override void OnDoubleClick(EventArgs e)
		{
			GotoDefinition();
		}
		
		public void GotoDefinition()
		{
			UITestNode node = SelectedTest;
			if (node != null) {
				FileLineReference lr = null;
				if (node.IsFixture) {
					lr = FindTest(node.FullName, null);
				} else if (node.IsTestCase) {
					lr = FindTest(node.FullName);
				}
				if (lr != null) {
					FileService.JumpToFilePosition(lr.FileName, lr.Line, lr.Column);
				}
			}
		}
		
		public IProject SelectedProject {
			get {
				IClass fixture = SelectedFixtureClass;
				if (fixture != null)
					return (IProject)fixture.ProjectContent.Project;
				TreeNode node = SelectedNode;
				if (node == null)
					return null;
				if (node.Level == 0 && node.Nodes.Count == 1) {
					// solution node clicked, but has only 1 project
					node = node.Nodes[0];
				}
				while (node.Level >= 2 && node.Parent.Nodes.Count == 1) {
					// namespace node clicked, but project has only 1 namespace
					node = node.Parent;
				}
				if (node.Level == 1) {
					foreach (IProject p in ProjectService.OpenSolution.Projects) {
						if (Path.GetFileNameWithoutExtension(p.OutputAssemblyFullPath) == node.Text) {
							return p;
						}
					}
				}
				return null;
			}
		}
		
		public IClass SelectedFixtureClass {
			get {
				IMember member = SelectedTestMethod;
				if (member != null)
					return member.DeclaringType;
				UITestNode node = SelectedTest;
				if (node != null && node.IsFixture) {
					foreach (IClass fixture in FindTestFixtures(node.FullName)) {
						return fixture;
					}
				}
				return null;
			}
		}
		
		public IMember SelectedTestMethod {
			get {
				UITestNode node = SelectedTest;
				if (node != null && node.IsTestCase) {
					string fullMethodName = node.FullName;
					int pos = fullMethodName.LastIndexOf('.');
					string shortName = fullMethodName.Substring(pos + 1);
					foreach (IClass fixture in FindTestFixtures(fullMethodName.Substring(0, pos))) {
						IMember m = FindTestMethod(fixture, shortName);
						if (m != null)
							return m;
					}
				}
				return null;
			}
		}
		
		static FileLineReference FindTest(string fullMethodName)
		{
			int pos = fullMethodName.LastIndexOf('.');
			return FindTest(fullMethodName.Substring(0, pos), fullMethodName.Substring(pos + 1));
		}
		
		static FileLineReference FindTest(string fixtureName, string methodName)
		{
			if (fixtureName != null) {
				List<IClass> fixtures = FindTestFixtures(fixtureName);
				if (fixtures.Count > 0) {
					if (methodName == null) {
						// Go to fixture definition.
						IClass c = fixtures[0];
						if (c.CompilationUnit != null) {
							return new FileLineReference(c.CompilationUnit.FileName, c.Region.BeginLine - 1, c.Region.BeginColumn - 1);
						}
					} else {
						foreach (IClass c in fixtures) {
							IMethod method = FindTestMethod(c, methodName);
							if (method != null) {
								return new FileLineReference(c.CompilationUnit.FileName, method.Region.BeginLine - 1, method.Region.BeginColumn - 1);
							}
						}
					}
				}
			}
			return null;
		}
		
		static List<IClass> FindTestFixtures(string fullName)
		{
			List<IClass> fixtures = new List<IClass>();
			if (ProjectService.OpenSolution != null) {
				foreach (IProject project in ProjectService.OpenSolution.Projects) {
					IProjectContent projectContent = ParserService.GetProjectContent(project);
					if (projectContent != null) {
						IClass c = projectContent.GetClass(fullName, 0, projectContent.Language, false);
						if (c != null) {
							fixtures.Add(c);
						}
					}
				}
			}
			return fixtures;
		}
		
		static IMethod FindTestMethod(IClass c, string name)
		{
			return c.Methods.Find(delegate(IMethod m) { return m.Name == name; });
		}
	}
}
