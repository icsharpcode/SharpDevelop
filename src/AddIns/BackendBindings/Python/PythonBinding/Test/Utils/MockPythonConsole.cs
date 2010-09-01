// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;

namespace PythonBinding.Tests.Utils
{
	public class MockPythonConsole : IPythonConsole
	{
		public string TextPassedToSendLine;
		
		public void SendLine(string text)
		{
			TextPassedToSendLine = text;
		}
	}
}
