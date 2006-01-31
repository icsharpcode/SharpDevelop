// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// An exception thrown by a <see cref="ProcessRunner"/>
	/// instance.
	/// </summary>
	public class ProcessRunnerException : ApplicationException
	{
		public ProcessRunnerException(string message)
			: base(message)
		{
		}
	}
}
