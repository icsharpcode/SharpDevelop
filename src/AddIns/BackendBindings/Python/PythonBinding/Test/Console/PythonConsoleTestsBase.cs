// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Console
{
	public class PythonConsoleTestsBase
	{
		public MockConsoleTextEditor MockConsoleTextEditor;
		public TestablePythonConsole TestablePythonConsole;
		
		public void CreatePythonConsole()
		{
			TestablePythonConsole = new TestablePythonConsole();
			MockConsoleTextEditor = TestablePythonConsole.MockConsoleTextEditor;
		}
	}
}
