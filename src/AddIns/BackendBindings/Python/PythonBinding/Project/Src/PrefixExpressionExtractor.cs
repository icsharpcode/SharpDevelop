using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// This Vistor is used to locate Prefix expressions and insert them
	///   in statement block prior to owned statement.  This is probably expensive
	///   but without caching on statement writes no easy way to do it.
	///   May be faster to do once per Method to see if any Prefix exist at all
	///    and then disable checks while in that method
	/// </summary>
	internal class PrefixExpressionExtractor : NodeTrackingAstVisitor
	{
		readonly List<Expression> statements = new List<Expression>();
		private int statementRecursion;
		public List<Expression> Statements
		{
			get { return statements; }
		}

		public void Reset()
		{
			statementRecursion = 0;
		}

		protected override void BeginVisit(INode node)
		{
			if (node is Statement)
				statementRecursion++;
			base.BeginVisit(node);
		}
		protected override void EndVisit(INode node)
		{
			if (node is Statement)
				statementRecursion--;
			base.EndVisit(node);
		}
		public override object TrackedVisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			// only accumulate if current statement is active. And also not immediate parent
			if (statementRecursion == 1 && !(unaryOperatorExpression.Parent is Statement)) {
				switch (unaryOperatorExpression.Op) {
					case UnaryOperatorType.Increment:
					case UnaryOperatorType.Decrement:
						statements.Add(unaryOperatorExpression);
						break;
				}
			}
			return base.TrackedVisitUnaryOperatorExpression(unaryOperatorExpression, data);
		}
		public override object TrackedVisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			if (statementRecursion == 1 && !(anonymousMethodExpression.Parent is Statement)) {
				statements.Add(anonymousMethodExpression);
			}
			return null;
		}
		public override object TrackedVisitLambdaExpression(LambdaExpression lambdaExpression, object data)
		{
			if (statementRecursion == 1 && !(lambdaExpression.Parent is Statement)) {
				if (!lambdaExpression.StatementBody.IsNull)
					statements.Add(lambdaExpression);
			}
			return base.TrackedVisitLambdaExpression(lambdaExpression, data);
		}
	}
}