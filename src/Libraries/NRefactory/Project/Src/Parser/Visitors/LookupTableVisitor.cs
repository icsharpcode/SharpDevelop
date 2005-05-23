using System;
using System.Drawing;
using System.Collections.Generic;

using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Parser
{
	public class LocalLookupVariable
	{
		TypeReference typeRef;
		Point         startPos;
		Point         endPos;
		
		public TypeReference TypeRef {
			get {
				return typeRef;
			}
		}
		public Point StartPos {
			get {
				return startPos;
			}
		}
		public Point EndPos {
			get {
				return endPos;
			}
		}
		
		public LocalLookupVariable(TypeReference typeRef, Point startPos, Point endPos)
		{
			this.typeRef = typeRef;
			this.startPos = startPos;
			this.endPos = endPos;
		}
	}
	
	public class LookupTableVisitor : AbstractASTVisitor
	{
		Dictionary<string, List<LocalLookupVariable>> variables;
		
		public Dictionary<string, List<LocalLookupVariable>> Variables {
			get {
				return variables;
			}
		}
		
		public LookupTableVisitor(StringComparer nameComparer)
		{
			variables = new Dictionary<string, List<LocalLookupVariable>>(nameComparer);
		}
		
		public void AddVariable(TypeReference typeRef, string name, Point startPos, Point endPos)
		{
			if (name == null || name.Length == 0) {
				return;
			}
			List<LocalLookupVariable> list;
			if (!variables.ContainsKey(name)) {
				variables[name] = list = new List<LocalLookupVariable>();
			} else {
				list = (List<LocalLookupVariable>)variables[name];
			}
			list.Add(new LocalLookupVariable(typeRef, startPos, endPos));
		}
		
		// todo: move this method to a better place.
		bool IsInside(Point between, Point start, Point end)
		{
			if (between.Y < start.Y || between.Y > end.Y) {
//				Console.WriteLine("Y = {0} not between {1} and {2}", between.Y, start.Y, end.Y);
				return false;
			}
			if (between.Y > start.Y) {
				if (between.Y < end.Y) {
					return true;
				}
				// between.Y == end.Y
//				Console.WriteLine("between.Y = {0} == end.Y = {1}", between.Y, end.Y);
//				Console.WriteLine("returning {0}:, between.X = {1} <= end.X = {2}", between.X <= end.X, between.X, end.X);
				return between.X <= end.X;
			}
			// between.Y == start.Y
//			Console.WriteLine("between.Y = {0} == start.Y = {1}", between.Y, start.Y);
			if (between.X < start.X) {
				return false;
			}
			// start is OK and between.Y <= end.Y
			return between.Y < end.Y || between.X <= end.X;
		}
		
		public override object Visit(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			for (int i = 0; i < localVariableDeclaration.Variables.Count; ++i) {
				VariableDeclaration varDecl = (VariableDeclaration)localVariableDeclaration.Variables[i];
				
				AddVariable(localVariableDeclaration.GetTypeForVariable(i),
				            varDecl.Name,
				            localVariableDeclaration.StartLocation,
				            CurrentBlock == null ? new Point(-1, -1) : CurrentBlock.EndLocation);
			}
			return data;
		}
		
		public override object Visit(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			foreach (ParameterDeclarationExpression p in anonymousMethodExpression.Parameters) {
				AddVariable(p.TypeReference, p.ParameterName, anonymousMethodExpression.StartLocation, anonymousMethodExpression.EndLocation);
			}
			return base.Visit(anonymousMethodExpression, data);
		}
		
		// ForStatement and UsingStatement use a LocalVariableDeclaration,
		// so they don't need to be visited separately
		
		public override object Visit(ForeachStatement foreachStatement, object data)
		{
			AddVariable(foreachStatement.TypeReference,
			            foreachStatement.VariableName,
			            foreachStatement.StartLocation,
			            foreachStatement.EndLocation);
			
			if (foreachStatement.Expression != null) {
				foreachStatement.Expression.AcceptVisitor(this, data);
			}
			if (foreachStatement.EmbeddedStatement == null) {
				return data;
			}
			return foreachStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}
		
		
		
		public override object Visit(TryCatchStatement tryCatchStatement, object data)
		{
			if (tryCatchStatement == null) {
				return data;
			}
			if (tryCatchStatement.StatementBlock != null) {
				tryCatchStatement.StatementBlock.AcceptVisitor(this, data);
			}
			if (tryCatchStatement.CatchClauses != null) {
				foreach (CatchClause catchClause in tryCatchStatement.CatchClauses) {
					if (catchClause != null) {
						if (catchClause.TypeReference != null && catchClause.VariableName != null) {
							AddVariable(catchClause.TypeReference,
							            catchClause.VariableName,
							            catchClause.StatementBlock.StartLocation,
							            catchClause.StatementBlock.EndLocation);
						}
						catchClause.StatementBlock.AcceptVisitor(this, data);
					}
				}
			}
			if (tryCatchStatement.FinallyBlock != null) {
				return tryCatchStatement.FinallyBlock.AcceptVisitor(this, data);
			}
			return data;
		}
	}
}
