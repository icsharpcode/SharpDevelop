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
	/// The exception that is thrown when a non-fatal 
	/// error occurs in the code coverage add-in.
	/// </summary>
	public class CodeCoverageException : ApplicationException
	{
		public CodeCoverageException()
		{
		}
		
		public CodeCoverageException(string message)
			: base(message)
		{
		}
	}
}
