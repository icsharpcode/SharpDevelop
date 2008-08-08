/*
 * Created by SharpDevelop.
 * User: HP
 * Date: 28.04.2008
 * Time: 22:18
 */

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace SharpRefactoring.Visitors
{
	public class FindLocalVariablesVisitor : AbstractAstVisitor
	{
		List<VariableDeclaration> variables;
		
		TypeReference currentType;
		Location start, end;
		
		public List<VariableDeclaration> Variables {
			get { return variables; }
		}
		
		public FindLocalVariablesVisitor()
		{
			this.variables = new List<VariableDeclaration>();
		}
		
		public override object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			this.currentType = localVariableDeclaration.TypeReference;
			this.start = localVariableDeclaration.StartLocation;
			this.end = localVariableDeclaration.EndLocation;
			return base.VisitLocalVariableDeclaration(localVariableDeclaration, data);
		}
		
		public override object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			variableDeclaration.TypeReference = currentType;
			variableDeclaration.StartLocation = start;
			variableDeclaration.EndLocation = end;
			System.Diagnostics.Debug.Print(variableDeclaration.TypeReference + " " + variableDeclaration.Name);
			this.variables.Add(variableDeclaration);
			return base.VisitVariableDeclaration(variableDeclaration, data);
		}
		
		public override object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			VariableDeclaration v = new VariableDeclaration(foreachStatement.VariableName, Expression.Null, foreachStatement.TypeReference);
			v.StartLocation = foreachStatement.StartLocation;
			v.EndLocation = foreachStatement.StartLocation;
			return base.VisitForeachStatement(foreachStatement, data);
		}
		
		public override object VisitCatchClause(CatchClause catchClause, object data)
		{
			VariableDeclaration v =
				new VariableDeclaration(catchClause.VariableName,
				                        new ObjectCreateExpression(catchClause.TypeReference,
				                                                   new List<Expression>()),
				                        catchClause.TypeReference);
			
			v.StartLocation = catchClause.StartLocation;
			v.EndLocation   = catchClause.EndLocation;
			
			this.variables.Add(v);
			
			return base.VisitCatchClause(catchClause, data);
		}
	}
}
