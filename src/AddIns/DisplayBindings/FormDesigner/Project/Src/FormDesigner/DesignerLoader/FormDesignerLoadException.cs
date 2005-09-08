/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 08.09.2005
 * Time: 17:25
 */

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.FormDesigner
{
	[Serializable()]
	public class FormDesignerLoadException : ApplicationException
	{
		public FormDesignerLoadException() : base()
		{
		}
		
		public FormDesignerLoadException(string message) : base(message)
		{
		}
		
		public FormDesignerLoadException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected FormDesignerLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
