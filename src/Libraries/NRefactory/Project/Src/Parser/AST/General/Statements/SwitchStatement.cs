using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST 
{
	public class SwitchStatement : BlockStatement
	{
		Expression          switchExpression;
//		List<SwitchSection> switchSections;
		ArrayList switchSections;
		
		public Expression SwitchExpression {
			get {
				return switchExpression;
			}
			set {
				switchExpression = Expression.CheckNull(value);
			}
		}
		
		public ArrayList SwitchSections {
			get {
				return switchSections;
			}
			set {
				switchSections =  value == null ? new ArrayList(1) : value;
			}
		}
		
		public SwitchStatement(Expression switchExpression, ArrayList switchSections)
		{
			this.SwitchExpression = switchExpression;
			this.SwitchSections   = switchSections;
		}
		
		public SwitchStatement(Expression switchExpression)
		{
			this.SwitchExpression = switchExpression;
			this.switchSections   = new ArrayList(1);
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[SwitchStatement: SwitchExpression={0}, SwitchSections={1}]",
			                     SwitchExpression,
			                     GetCollectionString(SwitchSections));
		}
	}
	
	public class SwitchSection : BlockStatement
	{
//		List<CaseLabel> switchLabels;
		ArrayList switchLabels;
		
		public ArrayList SwitchLabels {
			get {
				return switchLabels;
			}
			set {
				switchLabels = value == null ? new ArrayList(1) : value;
			}
		}
		
		public SwitchSection()
		{
			switchLabels = new ArrayList(1);
		}
		
		public SwitchSection(ArrayList switchLabels)
		{
			SwitchLabels = switchLabels;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[SwitchSection: SwitchLabels={0}]",
			                     GetCollectionString(SwitchLabels));
		}
	}
	
	public class CaseLabel : AbstractNode
	{
		Expression         label        = Expression.Null;
		BinaryOperatorType bOp          = BinaryOperatorType.None;
		Expression         toExpression = Expression.Null;
		
		/// <value>null means default case</value>
		public Expression Label {
			get {
				return label;
			}
			set {
				label = Expression.CheckNull(value);
			}
		}
		public bool IsDefault {
			get {
				return label.IsNull;
			}
		}
		
		public BinaryOperatorType BinaryOperatorType {
			get {
				return bOp;
			}
			set {
				bOp = value;
			}
		}
		
		public Expression ToExpression {
			get {
				return toExpression;
			}
			set {
				toExpression = Expression.CheckNull(value);
			}
		}
		
		public CaseLabel(BinaryOperatorType bOp, Expression label)
		{
			this.BinaryOperatorType = bOp;
			this.Label = label;
		}
		
		public CaseLabel(Expression label)
		{
			this.Label = label;
		}
		
		public CaseLabel(Expression label, Expression toExpression)
		{
			this.Label = label;
			this.ToExpression = toExpression;
		}
		
		public CaseLabel()
		{
			this.label = Expression.Null;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[CaseLabel: Label = {0}]",
			                     Label);
		}
	}
}
