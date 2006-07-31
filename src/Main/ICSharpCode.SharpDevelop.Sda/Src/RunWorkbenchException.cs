// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.SharpDevelop.Sda
{
	/// <summary>
	/// Exception used when there's an unhandled exception running the workbench.
	/// </summary>
	[Serializable()]
	public class RunWorkbenchException : Exception
	{
		/// <summary>
		/// Create a new RunWorkbenchException instance.
		/// </summary>
		public RunWorkbenchException() : base()
		{
		}
		
		/// <summary>
		/// Create a new RunWorkbenchException instance.
		/// </summary>
		public RunWorkbenchException(string message) : base(message)
		{
		}
		
		/// <summary>
		/// Create a new RunWorkbenchException instance.
		/// </summary>
		public RunWorkbenchException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		/// <summary>
		/// Create a new RunWorkbenchException instance.
		/// </summary>
		protected RunWorkbenchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
