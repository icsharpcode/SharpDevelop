// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
//	   <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

using DebuggerLibrary;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class ExceptionHistoryPad : AbstractPadContent
	{
		WindowsDebugger debugger;
		NDebugger debuggerCore;

		ListView  exceptionHistoryList;
		
		ColumnHeader time      = new ColumnHeader();
		ColumnHeader exception = new ColumnHeader();
		ColumnHeader location  = new ColumnHeader();
		
		public override Control Control {
			get {
				return exceptionHistoryList;
			}
		}
		
		public ExceptionHistoryPad()
		{
			InitializeComponents();	
		}		
				
		void InitializeComponents()
		{
			debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
			
			exceptionHistoryList = new ListView();
			exceptionHistoryList.FullRowSelect = true;
			exceptionHistoryList.AutoArrange = true;
			exceptionHistoryList.Alignment   = ListViewAlignment.Left;
			exceptionHistoryList.View = View.Details;
			exceptionHistoryList.Dock = DockStyle.Fill;
			exceptionHistoryList.GridLines  = false;
			exceptionHistoryList.Activation = ItemActivation.OneClick;
			exceptionHistoryList.Columns.AddRange(new ColumnHeader[] {time, exception, location} );
			exceptionHistoryList.ItemActivate += new EventHandler(ExceptionHistoryListItemActivate);
			exception.Width = 300;
			location.Width = 400;
			time.Width = 80;
			
			RedrawContent();

			if (debugger.ServiceInitialized) {
				InitializeDebugger();
			} else {
				debugger.Initialize += delegate {
					InitializeDebugger();
				};
			}
		}

		public void InitializeDebugger()
		{
			debuggerCore = debugger.DebuggerCore;

			debuggerCore.IsDebuggingChanged += new EventHandler<DebuggerEventArgs>(DebuggerStateChanged);
			debuggerCore.IsProcessRunningChanged += new EventHandler<DebuggerEventArgs>(DebuggerStateChanged);

			RefreshList();
		}
		
		public override void RedrawContent()
		{
			time.Text        = "Time";
			exception.Text   = "Exception";
			location.Text    = "Location";
		}
		
		void ExceptionHistoryListItemActivate(object sender, EventArgs e)
		{
			SourcecodeSegment nextStatement = ((DebuggerLibrary.Exception)(exceptionHistoryList.SelectedItems[0].Tag)).Location;

			if (nextStatement == null) {
				return;
			}
			
			FileService.OpenFile(nextStatement.SourceFullFilename);
			IWorkbenchWindow window = FileService.GetOpenFile(nextStatement.SourceFullFilename);
			if (window != null) {
				IViewContent content = window.ViewContent;
			
				if (content is IPositionable) {
					((IPositionable)content).JumpTo((int)nextStatement.StartLine - 1, (int)nextStatement.StartColumn - 1);
				}
				
				/*if (content.Control is TextEditorControl) {
					IDocument document = ((TextEditorControl)content.Control).Document;
					LineSegment line = document.GetLineSegment((int)nextStatement.StartLine - 1);
					int offset = line.Offset + (int)nextStatement.StartColumn;
					currentLineMarker = new TextMarker(offset, (int)nextStatement.EndColumn - (int)nextStatement.StartColumn, TextMarkerType.SolidBlock, Color.Yellow);
					currentLineMarkerParent = document;
					currentLineMarkerParent.MarkerStrategy.TextMarker.Add(currentLineMarker);
					document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
					document.CommitUpdate();
				}*/
			}
		}

		public void DebuggerStateChanged(object sender, DebuggerEventArgs e)
		{
			RefreshList();
		}
			
		public void RefreshList()
		{
			exceptionHistoryList.BeginUpdate();
			exceptionHistoryList.Items.Clear();
		
			foreach(DebuggerLibrary.Exception exception in debugger.ExceptionHistory) {
				ListViewItem item = new ListViewItem(new string[] {exception.CreationTime.ToLongTimeString() , exception.Type + " - " + exception.Message, exception.Location.SourceFilename + ":" + exception.Location.StartLine + " (type=" + exception.ExceptionType.ToString() + ")"});
				item.Tag = exception;
				item.ForeColor = Color.Black;
				if (exception.ExceptionType == ExceptionType.DEBUG_EXCEPTION_UNHANDLED) {
					item.ForeColor = Color.Red;
				}
				if (exception.ExceptionType == ExceptionType.DEBUG_EXCEPTION_FIRST_CHANCE ||
					exception.ExceptionType == ExceptionType.DEBUG_EXCEPTION_USER_FIRST_CHANCE) {
					item.ForeColor = Color.Blue;
				}				
				exceptionHistoryList.Items.Add(item);
			}

			exceptionHistoryList.EndUpdate();
		}
	}
}
