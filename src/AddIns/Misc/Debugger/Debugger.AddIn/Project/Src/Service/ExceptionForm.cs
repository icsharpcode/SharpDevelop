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
using System.Text;

using Debugger;
using ICSharpCode.Core;


namespace ICSharpCode.SharpDevelop.Services
{
	internal sealed partial class ExceptionForm
	{
		public enum Result {Break, Continue, Terminate};
		
		private Result result = Result.Break; // Default
		
		private ExceptionForm()
		{
			InitializeComponent();
			this.Text = StringParser.Parse(this.Text);
			buttonBreak.Text = StringParser.Parse(buttonBreak.Text);
			buttonContinue.Text = StringParser.Parse(buttonContinue.Text);
			buttonTerminate.Text = StringParser.Parse(buttonTerminate.Text);
		}
		
		public static Result Show(Debugger.Exception exception)
		{
			IDebugeeException ex = exception;
			StringBuilder msg = new StringBuilder();
			msg.AppendFormat(ResourceService.GetString("MainWindow.Windows.Debug.ExceptionForm.Message"), ex.Type);
			msg.AppendLine();
			msg.AppendLine("Message:");
			msg.Append("\t");
			msg.AppendLine(ex.Message);
			msg.AppendLine();
			while(ex.InnerException != null) {
				ex = ex.InnerException;
				msg.AppendLine("Inner Exception:");
				msg.AppendFormat("Type: [{0}]", ex.Type);
				msg.AppendLine();
				msg.AppendLine("Message:");
				msg.Append("\t");
				if (ex.Message != "null") { msg.AppendLine(ex.Message); }
				msg.AppendLine();
			}
			msg.AppendLine("StackTrace:");
			msg.AppendLine(exception.Callstack.Replace("\n","\r\n"));
			
			using (ExceptionForm form = new ExceptionForm()) {
				form.textBox.Text = msg.ToString();
				form.pictureBox.Image = ResourceService.GetBitmap((exception.ExceptionType != ExceptionType.DEBUG_EXCEPTION_UNHANDLED)?"Icons.32x32.Warning":"Icons.32x32.Error");
				form.buttonContinue.Enabled = exception.ExceptionType != ExceptionType.DEBUG_EXCEPTION_UNHANDLED;
				form.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
				return form.result;
			}
		}
		
		#region Form Event Handlers
		private void buttonBreak_Click(object sender, System.EventArgs e)
		{
			result = Result.Break;
			Close();
		}

		private void buttonContinue_Click(object sender, System.EventArgs e)
		{
			result = Result.Continue;
			Close();
		}

		private void buttonTerminate_Click(object sender, System.EventArgs e)
		{
			result = Result.Terminate;
			Close();
		}
		#endregion Form Event Handlers
	}
}
