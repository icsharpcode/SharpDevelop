/*
 * Created by SharpDevelop.
 * User: HP
 * Date: 28.04.2008
 * Time: 22:18
 */

using System;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using Dom = ICSharpCode.SharpDevelop.Dom;

namespace SharpRefactoring.Visitors
{
	public class FindReferenceVisitor : AbstractAstVisitor
	{
		List<IdentifierExpression> identifiers;
		string name;
		TypeReference type;
		
		public List<IdentifierExpression> Identifiers {
			get { return identifiers; }
		}
		
		public FindReferenceVisitor(string name, TypeReference type)
		{
			this.identifiers = new List<IdentifierExpression>();
			this.name = name;
			this.type = type;
		}
		
		public override object VisitIdentifierExpression(ICSharpCode.NRefactory.Ast.IdentifierExpression identifierExpression, object data)
		{
			if (identifierExpression.Identifier == name) {
				identifiers.Add(identifierExpression);
			}
			return base.VisitIdentifierExpression(identifierExpression, data);
		}
	}
}
