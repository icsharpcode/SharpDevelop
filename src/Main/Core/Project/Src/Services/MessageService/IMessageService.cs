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
using ICSharpCode.Core.Implementation;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Interface for the MessageService.
	/// </summary>
	[SDService("SD.MessageService", FallbackImplementation = typeof(FallbackMessageService))]
	public interface IMessageService
	{
		/// <summary>
		/// Shows an exception.
		/// </summary>
		void ShowException(Exception ex, string message = null);
		
		/// <summary>
		/// Shows an exception.
		/// </summary>
		void ShowHandledException(Exception ex, string message = null);
		
		/// <summary>
		/// Shows an error.
		/// </summary>
		void ShowError(string message);
		
		/// <summary>
		/// Shows an error using a message box.
		/// <paramref name="formatstring"/> is first passed through the
		/// <see cref="StringParser"/>,
		/// then through <see cref="string.Format(string, object)"/>, using the formatitems as arguments.
		/// </summary>
		void ShowErrorFormatted(string formatstring, params object[] formatitems);
		
		/// <summary>
		/// Shows a warning message.
		/// </summary>
		void ShowWarning(string message);
		
		/// <summary>
		/// Shows a warning message.
		/// <paramref name="formatstring"/> is first passed through the
		/// <see cref="StringParser"/>,
		/// then through <see cref="string.Format(string, object)"/>, using the formatitems as arguments.
		/// </summary>
		void ShowWarningFormatted(string formatstring, params object[] formatitems);
		
		void ShowMessage(string message, string caption = null);
		void ShowMessageFormatted(string formatstring, string caption, params object[] formatitems);
		
		/// <summary>
		/// Asks the user a Yes/No question, using "Yes" as the default button.
		/// Returns <c>true</c> if yes was clicked, <c>false</c> if no was clicked.
		/// </summary>
		bool AskQuestion(string question, string caption = null);
		
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
		int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts);
		
		/// <summary>
		/// Shows an input box.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="dialogText"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		string ShowInputBox(string caption, string dialogText, string defaultValue);
		
		/// <summary>
		/// Gets the default message box title.
		/// </summary>
		string DefaultMessageBoxTitle { get; }
		
		/// <summary>
		/// Gets the application product name.
		/// </summary>
		string ProductName { get; }
		
		/// <summary>
		/// Show a message informing the user about a save error.
		/// </summary>
		void InformSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot);
		
		/// <summary>
		/// Show a message informing the user about a save error,
		/// and allow him to retry/save under alternative name.
		/// </summary>
		ChooseSaveErrorResult ChooseSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled);
	}
	
	sealed class FallbackMessageService : TextWriterMessageService
	{
		public FallbackMessageService() : base(Console.Out) {}
	}
	
	public sealed class ChooseSaveErrorResult
	{
		public bool IsRetry { get; private set; }
		public bool IsIgnore { get; private set; }
		public bool IsSaveAlternative { get { return AlternativeFileName != null; } }
		public FileName AlternativeFileName { get; private set; }
		
		private ChooseSaveErrorResult() {}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification="ChooseSaveErrorResult is immutable")]
		public readonly static ChooseSaveErrorResult Retry = new ChooseSaveErrorResult { IsRetry = true };
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification="ChooseSaveErrorResult is immutable")]
		public readonly static ChooseSaveErrorResult Ignore = new ChooseSaveErrorResult { IsIgnore = true };
		
		public static ChooseSaveErrorResult SaveAlternative(FileName alternativeFileName)
		{
			if (alternativeFileName == null)
				throw new ArgumentNullException("alternativeFileName");
			return new ChooseSaveErrorResult { AlternativeFileName = alternativeFileName };
		}
	}
}
