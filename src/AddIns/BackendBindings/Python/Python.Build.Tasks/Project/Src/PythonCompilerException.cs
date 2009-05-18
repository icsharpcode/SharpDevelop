// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Python.Build.Tasks
{
	public class PythonCompilerException : ApplicationException
	{
		public PythonCompilerException(string message) : base(message)
		{
		}
	}
}
