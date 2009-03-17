// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
