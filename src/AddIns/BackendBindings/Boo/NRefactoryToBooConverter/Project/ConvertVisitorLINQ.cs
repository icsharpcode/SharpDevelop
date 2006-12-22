// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Boo.Lang.Compiler;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using B = Boo.Lang.Compiler.Ast;

namespace NRefactoryToBooConverter
{
	partial class ConvertVisitor
	{
		public object VisitQueryExpression(QueryExpression queryExpression, object data)
		{
			AddError(queryExpression, "QueryExpression is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionFromClause(QueryExpressionFromClause queryExpressionFromClause, object data)
		{
			AddError(queryExpressionFromClause, "QueryExpressionFromClause is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionFromGenerator(QueryExpressionFromGenerator queryExpressionFromGenerator, object data)
		{
			AddError(queryExpressionFromGenerator, "QueryExpressionFromGenerator is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionGroupClause(QueryExpressionGroupClause queryExpressionGroupClause, object data)
		{
			AddError(queryExpressionGroupClause, "QueryExpressionGroupClause is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionIntoClause(QueryExpressionIntoClause queryExpressionIntoClause, object data)
		{
			AddError(queryExpressionIntoClause, "QueryExpressionIntoClause is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionOrdering(QueryExpressionOrdering queryExpressionOrdering, object data)
		{
			AddError(queryExpressionOrdering, "QueryExpressionOrdering is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionSelectClause(QueryExpressionSelectClause queryExpressionSelectClause, object data)
		{
			AddError(queryExpressionSelectClause, "queryExpressionSelectClause is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionWhereClause(QueryExpressionWhereClause queryExpressionWhereClause, object data)
		{
			AddError(queryExpressionWhereClause, "QueryExpressionWhereClause is not supported.");
			return null;
		}
	}
}
