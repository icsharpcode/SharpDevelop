// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Scripting
{
	public interface IScriptingConsole : IDisposable
	{
		event EventHandler LineReceived;
				
		bool ScrollToEndWhenTextWritten { get; set; }
		
		void Clear();
		void SendLine(string line);
		void SendText(string text);
		void WriteLine();
		void WriteLine(string text, ScriptingStyle style);
		void Write(string text, ScriptingStyle style);
		string ReadLine(int autoIndentSize);
		string ReadFirstUnreadLine();
		int GetMaximumVisibleColumns();
	}
}
