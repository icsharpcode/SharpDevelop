// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core.Services;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockMessageService : IMessageService
	{
		string question;
		string caption;
		bool returnValue;
		
		public bool AskQuestionReturnValue {
			get { return returnValue; }
			set { returnValue = value; }
		}
		
		public bool AskQuestion(string question, string caption)
		{
			this.question = question;
			this.caption = caption;
			return returnValue;
		}
		
		public string Question {
			get { return question; }
		}
		
		public string Caption {
			get { return caption; }
		}
		
		void IMessageService.ShowError(string message)
		{
			throw new NotImplementedException();
		}
		
		void IMessageService.ShowException(Exception ex, string message)
		{
			throw new NotImplementedException();
		}
		
		void IMessageService.ShowWarning(string message)
		{
			throw new NotImplementedException();
		}
		
		int IMessageService.ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
		{
			throw new NotImplementedException();
		}
		
		string IMessageService.ShowInputBox(string caption, string dialogText, string defaultValue)
		{
			throw new NotImplementedException();
		}
		
		void IMessageService.ShowMessage(string message, string caption)
		{
			throw new NotImplementedException();
		}
		
		void IMessageService.InformSaveError(string fileName, string message, string dialogName, Exception exceptionGot)
		{
			throw new NotImplementedException();
		}
		
		ChooseSaveErrorResult IMessageService.ChooseSaveError(string fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled)
		{
			throw new NotImplementedException();
		}
	}
}
