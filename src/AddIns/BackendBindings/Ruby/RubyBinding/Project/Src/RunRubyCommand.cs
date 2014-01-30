// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Runs the Ruby console passing the filename of the 
	/// active Ruby script open in SharpDevelop.
	/// </summary>
	public class RunRubyCommand : RunScriptingConsoleApplicationCommand
	{		
		public RunRubyCommand()
			: this(new RubyWorkbench(), new RubyAddInOptions(), DebuggerService.CurrentDebugger)
		{
		}
		
		public RunRubyCommand(IScriptingWorkbench workbench, RubyAddInOptions options, IDebugger debugger)
			: base(workbench, debugger, new RubyConsoleApplication(options))
		{
		}
	}
}
