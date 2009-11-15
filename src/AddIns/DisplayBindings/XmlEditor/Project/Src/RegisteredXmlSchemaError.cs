// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.XmlEditor
{
	public class RegisteredXmlSchemaError
	{
		string message;
		Exception exception;
		
		public RegisteredXmlSchemaError(string message)
			: this(message, null)
		{
		}
		
		public RegisteredXmlSchemaError(string message, Exception exception)
		{
			this.message = message;
			this.exception = exception;	
		}
		
		public string Message {
			get { return message; }
		}
		
		public Exception Exception {
			get { return exception; }
		}
		
		public bool HasException {
			get { return exception != null; }
		}
		
		public override string ToString()
		{
			if (HasException) {
				return message + Environment.NewLine + exception.Message;
			}
			return message;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object obj)
		{
			RegisteredXmlSchemaError rhs = obj as RegisteredXmlSchemaError;
			return (message == rhs.message) && Object.ReferenceEquals(exception, rhs.exception);
		}
	}
}
