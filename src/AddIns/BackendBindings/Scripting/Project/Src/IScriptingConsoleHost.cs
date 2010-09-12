// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Scripting
{
	public interface IScriptingConsoleHost : IDisposable
	{
		IScriptingConsole ScriptingConsole { get; }
		void Run();
	}
}
