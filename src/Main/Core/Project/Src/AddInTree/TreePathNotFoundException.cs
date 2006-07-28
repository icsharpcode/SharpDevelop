// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Is thrown when the AddInTree could not find the requested path.
	/// </summary>
	[Serializable()]
	public class TreePathNotFoundException : CoreException
	{
		/// <summary>
		/// Constructs a new <see cref="TreePathNotFoundException"/>
		/// </summary>
		public TreePathNotFoundException(string path) : base("Treepath not found: " + path)
		{
		}
		
		// Required for Serialization
		public TreePathNotFoundException() : base()
		{
		}
		
		public TreePathNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected TreePathNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
