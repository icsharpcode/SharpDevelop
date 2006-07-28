// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
