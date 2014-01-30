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
using System.Text;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Class with static methods to show message boxes.
	/// All text displayed using the MessageService is passed to the
	/// <see cref="StringParser"/> to replace ${res} markers.
	/// </summary>
	public static class MessageService
	{
		static IMessageService Service {
			get { return ServiceSingleton.GetRequiredService<IMessageService>(); }
		}
		
		/// <summary>
		/// Shows an error using a message box.
		/// </summary>
		public static void ShowError(string message)
		{
			Service.ShowError(message);
		}
		
		/// <summary>
		/// Shows an error using a message box.
		/// <paramref name="formatstring"/> is first passed through the
		/// <see cref="StringParser"/>,
		/// then through <see cref="string.Format(string, object)"/>, using the formatitems as arguments.
		/// </summary>
		public static void ShowErrorFormatted(string formatstring, params object[] formatitems)
		{
			Service.ShowErrorFormatted(formatstring, formatitems);
		}
		
		/// <summary>
		/// Shows an exception.
		/// </summary>
		public static void ShowException(Exception ex, string message = null)
		{
			Service.ShowException(ex, message);
		}
		
		/// <summary>
		/// Shows a warning message.
		/// </summary>
		public static void ShowWarning(string message)
		{
			Service.ShowWarning(message);
		}
		
		/// <summary>
		/// Shows a warning message.
		/// <paramref name="formatstring"/> is first passed through the
		/// <see cref="StringParser"/>,
		/// then through <see cref="string.Format(string, object)"/>, using the formatitems as arguments.
		/// </summary>
		public static void ShowWarningFormatted(string formatstring, params object[] formatitems)
		{
			Service.ShowWarningFormatted(formatstring, formatitems);
		}
		
		/// <summary>
		/// Asks the user a Yes/No question, using "Yes" as the default button.
		/// Returns <c>true</c> if yes was clicked, <c>false</c> if no was clicked.
		/// </summary>
		public static bool AskQuestion(string question, string caption = null)
		{
			return Service.AskQuestion(question, caption);
		}
		
		public static bool AskQuestionFormatted(string caption, string formatstring, params object[] formatitems)
		{
			return Service.AskQuestion(StringParser.Format(formatstring, formatitems), caption);
		}
		
		public static bool AskQuestionFormatted(string formatstring, params object[] formatitems)
		{
			return Service.AskQuestion(StringParser.Format(formatstring, formatitems));
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
			return Service.ShowCustomDialog(caption, dialogText, acceptButtonIndex, cancelButtonIndex, buttontexts);
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
			return Service.ShowInputBox(caption, dialogText, defaultValue);
		}
		
		/// <summary>
		/// Gets/Sets the name of the product using ICSharpCode.Core.
		/// Is used by the string parser as replacement for ${ProductName}.
		/// </summary>
		public static string ProductName {
			get { return Service.ProductName; }
		}
		
		/// <summary>
		/// Gets/Sets the default title for message boxes displayed
		/// by the message service.
		/// </summary>
		public static string DefaultMessageBoxTitle {
			get { return Service.DefaultMessageBoxTitle; }
		}
		
		public static void ShowMessageFormatted(string formatstring, params object[] formatitems)
		{
			Service.ShowMessageFormatted(formatstring, null, formatitems);
		}
		
		public static void ShowMessageFormatted(string caption, string formatstring, params object[] formatitems)
		{
			Service.ShowMessageFormatted(formatstring, caption, formatitems);
		}
		
		public static void ShowMessage(string message, string caption = null)
		{
			Service.ShowMessage(message, caption);
		}
		
		public static void ShowHandledException(Exception ex, string message = null)
		{
			Service.ShowHandledException(ex, message);
		}
	}
}
