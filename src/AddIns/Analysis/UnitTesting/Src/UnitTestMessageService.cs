// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.Core.Services;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestMessageService : IUnitTestMessageService
	{
		public bool AskQuestion(string question, string caption)
		{
			return ServiceManager.Instance.MessageService.AskQuestion(question, caption);
		}
		
		public void ShowFormattedErrorMessage(string format, string item)
		{
			format = StringParser.Parse(format);
			string message = String.Format(format, item);
			ServiceManager.Instance.MessageService.ShowError(message);
		}
	}
}
