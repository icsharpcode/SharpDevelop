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
