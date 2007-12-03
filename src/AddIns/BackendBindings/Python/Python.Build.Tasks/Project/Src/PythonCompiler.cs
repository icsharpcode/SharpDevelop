// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using IronPy = IronPython.Hosting;

namespace ICSharpCode.Python.Build.Tasks
{
	/// <summary>
	/// Wraps the IronPython.Hosting.PythonCompiler class so it 
	/// implements the IPythonCompiler interface.
	/// </summary>
	public class PythonCompiler : IronPy.PythonCompiler, IPythonCompiler
	{
		public PythonCompiler()
			: base(null, null)
		{
		}
	}
}
