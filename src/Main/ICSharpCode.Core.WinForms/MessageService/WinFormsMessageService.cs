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
using System.ComponentModel;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	/// <summary>
	/// Class with static methods to show message boxes.
	/// All text displayed using the MessageService is passed to the
	/// <see cref="StringParser"/> to replace ${res} markers.
	/// </summary>
	public class WinFormsMessageService : IDialogMessageService
	{
		/// <summary>
		/// Gets/Sets the form used as owner for shown message boxes.
		/// </summary>
		public IWin32Window DialogOwner { get; set; }
		
		/// <summary>
		/// Gets/Sets the object used to synchronize message boxes shown on other threads.
		/// </summary>
		public ISynchronizeInvoke DialogSynchronizeInvoke { get; set; }
		
		public string DefaultMessageBoxTitle { get; set; }
		
		public string ProductName { get; set; }
		
		public WinFormsMessageService()
		{
			this.DefaultMessageBoxTitle = this.ProductName = "SharpDevelop";
		}
		
		void BeginInvoke(MethodInvoker method)
		{
			ISynchronizeInvoke si = DialogSynchronizeInvoke;
			if (si == null || !si.InvokeRequired)
				method();
			else
				si.BeginInvoke(method, null);
		}
		
		void Invoke(MethodInvoker method)
		{
			ISynchronizeInvoke si = DialogSynchronizeInvoke;
			if (si == null || !si.InvokeRequired)
				method();
			else
				si.Invoke(method, null);
		}
		
		public virtual void ShowException(Exception ex, string message)
		{
			LoggingService.Error(message, ex);
			LoggingService.Warn("Stack trace of last exception log:\n" + Environment.StackTrace);
			message = StringParser.Parse(message);
			if (ex != null) {
				message += "\n\nException occurred: " + ex.ToString();
			}
			DoShowMessage(message, StringParser.Parse("${res:Global.ErrorText}"), MessageBoxIcon.Error);
		}
		
		void DoShowMessage(string message, string caption, MessageBoxIcon icon)
		{
			BeginInvoke(
				delegate {
					MessageBox.Show(DialogOwner,
					                message, caption ?? DefaultMessageBoxTitle,
					                MessageBoxButtons.OK, MessageBoxIcon.Warning,
					                MessageBoxDefaultButton.Button1, GetOptions(message, caption));
				});
		}
		
		public void ShowError(string message)
		{
			LoggingService.Error(message);
			DoShowMessage(StringParser.Parse(message), StringParser.Parse("${res:Global.ErrorText}"), MessageBoxIcon.Error);
		}
		
		public void ShowWarning(string message)
		{
			LoggingService.Warn(message);
			DoShowMessage(StringParser.Parse(message), StringParser.Parse("${res:Global.WarningText}"), MessageBoxIcon.Warning);
		}
		
		public void ShowMessage(string message, string caption)
		{
			LoggingService.Info(message);
			DoShowMessage(StringParser.Parse(message), StringParser.Parse(caption), MessageBoxIcon.Information);
		}
		
		public void ShowErrorFormatted(string formatstring, params object[] formatitems)
		{
			LoggingService.Error(formatstring);
			DoShowMessage(StringParser.Format(formatstring, formatitems), StringParser.Parse("${res:Global.ErrorText}"), MessageBoxIcon.Error);
		}
		
		public void ShowWarningFormatted(string formatstring, params object[] formatitems)
		{
			LoggingService.Warn(formatstring);
			DoShowMessage(StringParser.Format(formatstring, formatitems), StringParser.Parse("${res:Global.WarningText}"), MessageBoxIcon.Warning);
		}
		
		public void ShowMessageFormatted(string formatstring, string caption, params object[] formatitems)
		{
			LoggingService.Info(formatstring);
			DoShowMessage(StringParser.Format(formatstring, formatitems), StringParser.Parse(caption), MessageBoxIcon.Information);
		}
		
		public bool AskQuestion(string question, string caption)
		{
			DialogResult result = DialogResult.None;
			Invoke(
				delegate {
					result = MessageBox.Show(DialogOwner,
					                         StringParser.Parse(question),
					                         StringParser.Parse(caption ?? "${res:Global.QuestionText}"),
					                         MessageBoxButtons.YesNo,
					                         MessageBoxIcon.Question,
					                         MessageBoxDefaultButton.Button1,
					                         GetOptions(question, caption));
				});
			return result == DialogResult.Yes;
		}
		
		static MessageBoxOptions GetOptions(string text, string caption)
		{
			return IsRtlText(text) ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign : 0;
		}
		
		static bool IsRtlText(string text)
		{
			if (!RightToLeftConverter.IsRightToLeft)
				return false;
			foreach (char c in StringParser.Parse(text)) {
				if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherLetter)
					return true;
			}
			return false;
		}
		
		public int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
		{
			int result = 0;
			Invoke(
				delegate {
					using (CustomDialog messageBox = new CustomDialog(caption, dialogText, acceptButtonIndex, cancelButtonIndex, buttontexts)) {
						messageBox.ShowDialog(DialogOwner);
						result = messageBox.Result;
					}
				});
			return result;
		}
		
		public string ShowInputBox(string caption, string dialogText, string defaultValue)
		{
			string result = null;
			Invoke(
				delegate {
					using (InputBox inputBox = new InputBox(dialogText, caption, defaultValue)) {
						inputBox.ShowDialog(DialogOwner);
						result = inputBox.Result;
					}
				});
			return result;
		}
		
		public void InformSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot)
		{
			BeginInvoke(
				delegate {
					using (SaveErrorInformDialog dlg = new SaveErrorInformDialog(fileName, message, dialogName, exceptionGot)) {
						dlg.ShowDialog(DialogOwner);
					}
				});
		}
		
		public ChooseSaveErrorResult ChooseSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled)
		{
			ChooseSaveErrorResult r = ChooseSaveErrorResult.Ignore;
			Invoke(
				delegate {
				restartlabel:
					using (SaveErrorChooseDialog dlg = new SaveErrorChooseDialog(fileName, message, dialogName, exceptionGot, chooseLocationEnabled)) {
						switch (dlg.ShowDialog(DialogOwner)) {
							case DialogResult.OK:
								// choose location:
								using (SaveFileDialog fdiag = new SaveFileDialog()) {
									fdiag.OverwritePrompt = true;
									fdiag.AddExtension    = true;
									fdiag.CheckFileExists = false;
									fdiag.CheckPathExists = true;
									fdiag.Title           = "Choose alternate file name";
									fdiag.FileName        = fileName;
									if (fdiag.ShowDialog() == DialogResult.OK) {
										r = ChooseSaveErrorResult.SaveAlternative(FileName.Create(fdiag.FileName));
										break;
									} else {
										goto restartlabel;
									}
								}
							case DialogResult.Retry:
								r = ChooseSaveErrorResult.Retry;
								break;
							default:
								r = ChooseSaveErrorResult.Ignore;
								break;
						}
					}
				});
			return r;
		}
		
		public void ShowHandledException(Exception ex, string message = null)
		{
			LoggingService.Error(message, ex);
			LoggingService.Warn("Stack trace of last exception log:\n" + Environment.StackTrace);
			if (message == null) {
				message = ex.Message;
			} else {
				message = StringParser.Parse(message) + "\r\n\r\n" + ex.Message;
			}
			DoShowMessage(message, StringParser.Parse("${res:Global.ErrorText}"), MessageBoxIcon.Error);
		}
	}
}
