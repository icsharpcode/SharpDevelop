// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
