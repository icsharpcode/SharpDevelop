/*
 * Created by SharpDevelop.
 * User: HP
 * Date: 28.04.2008
 * Time: 22:18
 */

using System;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace SharpRefactoring.Visitors
{
	public class HasAssignmentsVisitor : AbstractAstVisitor
	{
		string name;
		TypeReference type;
		bool hasAssignment = false;
		
		public bool HasAssignment {
			get { return hasAssignment; }
		}
		
		public HasAssignmentsVisitor(string name, TypeReference type)
		{
			this.name = name;
			this.type = type;
		}
		
		public override object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			if (!hasAssignment) {
				if (assignmentExpression.Left is IdentifierExpression) {
					hasAssignment = (((IdentifierExpression)assignmentExpression.Left).Identifier == name);
				}
			}
			return base.VisitAssignmentExpression(assignmentExpression, data);
		}
	}
}
