// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.InteropServices;
using System.Text;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{	
	/// <summary> This convenience class provides access to an exception within the debugee. </summary>
	/// <seealso cref="System.Exception" />
	public class Exception: DebuggerObject
	{
		Value exception;
		
		public Value Value {
			get { return exception; }
		}
		
		public Exception(Value exception)
		{
			this.exception = exception;
		}
		
		/// <summary> The <c>GetType().FullName</c> of the exception. </summary>
		/// <seealso cref="System.Exception" />
		public string Type {
			get {
				return exception.Type.FullName;
			}
		}
		
		/// <summary> The <c>Message</c> property of the exception. </summary>
		/// <seealso cref="System.Exception" />
		public string Message {
			get {
				Value message = exception.GetMemberValue("_message");
				return message.IsNull ? string.Empty : message.AsString;
			}
		}
		
		public string StackTrace {
			get {
				try {
					Value stackTrace = exception.GetMemberValue("StackTrace");
					return stackTrace.IsNull ? string.Empty : stackTrace.AsString;
				} catch (GetValueException) {
					// Evaluation is not possible after a stackoverflow exception
					return string.Empty;
				}
			}
		}
		
		/// <summary> The <c>InnerException</c> property of the exception. </summary>
		/// <seealso cref="System.Exception" />
		public Exception InnerException {
			get {
				Value innerException = exception.GetMemberValue("_innerException");
				return innerException.IsNull ? null : new Exception(innerException);
			}
		}
		
		public void MakeValuePermanent()
		{
			exception = exception.GetPermanentReference();
		}
		
		public override string ToString()
		{
			return ToString("--- End of inner exception stack trace ---");
		}
		
		public string ToString(string endOfInnerExceptionFormat)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(this.Type);
			if (!string.IsNullOrEmpty(this.Message)) {
				sb.Append(": ");
				sb.Append(this.Message);
			}
			if (this.InnerException != null) {
				sb.Append(" ---> ");
				sb.Append(this.InnerException.ToString(endOfInnerExceptionFormat));
				sb.AppendLine();
				sb.Append("   ");
				sb.Append(endOfInnerExceptionFormat);
			}
			sb.AppendLine();
			sb.Append(this.StackTrace);
			return sb.ToString();
		}
	}
	
	public class ExceptionEventArgs: ProcessEventArgs
	{
		Exception exception;
		ExceptionType exceptionType;
		bool isUnhandled;
		
		public Exception Exception {
			get { return exception; }
		}
		
		public ExceptionType ExceptionType {
			get { return exceptionType; }
		}
		
		public bool IsUnhandled {
			get { return isUnhandled; }
		}
		
		public ExceptionEventArgs(Process process, Exception exception, ExceptionType exceptionType, bool isUnhandled):base(process)
		{
			this.exception = exception;
			this.exceptionType = exceptionType;
			this.isUnhandled = isUnhandled;
		}
	}
}
