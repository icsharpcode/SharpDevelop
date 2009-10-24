// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory.Ast;

namespace Debugger
{
	public class GetValueException: DebuggerException
	{
		INode expression;
		string error;
		
		/// <summary> Expression that has caused this exception to occur </summary>
		public INode Expression {
			get { return expression; }
			set { expression = value; }
		}
		
		public string Error {
			get { return error; }
		}
		
		public override string Message {
			get {
				if (expression == null) {
					return error;
				} else {
					return String.Format("Error evaluating \"{0}\": {1}", expression.PrettyPrint(), error);
				}
			}
		}
		
		public GetValueException(INode expression, string error):base(error)
		{
			this.expression = expression;
			this.error = error;
		}
		
		public GetValueException(string error, System.Exception inner):base(error, inner)
		{
			this.error = error;
		}
		
		public GetValueException(string error):base(error)
		{
			this.error = error;
		}
	}
}
