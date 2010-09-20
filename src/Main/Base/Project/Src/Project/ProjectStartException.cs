// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// This exception occurs when a project cannot be started.
	/// </summary>
	[Serializable]
	public class ProjectStartException : Exception
	{
		public ProjectStartException() : base()
		{
		}
		
		public ProjectStartException(string message) : base(message)
		{
		}
		
		public ProjectStartException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected ProjectStartException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
