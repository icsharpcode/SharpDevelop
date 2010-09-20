// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
