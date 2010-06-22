// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;
using ICSharpCode.PythonBinding;

namespace PythonBinding.Tests.Utils
{
	public class MockConsoleTextEditorKeyEventArgs : ConsoleTextEditorKeyEventArgs
	{
		bool handled;
		
		public MockConsoleTextEditorKeyEventArgs(Key key)
			: base(key)
		{
		}
		
		public override bool Handled {
			get { return handled; }
			set { handled = value; }
		}
	}
}
