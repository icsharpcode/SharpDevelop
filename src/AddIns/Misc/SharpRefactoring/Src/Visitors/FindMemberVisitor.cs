/*
 * Created by SharpDevelop.
 * User: HP
 * Date: 28.04.2008
 * Time: 22:18
 */

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace SharpRefactoring.Visitors
{
	public class FindMemberVisitor : AbstractAstVisitor
	{
		int startColumn, startLine;
		int endColumn, endLine;
		ParametrizedNode member = null;
		
		public ParametrizedNode Member {
			get { return member; }
		}
		
		public FindMemberVisitor(int startColumn, int startLine, int endColumn, int endLine)
		{
			this.startColumn = startColumn;
			this.startLine = startLine;
			this.endColumn = endColumn;
			this.endLine = endLine;
		}
		
		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			if ((methodDeclaration.Body.StartLocation < new Location(startColumn + 1, startLine + 1)) &&
			    (methodDeclaration.Body.EndLocation > new Location(endColumn + 1, endLine + 1))) {
				this.member = methodDeclaration;
			}
			
			return base.VisitMethodDeclaration(methodDeclaration, data);
		}
		
		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			if ((propertyDeclaration.BodyStart < new Location(startColumn + 1, startLine + 1)) &&
			    (propertyDeclaration.BodyEnd > new Location(endColumn + 1, endLine + 1))) {
				this.member = propertyDeclaration;
			}
			return base.VisitPropertyDeclaration(propertyDeclaration, data);
		}
		
		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			if ((constructorDeclaration.Body.StartLocation < new Location(startColumn + 1, startLine + 1)) &&
			    (constructorDeclaration.Body.EndLocation > new Location(endColumn + 1, endLine + 1))) {
				this.member = constructorDeclaration;
			}
			
			return base.VisitConstructorDeclaration(constructorDeclaration, data);
		}
		
		public override object VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
		{
			if ((operatorDeclaration.Body.StartLocation < new Location(startColumn + 1, startLine + 1)) &&
			    (operatorDeclaration.Body.EndLocation > new Location(endColumn + 1, endLine + 1))) {
				this.member = operatorDeclaration;
			}
			
			return base.VisitOperatorDeclaration(operatorDeclaration, data);
		}
	}
}
