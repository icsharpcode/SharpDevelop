// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.RubyBinding;

namespace RubyBinding.Tests.Utils
{
	public class MockRubyConsole : IRubyConsole
	{
		public string TextPassedToSendLine;
		
		public void SendLine(string text)
		{
			TextPassedToSendLine = text;
		}
	}
}
