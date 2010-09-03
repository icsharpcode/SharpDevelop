// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.FormsDesigner
{
	[Serializable()]
	public class FormsDesignerLoadException : Exception
	{
		public FormsDesignerLoadException() : base()
		{
		}
		
		public FormsDesignerLoadException(string message) : base(message)
		{
		}
		
		public FormsDesignerLoadException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected FormsDesignerLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
