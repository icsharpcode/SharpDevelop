// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.NAntAddIn
{
	/// <summary>
	/// The exception that is thrown when a non-fatal
	/// error occurs in the NAnt add-in.
	/// </summary>
	[Serializable()]
	public class NAntAddInException : ApplicationException
	{
		public NAntAddInException()
		{
		}
		
		public NAntAddInException(string message)
			: base(message)
		{
		}
		
		public NAntAddInException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected NAntAddInException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
