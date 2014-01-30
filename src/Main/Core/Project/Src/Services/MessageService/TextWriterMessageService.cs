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
using System.IO;

namespace ICSharpCode.Core.Implementation
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
			this.DefaultMessageBoxTitle = this.ProductName = "SharpDevelop";
		}
		
		public void ShowError(string message)
		{
			writer.WriteLine(message);
		}
		
		public void ShowException(Exception ex, string message = null)
		{
			if (message != null) {
				writer.WriteLine(message);
			}
			if (ex != null) {
				writer.WriteLine(ex.ToString());
			}
		}
		
		public void ShowHandledException(Exception ex, string message = null)
		{
			if (message != null) {
				writer.WriteLine(message);
			}
			if (ex != null) {
				writer.WriteLine(ex.Message);
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
		
		public void InformSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot)
		{
			writer.WriteLine(dialogName + ": " + message + " (" + fileName + ")");
			if (exceptionGot != null)
				writer.WriteLine(exceptionGot.ToString());
		}
		
		public ChooseSaveErrorResult ChooseSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled)
		{
			writer.WriteLine(dialogName + ": " + message + " (" + fileName + ")");
			if (exceptionGot != null)
				writer.WriteLine(exceptionGot.ToString());
			return ChooseSaveErrorResult.Ignore;
		}
		
		public void ShowErrorFormatted(string formatstring, params object[] formatitems)
		{
			writer.WriteLine(StringParser.Format(formatstring, formatitems));
		}
		
		public void ShowWarningFormatted(string formatstring, params object[] formatitems)
		{
			writer.WriteLine(StringParser.Format(formatstring, formatitems));
		}
		
		public void ShowMessageFormatted(string formatstring, string caption, params object[] formatitems)
		{
			writer.WriteLine(StringParser.Format(formatstring, formatitems));
		}
		
		public string DefaultMessageBoxTitle { get; set; }
		public string ProductName { get; set; }
	}
}
