// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.Profiler.Controller
{
	/// <summary>
	/// Represents errors that occur during execution of the profiler/unmanaged hook.
	/// </summary>
	[Serializable]
	public class ProfilerException : Exception
	{
		/// <summary>
		/// Creates a new instance of ProfilerException.
		/// </summary>
		public ProfilerException()
		{
		}
		
		/// <summary>
		/// Creates a new instance of ProfilerException.
		/// </summary>
		public ProfilerException(string message)
			: base(message)
		{
		}
		
		/// <summary>
		/// Creates a new instance of ProfilerException.
		/// </summary>
		public ProfilerException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
		
		/// <summary>
		/// Creates a new instance of ProfilerException.
		/// </summary>
		protected ProfilerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
