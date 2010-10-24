// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Visitors;

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
		
		public string CodeStatementToString(System.CodeDom.CodeStatement statement)
		{
			CodeDomVerboseOutputGenerator outputGenerator = new CodeDomVerboseOutputGenerator();
			using(StringWriter sw = new StringWriter(System.Globalization.CultureInfo.InvariantCulture)) {
				outputGenerator.PublicGenerateCodeFromStatement(statement, sw, null);
				return sw.ToString().TrimEnd();
			}
		}
	}
}
