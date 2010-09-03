// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.RubyBinding
{
	public interface IRubyConsolePad
	{
		void BringToFront();
		IConsoleTextEditor ConsoleTextEditor { get; }
		IRubyConsole RubyConsole { get; }
	}
}
