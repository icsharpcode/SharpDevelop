// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

namespace DebuggerLibrary
{
	public class DebuggerException: System.Exception
	{
		public DebuggerException() {}
		public DebuggerException(string message): base(message) {}
		public DebuggerException(string message, System.Exception inner): base(message, inner) {}
	}

	public class UnableToGetPropertyException: DebuggerException
	{
		public object sender;
		public string property;
		public string hint;

		public UnableToGetPropertyException(object sender, string property): this(sender, property, null) 
		{
		}

		public UnableToGetPropertyException(object sender, string property, string hint): base() 
		{
            this.sender   = sender;
			this.property = property;
			this.hint     = hint;
		}

		public override string Message {
			get {
				return "Sender of exception: " + sender.GetType().ToString() + "\n" +
				       "Description of sender: " + sender.ToString() + "\n" +
				       "Unable to get property: " + property.ToString() + "\n" +
				       ((hint != null)?("Comment: " + hint + "\n"):"");
			}
		}
	}

	public class BadSignatureException: DebuggerException {}

	public class NotAviableException: DebuggerException {}
	public class FrameNotAviableException: NotAviableException {}
	public class SymbolsNotAviableException: NotAviableException {}
	public class CurrentFunctionNotAviableException: NotAviableException {}
	public class CurrentThreadNotAviableException: NotAviableException {}
	public class NextStatementNotAviableException: NotAviableException {}
}
