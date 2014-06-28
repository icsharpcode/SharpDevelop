// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Drawing;
using System.Windows.Forms;
using Debugger;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Services
{
	internal sealed partial class DebuggeeExceptionForm
	{
		Process process;
		
		public bool Break { get; set; }
		
		DebuggeeExceptionForm(Process process)
		{
			InitializeComponent();
			
			this.Break = true;
			
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
			this.btnContinue.Text  = StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Continue}");
			
			this.btnBreak.Image = WinFormsResourceService.GetBitmap("Icons.16x16.Debug.Break");
			this.btnStop.Image = WinFormsResourceService.GetBitmap("Icons.16x16.StopProcess");
			this.btnContinue.Image = WinFormsResourceService.GetBitmap("Icons.16x16.Debug.Continue");
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
		
		public static bool Show(Process process, string title, string type, string stacktrace, Bitmap icon, bool isUnhandled)
		{
			DebuggeeExceptionForm form = new DebuggeeExceptionForm(process);
			form.Text = title;
			form.pictureBox.Image = icon;
			form.lblExceptionText.Text = type;
			form.exceptionView.Text = stacktrace;
			form.btnContinue.Enabled = !isUnhandled;
			
			// Showing the form as dialg seems like a resonable thing in the presence of potentially multiple
			// concurent debugger evetns
			form.ShowDialog(SD.WinForms.MainWin32Window);
			
			return form.Break;
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
				if (index < 0)
					return;
				int start = index;
				// find start of current line
				while (start > 0 && fullText[start - 1] != '\n')
					start--;
				// find end of current line
				while (index < fullText.Length && fullText[index] != '\n')
					index++;
				
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
			this.Break = true;
			Close();
		}
		
		void BtnStopClick(object sender, EventArgs e)
		{
			this.process.Terminate();
			Close();
		}
		
		void BtnContinueClick(object sender, EventArgs e)
		{
			this.Break = false;
			Close();
		}
	}
}
