// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.SharpDevelop.Util
{
	/// <summary>
	/// An exception thrown by a <see cref="ProcessRunner"/>
	/// instance.
	/// </summary>
	[Serializable()]
	public class ProcessRunnerException : ApplicationException
	{
		public ProcessRunnerException() : base()
		{
		}
		
		public ProcessRunnerException(string message) : base(message)
		{
		}
		
		public ProcessRunnerException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected ProcessRunnerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
