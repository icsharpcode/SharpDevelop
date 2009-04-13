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
	/// Exception thrown by the PythonComponentWalker class.
	/// </summary>
	public class PythonComponentWalkerException : Exception
	{
		public PythonComponentWalkerException(string message) : base(message)
		{
		}
	}
}
