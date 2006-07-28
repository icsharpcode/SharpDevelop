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
	/// Exception used when loading an AddIn fails.
	/// </summary>
	[Serializable]
	public class AddInLoadException : CoreException
	{
		public AddInLoadException() : base()
		{
		}
		
		public AddInLoadException(string message) : base(message)
		{
		}
		
		public AddInLoadException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected AddInLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
