// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Exception used when loading of a project fails.
	/// </summary>
	[Serializable()]
	public class ProjectLoadException : Exception
	{
		public ProjectLoadException() : base()
		{
		}
		
		public ProjectLoadException(string message) : base(message)
		{
		}
		
		public ProjectLoadException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected ProjectLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
