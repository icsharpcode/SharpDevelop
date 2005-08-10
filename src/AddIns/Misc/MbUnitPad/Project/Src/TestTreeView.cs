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
using MbUnit.Core.Remoting;
using MbUnit.Core.Reports.Serialization;

namespace MbUnitPad
{
	public class TestTreeView : ReflectorTreeView
	{
		Hashtable pipeNodes;
		
		public TestTreeView()
		{
			TypeTree.ContextMenu = null;
			TypeTree.ContextMenuStrip = MenuService.CreateContextMenu(this, "/MbUnitPad/ContextMenu");
			this.StartTests  += OnTestsStarted;
			this.FinishTests += delegate { BeginInvoke(new MethodInvoker(ExpandAllFailures)); };
			this.Facade.Updated += OnFacadeUpdated;
			// I don't see another good way to get a pipe node by GUID but stealing the facade's hashtable
			pipeNodes = (Hashtable)typeof(TestTreeNodeFacade).InvokeMember("pipeNodes", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic, null, Facade, null);
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
	}
}
