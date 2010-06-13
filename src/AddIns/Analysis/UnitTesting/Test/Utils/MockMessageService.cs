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
	}
}
