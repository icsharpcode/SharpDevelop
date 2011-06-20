// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core.Services;

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
		
		public void InformSaveError(string fileName, string message, string dialogName, Exception exceptionGot)
		{
		}
		
		public ChooseSaveErrorResult ChooseSaveError(string fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled)
		{
			return ChooseSaveErrorResult.Ignore;
		}
	}
}
