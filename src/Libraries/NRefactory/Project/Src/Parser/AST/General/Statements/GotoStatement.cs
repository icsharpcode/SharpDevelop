using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST 
{
	public class GotoStatement : Statement
	{
		string label = "";
		
		public string Label {
			get {
				return label;
			}
			set {
				Debug.Assert(value != null);
				label = value;
			}
		}
		
		public GotoStatement(string label)
		{
			Debug.Assert(label != null);
			this.label = label;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[GotoStatement: Label={0}]",
			                     label);
		}
	}
}
