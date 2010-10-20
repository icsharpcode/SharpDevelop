// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.FormsDesigner.Services
{
	/// <summary>
	/// Description of MessageService.
	/// </summary>
	public class FormsMessageService : IMessageService
	{
		public void ShowOutputPad()
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
		}
		
		public void AppendTextToBuildMessages(string text)
		{
			TaskService.BuildMessageViewCategory.AppendText(StringParser.Parse(text));
		}
		
		public void ShowException(Exception ex, string message)
		{
			MessageService.ShowException(ex, message);
		}
	}
}
