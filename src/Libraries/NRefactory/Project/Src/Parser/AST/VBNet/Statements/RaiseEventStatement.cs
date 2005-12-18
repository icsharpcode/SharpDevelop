// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class RaiseEventStatement : Statement
	{
		string eventName = "";
		List<Expression> arguments = new List<Expression>(1);
		
		public string EventName {
			get {
				return eventName;
			}
			set {
				Debug.Assert(value != null);
				eventName = value;
			}
		}
		public List<Expression> Arguments {
			get {
				return arguments;
			}
			set {
				Debug.Assert(value != null);
				arguments = value;
			}
		}
		
		public RaiseEventStatement(string eventName, List<Expression> arguments)
		{
			Debug.Assert(eventName != null);
			this.eventName = eventName;
			this.arguments = arguments ?? new List<Expression>();
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return  String.Format("[RaiseEventStatement: EventName={0}, Parameters={1}]",
			                      EventName,
			                      GetCollectionString(arguments));
		}
	}
}
