using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class ResumeStatement : Statement
	{
		string labelName = "";
		bool next;
		
		public string LabelName
		{
			get {
				return labelName;
			}
			set {
				Debug.Assert(value != null);
				labelName = value;
			}
		}
		
		public bool IsResumeNext {
			get {
				return next;
			}
			set {
				next = value;
			}
		}
		
		public ResumeStatement(bool next)
		{
			this.next = next;
		}
		
		public ResumeStatement(string labelName)
		{
			Debug.Assert(labelName != null);
			this.labelName = labelName;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ResumeStatement: LabelName = {0}, IsResumeNext = {1}]",
			                     LabelName,
			                     IsResumeNext);
		}
	}
}
