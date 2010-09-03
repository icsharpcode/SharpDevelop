// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.Core.Services
{
	/// <summary>
	/// IMessageService implementation that writes messages to a text writer.
	/// User input is not implemented by this service.
	/// </summary>
	public class TextWriterMessageService : IMessageService
	{
		readonly TextWriter writer;
		
		public TextWriterMessageService(TextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			this.writer = writer;
		}
		
		public void ShowError(string message)
		{
			writer.WriteLine(message);
		}
		
		public void ShowException(Exception ex, string message)
		{
			if (message != null) {
				writer.WriteLine(message);
			}
			if (ex != null) {
				writer.WriteLine(ex.ToString());
			}
		}
		
		public void ShowWarning(string message)
		{
			writer.WriteLine(message);
		}
		
		public bool AskQuestion(string question, string caption)
		{
			writer.WriteLine(caption + ": " + question);
			return false;
		}
		
		public int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
		{
			writer.WriteLine(caption + ": " + dialogText);
			return cancelButtonIndex;
		}
		
		public string ShowInputBox(string caption, string dialogText, string defaultValue)
		{
			writer.WriteLine(caption + ": " + dialogText);
			return defaultValue;
		}
		
		public void ShowMessage(string message, string caption)
		{
			writer.WriteLine(caption + ": " + message);
		}
		
		public void InformSaveError(string fileName, string message, string dialogName, Exception exceptionGot)
		{
			writer.WriteLine(dialogName + ": " + message + " (" + fileName + ")");
			if (exceptionGot != null)
				writer.WriteLine(exceptionGot.ToString());
		}
		
		public ChooseSaveErrorResult ChooseSaveError(string fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled)
		{
			writer.WriteLine(dialogName + ": " + message + " (" + fileName + ")");
			if (exceptionGot != null)
				writer.WriteLine(exceptionGot.ToString());
			return ChooseSaveErrorResult.Ignore;
		}
	}
}
