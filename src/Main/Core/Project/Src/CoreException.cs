// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Base class for exceptions thrown by the SharpDevelop core.
	/// </summary>
	[Serializable()]
	public class CoreException : Exception
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
