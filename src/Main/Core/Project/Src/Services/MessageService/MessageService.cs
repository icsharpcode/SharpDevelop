// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Class with static methods to show message boxes.
	/// All text displayed using the MessageService is passed to the
	/// <see cref="StringParser"/> to replace ${res} markers.
	/// </summary>
	public static class MessageService
	{
		static Form mainForm;
		
		/// <summary>
		/// Gets/Sets the form used as owner for shown message boxes.
		/// It is also used to synchronize message boxes shown on other threads.
		/// </summary>
		public static Form MainForm {
			get { return mainForm; }
			set { mainForm = value; }
		}
		
		/// <summary>
		/// Delegate used for custom error callbacks.
		/// </summary>
		public delegate void ShowErrorDelegate(Exception ex, string message);
		
		static ShowErrorDelegate customErrorReporter;
		
		/// <summary>
		/// Gets/Sets the custom error reporter callback delegate.
		/// If this property is null, the default messagebox is used.
		/// </summary>
		public static ShowErrorDelegate CustomErrorReporter {
			get { return customErrorReporter; }
			set { customErrorReporter = value; }
		}
		
		/// <summary>
		/// Shows an exception error using the <see cref="CustomErrorReporter"/>.
		/// </summary>
		public static void ShowError(Exception ex)
		{
			ShowError(ex, null);
		}
		
		/// <summary>
		/// Shows an error using a message box.
		/// </summary>
		public static void ShowError(string message)
		{
			ShowError(null, message);
		}
		
		/// <summary>
		/// Shows an error using a message box.
		/// <paramref name="formatstring"/> is first passed through the
		/// <see cref="StringParser"/>,
		/// then through <see cref="string.Format(string, object)"/>, using the formatitems as arguments.
		/// </summary>
		public static void ShowErrorFormatted(string formatstring, params string[] formatitems)
		{
			ShowError(null, Format(formatstring, formatitems));
		}
		
		/// <summary>
		/// Shows an error.
		/// If <paramref name="ex"/> is null, the message is shown inside
		/// a message box.
		/// Otherwise, the custom error reporter is used to display
		/// the exception error.
		/// </summary>
		public static void ShowError(Exception ex, string message)
		{
			if (message == null) message = string.Empty;
			
			if (ex != null) {
				LoggingService.Error(message, ex);
				LoggingService.Warn("Stack trace of last error log:\n" + Environment.StackTrace);
				if (customErrorReporter != null) {
					customErrorReporter(ex, message);
					return;
				}
			} else {
				LoggingService.Error(message);
			}
			
			string msg = message + "\n\n";
			
			if (ex != null) {
				msg += "Exception occurred: " + ex.ToString();
			}
			
			if (MessageService.MainForm == null) {
				MessageBox.Show(StringParser.Parse(msg), StringParser.Parse("SharpDevelop: ${res:Global.ErrorText}"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			} else {
				
				MethodInvoker showError = delegate {
					MessageBox.Show(MessageService.MainForm, StringParser.Parse(msg), StringParser.Parse("${res:Global.ErrorText}"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				};
				if (MessageService.MainForm.InvokeRequired) {
					MessageService.MainForm.BeginInvoke(showError);
				} else {
					showError();
				}
			}
		}
		
		/// <summary>
		/// Shows a warning message.
		/// </summary>
		public static void ShowWarning(string message)
		{
			message = StringParser.Parse(message);
			LoggingService.Warn(message);
			
			string caption = StringParser.Parse("${res:Global.WarningText}");
			if (MessageService.MainForm == null) {
				MessageBox.Show(message, caption,
				                MessageBoxButtons.OK, MessageBoxIcon.Warning,
				                MessageBoxDefaultButton.Button1, GetOptions(message, caption));
			} else {
				MethodInvoker showWarning = delegate {
					MessageBox.Show(MessageService.MainForm,
					                message, caption,
					                MessageBoxButtons.OK, MessageBoxIcon.Warning,
					                MessageBoxDefaultButton.Button1, GetOptions(message, caption));
				};
				if (MessageService.MainForm.InvokeRequired) {
					MessageService.MainForm.BeginInvoke(showWarning);
				} else {
					showWarning();
				}
			}
		}
		
		/// <summary>
		/// Shows a warning message.
		/// <paramref name="formatstring"/> is first passed through the
		/// <see cref="StringParser"/>,
		/// then through <see cref="string.Format(string, object)"/>, using the formatitems as arguments.
		/// </summary>
		public static void ShowWarningFormatted(string formatstring, params string[] formatitems)
		{
			ShowWarning(Format(formatstring, formatitems));
		}
		
		/// <summary>
		/// Asks the user a Yes/No question, using "Yes" as the default button.
		/// Returns <c>true</c> if yes was clicked, <c>false</c> if no was clicked.
		/// </summary>
		public static bool AskQuestion(string question, string caption)
		{
			return MessageBox.Show(MessageService.MainForm,
			                       StringParser.Parse(question),
			                       StringParser.Parse(caption),
			                       MessageBoxButtons.YesNo,
			                       MessageBoxIcon.Question,
			                       MessageBoxDefaultButton.Button1,
			                       GetOptions(question, caption))
				== DialogResult.Yes;
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
		
		public static bool AskQuestionFormatted(string caption, string formatstring, params string[] formatitems)
		{
			return AskQuestion(Format(formatstring, formatitems), caption);
		}
		
		public static bool AskQuestionFormatted(string formatstring, params string[] formatitems)
		{
			return AskQuestion(Format(formatstring, formatitems));
		}
		
		/// <summary>
		/// Asks the user a Yes/No question, using "Yes" as the default button.
		/// Returns <c>true</c> if yes was clicked, <c>false</c> if no was clicked.
		/// </summary>
		public static bool AskQuestion(string question)
		{
			return AskQuestion(StringParser.Parse(question), StringParser.Parse("${res:Global.QuestionText}"));
		}
		
		/// <summary>
		/// Shows a custom dialog.
		/// </summary>
		/// <param name="caption">The title of the dialog.</param>
		/// <param name="dialogText">The description shown in the dialog.</param>
		/// <param name="acceptButtonIndex">
		/// The number of the button that is the default accept button.
		/// Use -1 if you don't want to have an accept button.
		/// </param>
		/// <param name="cancelButtonIndex">
		/// The number of the button that is the cancel button.
		/// Use -1 if you don't want to have a cancel button.
		/// </param>
		/// <param name="buttontexts">The captions of the buttons.</param>
		/// <returns>The number of the button that was clicked.</returns>
		public static int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
		{
			using (CustomDialog messageBox = new CustomDialog(caption, dialogText, acceptButtonIndex, cancelButtonIndex, buttontexts)) {
				messageBox.ShowDialog(MessageService.MainForm);
				return messageBox.Result;
			}
		}
		
		/// <summary>
		/// Shows a custom dialog.
		/// </summary>
		/// <param name="caption">The title of the dialog.</param>
		/// <param name="dialogText">The description shown in the dialog.</param>
		/// <param name="buttontexts">The captions of the buttons.</param>
		/// <returns>The number of the button that was clicked.</returns>
		public static int ShowCustomDialog(string caption, string dialogText, params string[] buttontexts)
		{
			return ShowCustomDialog(caption, dialogText, -1, -1, buttontexts);
		}
		
		public static string ShowInputBox(string caption, string dialogText, string defaultValue)
		{
			using (InputBox inputBox = new InputBox(dialogText, caption, defaultValue)) {
				inputBox.ShowDialog(MessageService.MainForm);
				return inputBox.Result;
			}
		}
		
		static string defaultMessageBoxTitle = "MessageBox";
		static string productName = "Application Name";
		
		/// <summary>
		/// Gets/Sets the name of the product using ICSharpCode.Core.
		/// Is used by the string parser as replacement for ${ProductName}.
		/// </summary>
		public static string ProductName {
			get { return productName; }
			set { productName = value; }
		}
		
		/// <summary>
		/// Gets/Sets the default title for message boxes displayed
		/// by the message service.
		/// </summary>
		public static string DefaultMessageBoxTitle {
			get { return defaultMessageBoxTitle; }
			set { defaultMessageBoxTitle = value; }
		}
		
		public static void ShowMessage(string message)
		{
			ShowMessage(message, DefaultMessageBoxTitle);
		}
		
		public static void ShowMessageFormatted(string formatstring, params string[] formatitems)
		{
			ShowMessage(Format(formatstring, formatitems));
		}
		
		public static void ShowMessageFormatted(string caption, string formatstring, params string[] formatitems)
		{
			ShowMessage(Format(formatstring, formatitems), caption);
		}
		
		public static void ShowMessage(string message, string caption)
		{
			message = StringParser.Parse(message);
			LoggingService.Info(message);
			MessageBox.Show(mainForm,
			                message,
			                StringParser.Parse(caption),
			                MessageBoxButtons.OK,
			                MessageBoxIcon.Information,
			                MessageBoxDefaultButton.Button1,
			                GetOptions(message, caption));
		}
		
		static string Format(string formatstring, string[] formatitems)
		{
			try {
				return String.Format(StringParser.Parse(formatstring), formatitems);
			} catch (FormatException) {
				StringBuilder b = new StringBuilder(StringParser.Parse(formatstring));
				foreach(string formatitem in formatitems) {
					b.Append("\nItem: ");
					b.Append(formatitem);
				}
				return b.ToString();
			}
		}
	}
}
