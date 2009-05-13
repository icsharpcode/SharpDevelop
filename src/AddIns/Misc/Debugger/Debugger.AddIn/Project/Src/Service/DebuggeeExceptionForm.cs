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

using ICSharpCode.Core.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Debugger;
using Debugger.AddIn.TreeModel;
using Debugger.Expressions;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Services
{
	internal sealed partial class DebuggeeExceptionForm
	{
		Process process;
		
		DebuggeeExceptionForm(Process process)
		{
			InitializeComponent();
			
			this.process = process;
			
			this.process.Exited += ProcessHandler;
			this.process.Resumed += ProcessHandler;
			
			this.FormClosed += FormClosedHandler;
			
			this.WindowState = DebuggingOptions.Instance.DebuggeeExceptionWindowState;
			FormLocationHelper.Apply(this, "DebuggeeExceptionForm", true);
			
			this.MinimizeBox = this.MaximizeBox = this.ShowIcon = false;
			
			this.exceptionView.Font = WinFormsResourceService.DefaultMonospacedFont;
			this.exceptionView.DoubleClick += ExceptionViewDoubleClick;
			this.exceptionView.WordWrap = false;
			
			this.btnBreak.Text = StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Break}");
			this.btnStop.Text  = StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Terminate}");
			
			this.btnBreak.Image = WinFormsResourceService.GetBitmap("Icons.16x16.Debug.Break");
			this.btnStop.Image = WinFormsResourceService.GetBitmap("Icons.16x16.StopProcess");
		}

		void ProcessHandler(object sender, EventArgs e)
		{
			this.Close();
		}
		
		void FormClosedHandler(object sender, EventArgs e)
		{
			this.process.Exited -= ProcessHandler;
			this.process.Resumed -= ProcessHandler;
		}
		
		public static void Show(Process process, string title, string message, string stacktrace, Bitmap icon)
		{
			DebuggeeExceptionForm form = new DebuggeeExceptionForm(process);
			form.Text = title;
			form.pictureBox.Image = icon;
			form.lblExceptionText.Text = message;
			form.exceptionView.Text = stacktrace;
			
			form.Show(WorkbenchSingleton.MainForm);
		}
		
		void ExceptionViewDoubleClick(object sender, EventArgs e)
		{
			string fullText = exceptionView.Text;
			// Any text?
			if (fullText.Length > 0) {
				//int line = textEditorControl.ActiveTextAreaControl.Caret.Line;
				//string textLine = TextUtilities.GetLineAsString(textEditorControl.Document, line);
				Point clickPos = exceptionView.PointToClient(Control.MousePosition);
				int index = exceptionView.GetCharIndexFromPosition(clickPos);
				int start = index;
				// find start of current line
				while (--start > 0 && fullText[start - 1] != '\n');
				// find end of current line
				while (++index < fullText.Length && fullText[index] != '\n');
				
				string textLine = fullText.Substring(start, index - start);
				
				FileLineReference lineReference = OutputTextLineParser.GetFileLineReference(textLine);
				if (lineReference != null) {
					// Open matching file.
					FileService.JumpToFilePosition(lineReference.FileName, lineReference.Line, lineReference.Column);
				}
			}
		}
		
		void FormResize(object sender, EventArgs e)
		{
			DebuggingOptions.Instance.DebuggeeExceptionWindowState = WindowState;
		}
		
		void BtnBreakClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void BtnStopClick(object sender, EventArgs e)
		{
			this.process.Terminate();
			Close();
		}
	}
}
