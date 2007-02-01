/*
 * User: dickon
 * Date: 13/12/2006
 * Time: 16:02
 * 
 */

using System;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;
using System.Windows.Forms;

namespace SharpDbTools.Forms.TextEditor
{
	/// <summary>
	/// Description of SQLTextEditorControl.
	/// </summary>
	public class SQLTextEditorControl: TextEditorControl
	{
		public SQLTextEditorControl(): base()
		{
			this.ActiveTextAreaControl.TextArea.KeyEventHandler += new ICSharpCode.TextEditor.KeyEventHandler(KeyPressed);
		}
		
		
		protected override void InitializeTextAreaControl(TextAreaControl control)
		{
			LoggingService.Debug(this.GetType().Name + ": initialising TextArea for SQLTextEditorControl...");
			control.TextArea.KeyEventHandler += new ICSharpCode.TextEditor.KeyEventHandler(KeyPressed);
		}
		
		bool inKeyPressed = false;
		CodeCompletionWindow codeCompletionWindow = null;
		private bool KeyPressed(char next)
		{
			if (inKeyPressed) return false;
			inKeyPressed = true;
			LoggingService.Debug(this.GetType().Name + ": KeyPressed, handling character: " + next);
			try {
				// we already have a CodeCompletionWindow open, so it will handle
				// key presses at this point
				if (codeCompletionWindow != null && !codeCompletionWindow.IsDisposed) {
					return false; // not handling it yet
				}
				
	//			if (CodeCompletionOptions.EnableCodeCompletion) {
	//					foreach (ICodeCompletionBinding ccBinding in CodeCompletionBindings) {
	//						if (ccBinding.HandleKeyPress(this, ch))
	//							return false;
	//					}
	//			}
				
				// Lets just assume for now that we have only one binding, that is '.', which
				// will result in an attempt to show field name completions upon a table name
				// or alias
				
				if (next == '.') {
					ICompletionDataProvider completionDataProvider = new TestCodeCompletionProvider(); // TODO: create a simple provider that just returns a couple of strings
					codeCompletionWindow = 
						CodeCompletionWindow.ShowCompletionWindow((Form)WorkbenchSingleton.Workbench, 
						                                          this, this.FileName, completionDataProvider, next);
					if (codeCompletionWindow != null) {
						codeCompletionWindow.Closed += new EventHandler(CloseCodeCompletionWindow);
					}
					return false;
				}
				
			} catch(Exception ex) {
				LoggingService.Error(this.GetType().FullName, ex);
			} finally {
				inKeyPressed = false;
			}
			return false;
		}
		
		private void CloseCodeCompletionWindow(object sender, EventArgs args)
		{
			
		}
	}
	
	class TestCodeCompletionProvider : AbstractCompletionDataProvider, ICompletionDataProvider
	{
		/// <summary>
		/// Testing at this stage, aiming to get some test data into a code completion window
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="textArea"></param>
		/// <param name="charTyped"></param>
		/// <returns></returns>
		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			Random r = new Random();
			return new DefaultCompletionData[] { new TestCompletionData("Test" + r.Next(), "Test1", 0),
											     new TestCompletionData("Test" + r.Next(), "Test2", 0) };
		}
	}
	
	class TestCompletionData: DefaultCompletionData
	{		
		public TestCompletionData(string text, string description, int imageIndex): base(text, description, imageIndex)
		{
		}
	}
}
