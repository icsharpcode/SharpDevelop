// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.IconEditor
{
	[Serializable]
	public class InvalidIconException : Exception
	{
		public InvalidIconException() : base()
		{
		}
		
		public InvalidIconException(string message) : base(message)
		{
		}
		
		public InvalidIconException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected InvalidIconException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
