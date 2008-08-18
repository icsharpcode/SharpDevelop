// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Python.Build.Tasks;

namespace Python.Build.Tasks.Tests
{
	/// <summary>
	/// Overrides the GetCurrentFolder to return a predefined string.
	/// </summary>
	public class DummyPythonCompilerTask : PythonCompilerTask
	{
		string currentFolder;
		
		public DummyPythonCompilerTask(IPythonCompiler compiler, string currentFolder)
			: base(compiler)
		{
			this.currentFolder = currentFolder;
		}
		
		protected override string GetCurrentFolder()
		{
			return currentFolder;
		}
	}
}
