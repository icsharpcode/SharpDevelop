// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Scripting;

namespace ICSharpCode.RubyBinding
{
	public interface IRubyConsolePad
	{
		void BringToFront();
		IScriptingConsoleTextEditor ConsoleTextEditor { get; }
		IRubyConsole RubyConsole { get; }
	}
}
