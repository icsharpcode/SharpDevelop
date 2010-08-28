// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockMessageService : IUnitTestMessageService
	{
		public string QuestionPassedToAskQuestion;
		public string CaptionPassedToAskQuestion;
		public bool AskQuestionReturnValue;
		public string FormatPassedToShowFormattedErrorMessage;
		public string ItemPassedToShowFormattedErrorMessage;
		
		public bool AskQuestion(string question, string caption)
		{
			QuestionPassedToAskQuestion = question;
			CaptionPassedToAskQuestion = caption;
			return AskQuestionReturnValue;
		}
		
		public void ShowFormattedErrorMessage(string format, string item)
		{
			FormatPassedToShowFormattedErrorMessage = format;
			ItemPassedToShowFormattedErrorMessage = item;
		}		
	}
}
