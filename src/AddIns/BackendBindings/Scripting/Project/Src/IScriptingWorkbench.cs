// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
