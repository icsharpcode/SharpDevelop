// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// The exception that is thrown when a non-fatal
	/// error occurs in the code coverage add-in.
	/// </summary>
	[Serializable()]
	public class CodeCoverageException : Exception
	{
		public CodeCoverageException()
		{
		}
		
		public CodeCoverageException(string message)
			: base(message)
		{
		}
		
		public CodeCoverageException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected CodeCoverageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
