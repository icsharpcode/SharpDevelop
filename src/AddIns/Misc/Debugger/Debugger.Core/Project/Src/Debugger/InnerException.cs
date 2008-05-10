// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Justin Dearing" email="zippy1981@gmail.com"/>
//     <version>$Revision: 2900 $</version>
// </file>

using System.Text;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	/// <summary>
	/// This contains the nested InnerExceptions of a <c>Debugger.Exception</c> class
	/// </summary>
	/// <see cref="Debugger.Exception" />
	/// <seealso cref="System.Exception" />
	public class DebugeeInnerException : IDebugeeException
	{
		Debugger.Value exceptionValue;
		
		internal DebugeeInnerException (Debugger.Value exception) {
			this.exceptionValue = exception;
		}
		
		/// <summary>
		/// The <c>InnerException</c> property of the exception.
		/// </summary>
		/// <seealso cref="System.Exception" />
		public DebugeeInnerException InnerException {
			get {
				Debugger.Value exVal = this.exceptionValue.GetMemberValue("_innerException");
				return  (exVal.IsNull) ?  null : new DebugeeInnerException(exVal);
			}
		}
		
		/// <summary>
		/// The <c>Message</c> property of the exception.
		/// </summary>
		/// <seealso cref="System.Exception" />
		public string Message {
			get {
				return this.exceptionValue.GetMemberValue("_message").AsString;
			}
		}
		
		/// <summary>
		/// The <c>GetType().FullName</c> of the exception.
		/// </summary>
		/// <seealso cref="System.Exception" />
		public string Type {
			get {
				return this.exceptionValue.Type.FullName;
			}
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Type: {0}", this.Type);
			sb.AppendLine();
			sb.AppendLine("Message:");
			sb.Append(this.Message);
			
			return sb.ToString();
		}
	}
}
