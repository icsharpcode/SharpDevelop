// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
