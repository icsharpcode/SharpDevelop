// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Scripting;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockScriptingConsole : IScriptingConsole
	{
		public string TextPassedToSendLine;
		
		public void SendLine(string text)
		{
			TextPassedToSendLine = text;
		}
	}
}
