// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
#region License
//  
//  Copyright (c) 2007, ic#code
//  
//  All rights reserved.
//  
//  Redistribution  and  use  in  source  and  binary  forms,  with  or without
//  modification, are permitted provided that the following conditions are met:
//  
//  1. Redistributions  of  source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//  
//  2. Redistributions  in  binary  form  must  reproduce  the  above copyright
//     notice,  this  list  of  conditions  and the following disclaimer in the
//     documentation and/or other materials provided with the distribution.
//  
//  3. Neither the name of the ic#code nor the names of its contributors may be
//     used  to  endorse or promote products derived from this software without
//     specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND  ANY  EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//  ARE  DISCLAIMED.   IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
//  LIABLE  FOR  ANY  DIRECT,  INDIRECT,  INCIDENTAL,  SPECIAL,  EXEMPLARY,  OR
//  CONSEQUENTIAL  DAMAGES  (INCLUDING,  BUT  NOT  LIMITED  TO,  PROCUREMENT OF
//  SUBSTITUTE  GOODS  OR  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//  INTERRUPTION)  HOWEVER  CAUSED  AND  ON ANY THEORY OF LIABILITY, WHETHER IN
//  CONTRACT,  STRICT  LIABILITY,  OR  TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//  ARISING  IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//  POSSIBILITY OF SUCH DAMAGE.
//  
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Debugger;
using ICSharpCode.Core;

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
			IViewContent content = FileService.GetOpenFile(nextStatement.SourceFullFilename);
			
			if (content is IPositionable) {
				((IPositionable)content).JumpTo((int)nextStatement.StartLine - 1, (int)nextStatement.StartColumn - 1);
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
					location = ResourceService.GetString("Global.NA");
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
