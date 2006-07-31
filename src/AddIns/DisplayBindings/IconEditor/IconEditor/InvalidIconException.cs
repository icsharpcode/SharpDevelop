// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace IconEditor
{
	[Serializable]
	public class InvalidIconException : Exception
	{
		public InvalidIconException() : base()
		{
		}
		
		public InvalidIconException(string message) : base(message)
		{
		}
		
		public InvalidIconException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected InvalidIconException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
