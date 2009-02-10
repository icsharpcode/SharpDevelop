// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Exception thrown by the PythonFormWalker class.
	/// </summary>
	public class PythonFormWalkerException : Exception
	{
		public PythonFormWalkerException(string message) : base(message)
		{
		}
	}
}
