// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Text;

namespace Debugger
{
	enum ExceptionType
	{
		FirstChance = 1,
		UserFirstChance = 2,
		CatchHandlerFound = 3,
		Unhandled = 4,
	}
	
	/// <summary> This convenience class provides access to an exception within the debugee. </summary>
	/// <seealso cref="System.Exception" />
	public class Exception: DebuggerObject
	{
		Value exception;
		
		public Value Value {
			get { return exception; }
		}
		
		ExceptionType ExceptionType { get; set; }
		
		public bool IsUnhandled {
			get { return this.ExceptionType == ExceptionType.Unhandled; }
		}
		
		internal Exception(Value exception, ExceptionType exceptionType)
		{
			this.exception = exception;
			this.ExceptionType = exceptionType;
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
				Value message = exception.GetFieldValue("_message");
				return message.IsNull ? string.Empty : message.AsString();
			}
		}
		
		/// <summary> The <c>InnerException</c> property of the exception. </summary>
		/// <seealso cref="System.Exception" />
		public Exception InnerException {
			get {
				Value innerException = exception.GetFieldValue("_innerException");
				return innerException.IsNull ? null : new Exception(innerException, this.ExceptionType);
			}
		}
		
		public void MakeValuePermanent()
		{
			exception = exception.GetPermanentReferenceOfHeapValue();
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(this.Type);
			if (!string.IsNullOrEmpty(this.Message)) {
				sb.Append(": ");
				sb.Append(this.Message);
			}
			if (this.InnerException != null) {
				sb.Append(" ---> ");
				sb.Append(this.InnerException.ToString());
			}
			return sb.ToString();
		}
		
		public string GetStackTrace(Thread evalThread)
		{
			return GetStackTrace(evalThread, "--- End of inner exception stack trace ---");
		}
		
		/// <summary> Returs formated stacktrace for the exception </summary>
		/// <exception cref="GetValueException"> Getting the stacktrace involves property
		/// evaluation so GetValueException can be thrown in some cicumstances. </exception>
		public string GetStackTrace(Thread evalThread, string endOfInnerExceptionFormat)
		{
			StringBuilder sb = new StringBuilder();
			if (this.InnerException != null) {
				sb.Append(this.InnerException.GetStackTrace(evalThread, endOfInnerExceptionFormat));
				sb.Append("   ");
				sb.Append(endOfInnerExceptionFormat);
				sb.AppendLine();
			}
			// Note that evaluation is not possible after a stackoverflow exception
			Value stackTrace = exception.GetMemberValue(evalThread, "StackTrace");
			if (!stackTrace.IsNull) {
				sb.Append(stackTrace.AsString());
				sb.AppendLine();
			}
			return sb.ToString();
		}
	}
}
