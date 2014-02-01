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
using ICSharpCode.Core;

namespace PackageManagement.Tests.Helpers
{
	public class FakeMessageService : IMessageService
	{
		public string ErrorMessageDisplayed;
		
		public void ShowError(string message)
		{
			ErrorMessageDisplayed = message;
		}
		
		public Exception ExceptionPassedToShowException;
		
		public void ShowException(Exception ex, string message)
		{
			ExceptionPassedToShowException = ex;
		}
		
		public void ShowWarning(string message)
		{
		}
		
		public bool AskQuestion(string question, string caption)
		{
			return false;
		}
		
		public int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
		{
			return 0;
		}
		
		public string ShowInputBox(string caption, string dialogText, string defaultValue)
		{
			return String.Empty;
		}
		
		public void ShowMessage(string message, string caption)
		{
		}
		
		public void InformSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot)
		{
		}
		
		public ChooseSaveErrorResult ChooseSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled)
		{
			return ChooseSaveErrorResult.Ignore;
		}
		
		public string DefaultMessageBoxTitle {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string ProductName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void ShowHandledException(Exception ex, string message)
		{
			throw new NotImplementedException();
		}
		
		public void ShowErrorFormatted(string formatstring, params object[] formatitems)
		{
			throw new NotImplementedException();
		}
		
		public void ShowWarningFormatted(string formatstring, params object[] formatitems)
		{
			throw new NotImplementedException();
		}
		
		public void ShowMessageFormatted(string formatstring, string caption, params object[] formatitems)
		{
			throw new NotImplementedException();
		}
	}
}
