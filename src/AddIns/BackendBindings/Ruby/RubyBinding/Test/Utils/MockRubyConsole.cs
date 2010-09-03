// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
