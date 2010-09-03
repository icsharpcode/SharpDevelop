// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
