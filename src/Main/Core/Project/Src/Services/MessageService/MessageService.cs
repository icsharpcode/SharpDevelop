// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
using ICSharpCode.Core.Services;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Class with static methods to show message boxes.
	/// All text displayed using the MessageService is passed to the
	/// <see cref="StringParser"/> to replace ${res} markers.
	/// </summary>
	public static class MessageService
	{
		/// <summary>
		/// Shows an exception error.
		/// </summary>
		public static void ShowException(Exception ex)
		{
			ShowException(ex, null);
		}
		
		/// <summary>
		/// Shows an error using a message box.
		/// </summary>
		public static void ShowError(string message)
		{
			LoggingService.Error(message);
			ServiceManager.Instance.MessageService.ShowError(message);
		}
		
		/// <summary>
		/// Shows an error using a message box.
		/// <paramref name="formatstring"/> is first passed through the
		/// <see cref="StringParser"/>,
		/// then through <see cref="string.Format(string, object)"/>, using the formatitems as arguments.
		/// </summary>
		public static void ShowErrorFormatted(string formatstring, params object[] formatitems)
		{
			ShowError(Format(formatstring, formatitems));
		}
		
		/// <summary>
		/// Shows an exception.
		/// </summary>
		public static void ShowException(Exception ex, string message)
		{
			LoggingService.Error(message, ex);
			LoggingService.Warn("Stack trace of last exception log:\n" + Environment.StackTrace);
			ServiceManager.Instance.MessageService.ShowException(ex, message);
		}
		
		/// <summary>
		/// Shows an exception.
		/// </summary>
		public static void ShowHandledException(Exception ex)
		{
			ShowHandledException(ex, null);
		}

		/// <summary>
		/// Shows an exception.
		/// </summary>
		public static void ShowHandledException(Exception ex, string message)
		{
			LoggingService.Error(message, ex);
			LoggingService.Warn("Stack trace of last exception log:\n" + Environment.StackTrace);
			message = GetMessage(message, ex);
			ServiceManager.Instance.MessageService.ShowError(message);
		}
		
		static string GetMessage(string message, Exception ex)
		{
			if (message == null) {
				return ex.Message;
			}
			return message + "\r\n\r\n" + ex.Message;
		}
		
		/// <summary>
		/// Shows a warning message.
		/// </summary>
		public static void ShowWarning(string message)
		{
			LoggingService.Warn(message);
			ServiceManager.Instance.MessageService.ShowWarning(message);
		}
		
		/// <summary>
		/// Shows a warning message.
		/// <paramref name="formatstring"/> is first passed through the
		/// <see cref="StringParser"/>,
		/// then through <see cref="string.Format(string, object)"/>, using the formatitems as arguments.
		/// </summary>
		public static void ShowWarningFormatted(string formatstring, params object[] formatitems)
		{
			ShowWarning(Format(formatstring, formatitems));
		}
		
		/// <summary>
		/// Asks the user a Yes/No question, using "Yes" as the default button.
		/// Returns <c>true</c> if yes was clicked, <c>false</c> if no was clicked.
		/// </summary>
		public static bool AskQuestion(string question, string caption)
		{
			return ServiceManager.Instance.MessageService.AskQuestion(question, caption);
		}
		
		public static bool AskQuestionFormatted(string caption, string formatstring, params object[] formatitems)
		{
			return AskQuestion(Format(formatstring, formatitems), caption);
		}
		
		public static bool AskQuestionFormatted(string formatstring, params object[] formatitems)
		{
			return AskQuestion(Format(formatstring, formatitems));
		}
		
		/// <summary>
		/// Asks the user a Yes/No question, using "Yes" as the default button.
		/// Returns <c>true</c> if yes was clicked, <c>false</c> if no was clicked.
		/// </summary>
		public static bool AskQuestion(string question)
		{
			return AskQuestion(question, StringParser.Parse("${res:Global.QuestionText}"));
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
		/// <returns>The number of the button that was clicked, or -1 if the dialog was closed  without clicking a button.</returns>
		public static int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
		{
			return ServiceManager.Instance.MessageService.ShowCustomDialog(caption, dialogText, acceptButtonIndex, cancelButtonIndex, buttontexts);
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
			return ServiceManager.Instance.MessageService.ShowInputBox(caption, dialogText, defaultValue);
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
		
		public static void ShowMessageFormatted(string formatstring, params object[] formatitems)
		{
			ShowMessage(Format(formatstring, formatitems));
		}
		
		public static void ShowMessageFormatted(string caption, string formatstring, params object[] formatitems)
		{
			ShowMessage(Format(formatstring, formatitems), caption);
		}
		
		public static void ShowMessage(string message, string caption)
		{
			LoggingService.Info(message);
			ServiceManager.Instance.MessageService.ShowMessage(message, caption);
		}
		
		static string Format(string formatstring, object[] formatitems)
		{
			try {
				return String.Format(StringParser.Parse(formatstring), formatitems);
			} catch (FormatException ex) {
				LoggingService.Warn(ex);
				
				StringBuilder b = new StringBuilder(StringParser.Parse(formatstring));
				foreach(object formatitem in formatitems) {
					b.Append("\nItem: ");
					b.Append(formatitem);
				}
				return b.ToString();
			}
		}
	}
}
