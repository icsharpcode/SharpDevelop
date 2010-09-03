// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.RubyBinding
{
	public interface IRubyWorkbench
	{
		IRubyConsolePad GetRubyConsolePad();
		IViewContent ActiveViewContent { get; }
	}
}
