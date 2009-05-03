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
using System.Drawing;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Services
{
	internal partial class DebuggerEventForm
	{
		public enum Result {Break, Continue, Terminate};
		
		protected Result result = Result.Break; // Default
		
		protected DebuggerEventForm()
		{
			InitializeComponent();
			this.Text = StringParser.Parse(this.Text);
			buttonBreak.Text = StringParser.Parse(buttonBreak.Text);
			buttonContinue.Text = StringParser.Parse(buttonContinue.Text);
			buttonTerminate.Text = StringParser.Parse(buttonTerminate.Text);
						
            WindowState = DebuggingOptions.Instance.DebuggerEventWindowState;
            FormLocationHelper.Apply(this, "DebuggerEventForm", true);
		}
		
		/// <summary>
		/// Displays a DebuggerEvent form with the given message.
		/// </summary>
		/// <param name="title">Title of the dialog box.</param>
		/// <param name="message">The message to display in the TextArea of the dialog box.</param>
		/// <param name="icon">Icon to display i nthe dialog box.</param>
		/// <param name="canContinue">Set to true to enable the continue button on the form.</param>
		/// <returns></returns>
		public static Result Show(string title, string message, Bitmap icon, bool canContinue)
		{
			using (DebuggerEventForm form = new DebuggerEventForm()) {
				form.Text = title;
				form.textBox.Text = message;
				form.pictureBox.Image = icon;
				form.buttonContinue.Enabled = canContinue;
				form.ShowDialog(WorkbenchSingleton.MainForm);
				return form.result;
			}
		}
		
		private void buttonBreak_Click(object sender, EventArgs e)
		{
			result = Result.Break;
			Close();
		}

		private void buttonContinue_Click(object sender, EventArgs e)
		{
			result = Result.Continue;
			Close();
		}

		private void buttonTerminate_Click(object sender, EventArgs e)
		{
			result = Result.Terminate;
			Close();
		}
		
		void debuggerEventFormResize(object sender, EventArgs e)
		{
			DebuggingOptions.Instance.DebuggerEventWindowState = WindowState;
		}
	}
}
