// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Scripting
{
	public interface IScriptingConsole
	{
		void SendLine(string line);
		void SendText(string text);
	}
}
