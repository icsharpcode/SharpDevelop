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
