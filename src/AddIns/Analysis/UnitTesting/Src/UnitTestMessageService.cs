// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
