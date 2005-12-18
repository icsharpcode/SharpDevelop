// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.Core
{
	[Serializable()]
	public class CoreException : ApplicationException
	{
		public CoreException() : base()
		{
		}
		
		public CoreException(string message) : base(message)
		{
		}
		
		public CoreException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected CoreException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
