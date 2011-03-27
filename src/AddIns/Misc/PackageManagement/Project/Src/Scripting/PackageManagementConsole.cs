// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageManagementConsole : ThreadSafeScriptingConsole
	{
		TextEditor textEditor;
		
		public PackageManagementConsole()
			: this(TextEditorFactory.CreateTextEditor())
		{
		}

		public PackageManagementConsole(TextEditor textEditor)
			: this(new ScriptingConsoleTextEditor(textEditor), new ControlDispatcher(textEditor))
		{
			this.textEditor = textEditor;
		}
		
		public PackageManagementConsole(IScriptingConsoleTextEditor textEditor, IControlDispatcher dispatcher)
			: this(new ScriptingConsole(textEditor), dispatcher)
		{
		}
		
		public PackageManagementConsole(IScriptingConsole scriptingConsole, IControlDispatcher dispatcher)
			: base(scriptingConsole, dispatcher)
		{
		}
		
		public TextEditor TextEditor {
			get { return textEditor; }
		}
	}
}
