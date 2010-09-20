// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public object VisitQueryExpressionVB(QueryExpressionVB queryExpressionVB, object data)
		{
			AddError(queryExpressionVB, "QueryExpressionVB is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionFromClause(QueryExpressionFromClause queryExpressionFromClause, object data)
		{
			AddError(queryExpressionFromClause, "QueryExpressionFromClause is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionGroupClause(QueryExpressionGroupClause queryExpressionGroupClause, object data)
		{
			AddError(queryExpressionGroupClause, "QueryExpressionGroupClause is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionOrderClause(QueryExpressionOrderClause queryExpressionOrderClause, object data)
		{
			AddError(queryExpressionOrderClause, "QueryExpressionOrderClause is not supported.");
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
		
		public object VisitQueryExpressionJoinClause(QueryExpressionJoinClause queryExpressionJoinClause, object data)
		{
			AddError(queryExpressionJoinClause, "QueryExpressionJoinClause is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionLetClause(QueryExpressionLetClause queryExpressionLetClause, object data)
		{
			AddError(queryExpressionLetClause, "QueryExpressionLetClause is not supported.");
			return null;
		}
		
		public object VisitExpressionRangeVariable(ExpressionRangeVariable expressionRangeVariable, object data)
		{
			AddError(expressionRangeVariable, "ExpressionRangeVariable is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionAggregateClause(QueryExpressionAggregateClause queryExpressionAggregateClause, object data)
		{
			AddError(queryExpressionAggregateClause, "QueryExpressionAggregateClause is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionDistinctClause(QueryExpressionDistinctClause queryExpressionDistinctClause, object data)
		{
			AddError(queryExpressionDistinctClause, "QueryExpressionDistinctClause is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionGroupJoinVBClause(QueryExpressionGroupJoinVBClause queryExpressionGroupJoinVBClause, object data)
		{
			AddError(queryExpressionGroupJoinVBClause, "QueryExpressionGroupJoinVBClause is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionGroupVBClause(QueryExpressionGroupVBClause queryExpressionGroupVBClause, object data)
		{
			AddError(queryExpressionGroupVBClause, "QueryExpressionGroupVBClause is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionJoinConditionVB(QueryExpressionJoinConditionVB queryExpressionJoinConditionVB, object data)
		{
			AddError(queryExpressionJoinConditionVB, "QueryExpressionJoinConditionVB is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionJoinVBClause(QueryExpressionJoinVBClause queryExpressionJoinVBClause, object data)
		{
			AddError(queryExpressionJoinVBClause, "QueryExpressionJoinVBClause is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionLetVBClause(QueryExpressionLetVBClause queryExpressionLetVBClause, object data)
		{
			AddError(queryExpressionLetVBClause, "QueryExpressionLetVBClause is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionPartitionVBClause(QueryExpressionPartitionVBClause queryExpressionPartitionVBClause, object data)
		{
			AddError(queryExpressionPartitionVBClause, "QueryExpressionPartitionVBClause is not supported.");
			return null;
		}
		
		public object VisitQueryExpressionSelectVBClause(QueryExpressionSelectVBClause queryExpressionSelectVBClause, object data)
		{
			AddError(queryExpressionSelectVBClause, "QueryExpressionSelectVBClause is not supported.");
			return null;
		}
	}
}
