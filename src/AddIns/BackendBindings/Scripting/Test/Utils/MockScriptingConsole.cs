// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockScriptingConsole : IScriptingConsole
	{
		public string TextPassedToSendLine;
		public string TextPassedToSendText;
		
		public void SendLine(string text)
		{
			TextPassedToSendLine = text;
		}
		
		public void SendText(string text)
		{
			TextPassedToSendText = text;
		}
	}
}
