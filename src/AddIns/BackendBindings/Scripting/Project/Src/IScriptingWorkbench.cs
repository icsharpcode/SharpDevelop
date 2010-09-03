// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Scripting
{
	public interface IScriptingWorkbench
	{
		IScriptingConsolePad GetScriptingConsolePad();
		IViewContent ActiveViewContent { get; }
	}
}
