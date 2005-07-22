// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class RaiseEventStatement : Statement
	{
		string eventName = "";
//		List<Expression> parameters = new List<Expression>(1);
		ArrayList parameters = new ArrayList(1);
		
		public string EventName {
			get {
				return eventName;
			}
			set {
				Debug.Assert(value != null);
				eventName = value;
			}
		}
		public ArrayList Parameters {
			get {
				return parameters;
			}
			set {
				Debug.Assert(value != null);
				parameters = value;
			}
		}
		
		public RaiseEventStatement(string eventName, ArrayList parameters)
		{
			Debug.Assert(eventName != null);
			this.eventName = eventName;
			Debug.Assert(parameters != null);
			this.parameters = parameters;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return  String.Format("[RaiseEventStatement: EventName={0}, Parameters={1}]", 
			                     EventName,
			                     GetCollectionString(parameters));
		}
	}
}
