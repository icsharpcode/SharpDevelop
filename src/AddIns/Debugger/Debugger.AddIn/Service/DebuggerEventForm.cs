// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
				form.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window);
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
