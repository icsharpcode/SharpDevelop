// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
