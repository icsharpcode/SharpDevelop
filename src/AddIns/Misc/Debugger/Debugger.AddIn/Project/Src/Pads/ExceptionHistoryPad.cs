// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

using Debugger;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class ExceptionHistoryPad : DebuggerPad
	{
		ListView  exceptionHistoryList;
		Debugger.Process debuggedProcess;
		
		List<Debugger.Exception> exceptions = new List<Debugger.Exception>();
		
		ColumnHeader time      = new ColumnHeader();
		ColumnHeader exception = new ColumnHeader();
		ColumnHeader location  = new ColumnHeader();
		
		public override Control Control {
			get {
				return exceptionHistoryList;
			}
		}
		
		protected override void InitializeComponents()
		{
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
		}

		public override void RedrawContent()
		{
			time.Text      = ResourceService.GetString("MainWindow.Windows.Debug.ExceptionHistory.Time");
			exception.Text = ResourceService.GetString("MainWindow.Windows.Debug.ExceptionHistory.Exception");
			location.Text  = ResourceService.GetString("AddIns.HtmlHelp2.Location");
		}
		
		protected override void SelectProcess(Debugger.Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.ExceptionThrown -= debuggedProcess_ExceptionThrown;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.ExceptionThrown += debuggedProcess_ExceptionThrown;
			}
			exceptions.Clear();
			RefreshPad();
		}
		
		void debuggedProcess_ExceptionThrown(object sender, ExceptionEventArgs e)
		{
			exceptions.Add(e.Exception);
			RefreshPad();
		}
		
		void ExceptionHistoryListItemActivate(object sender, EventArgs e)
		{
			SourcecodeSegment nextStatement = ((Debugger.Exception)(exceptionHistoryList.SelectedItems[0].Tag)).Location;

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
			}
		}
		
		public override void RefreshPad()
		{
			exceptionHistoryList.BeginUpdate();
			exceptionHistoryList.Items.Clear();
			
			foreach(Debugger.Exception exception in exceptions) {
				string location;
				if (exception.Location != null) {
					location = exception.Location.SourceFilename + ":" + exception.Location.StartLine;
				} else {
					location = "n/a";
				}
				location += " (type=" + exception.ExceptionType.ToString() + ")";
				ListViewItem item = new ListViewItem(new string[] {exception.CreationTime.ToLongTimeString() , exception.Type + " - " + exception.Message, location});
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
