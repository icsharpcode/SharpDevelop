// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Resources;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace ICSharpCode.Core
{
	/// <summary>
	/// This interface must be implemented by all services.
	/// </summary>
	public static class MessageService
	{
		static Form mainForm;
		
		public static Form MainForm {
			get {
				return mainForm;
			}
			set {
				mainForm = value;
			}
		}
		
		public static void ShowError(Exception ex)
		{
			ShowError(ex, null);
		}
		
		public static void ShowError(string message)
		{
			ShowError(null, message);
		}
		
		public static void ShowErrorFormatted(string formatstring, params string[] formatitems)
		{
			ShowError(null, String.Format(StringParser.Parse(formatstring), formatitems));
		}
		
		
		public delegate void ShowErrorDelegate(Exception ex, string message);
		
		static ShowErrorDelegate customErrorReporter;
		
		/// <summary>
		/// Gets/Sets the custom error reporter. If this property is null, a default
		/// messagebox is used.
		/// </summary>
		public static ShowErrorDelegate CustomErrorReporter {
			get {
				return customErrorReporter;
			}
			set {
				customErrorReporter = value;
			}
		}
		
		public static void ShowError(Exception ex, string message)
		{
			if (customErrorReporter != null && ex != null) {
				customErrorReporter(ex, message);
				return;
			}
			
			#if DEBUG
			Console.WriteLine();
			if (message != null)
				Console.WriteLine(message);
			if (ex != null) {
				Console.WriteLine(ex);
				Console.Beep();
				return;
			}
			#endif
			string msg = String.Empty;
			
			if (message != null) {
				msg += message;
			}
			
			if (message != null && ex != null) {
				msg += "\n\n";
			}
			
			if (ex != null) {
				msg += "Exception occurred: " + ex.ToString();
			}
			
			MessageBox.Show(MessageService.MainForm, StringParser.Parse(msg), StringParser.Parse("${res:Global.ErrorText}"), MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		
		public static void ShowWarning(string message)
		{
			MessageBox.Show(MessageService.MainForm, StringParser.Parse(message), StringParser.Parse("${res:Global.WarningText}"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		
		public static void ShowWarningFormatted(string formatstring, params string[] formatitems)
		{
			ShowWarning(String.Format(StringParser.Parse(formatstring), formatitems));
		}
		
		public static bool AskQuestion(string question, string caption)
		{
			return MessageBox.Show(MessageService.MainForm, StringParser.Parse(question), StringParser.Parse(caption), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
		}
		
		public static bool AskQuestionFormatted(string caption, string formatstring, params string[] formatitems)
		{
			return AskQuestion(String.Format(StringParser.Parse(formatstring), formatitems), caption);
		}
		
		public static bool AskQuestionFormatted(string formatstring, params string[] formatitems)
		{
			return AskQuestion(String.Format(StringParser.Parse(formatstring), formatitems));
		}
		
		public static bool AskQuestion(string question)
		{
			return AskQuestion(StringParser.Parse(question), StringParser.Parse("${res:Global.QuestionText}"));
		}
		
		public static int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
		{
			using (CustomDialog messageBox = new CustomDialog(caption, dialogText, acceptButtonIndex, cancelButtonIndex, buttontexts)) {
				messageBox.ShowDialog(MessageService.MainForm);
				return messageBox.Result;
			}
		}
		
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
		
		public static void ShowMessage(string message)
		{
			ShowMessage(message, "SharpDevelop");
		}
		
		public static void ShowMessageFormatted(string formatstring, params string[] formatitems)
		{
			ShowMessage(String.Format(StringParser.Parse(formatstring), formatitems));
		}
		
		public static void ShowMessageFormatted(string caption, string formatstring, params string[] formatitems)
		{
			ShowMessage(String.Format(StringParser.Parse(formatstring), formatitems), caption);
		}
		
		public static void ShowMessage(string message, string caption)
		{
			MessageBox.Show(mainForm, StringParser.Parse(message), StringParser.Parse(caption), MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
