// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ICSharpCode.Profiler.Controller.Data.Linq
{
	class SortArgument
	{
		readonly LambdaExpression arg;
		readonly bool desc;
		
		public SortArgument(LambdaExpression arg, bool desc)
		{
			this.arg = arg;
			this.desc = desc;
		}
		
		public bool Descending {
			get { return desc; }
		}
		public LambdaExpression Argument {
			get { return arg; }
		}
		
		public override string ToString()
		{
			if (Descending)
				return Argument.ToString() + " DESC";
			else
				return Argument.ToString();
		}
	}
	
	sealed class Sort : QueryNode
	{
		ReadOnlyCollection<SortArgument> arguments;
		
		public Sort(QueryNode target, IList<SortArgument> args)
			: base(target)
		{
			this.arguments = new ReadOnlyCollection<SortArgument>(args);
		}
		
		public override SqlStatementKind BuildSql(StringBuilder b, SqlQueryContext context)
		{
			SqlStatementKind kind = Target.BuildSql(b, context);
			if (kind == SqlStatementKind.SelectOrderBy)
				WrapSqlIntoNestedStatement(b, context);
			
			b.Append(" ORDER BY ");
			
			for (int i = 0; i < arguments.Count; i++) {
				StringWriter w = new StringWriter();
				ExpressionSqlWriter writer = new ExpressionSqlWriter(w, context, arguments[i].Argument.Parameters[0]);
				writer.Write(arguments[i].Argument.Body);
				
				if (i == 0)
					b.Append(w.ToString());
				else
					b.Append(", " + w.ToString());
				
				if (arguments[i].Descending)
					b.Append(" DESC");
				else
					b.Append(" ASC");
			}
			
			return SqlStatementKind.SelectOrderBy;
		}
		
		protected override Expression VisitChildren(ExpressionVisitor visitor)
		{
			QueryNode newTarget = (QueryNode)visitor.Visit(Target);
			if (newTarget == Target)
				return this;
			else
				return new Sort(newTarget, arguments);
		}
		
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < arguments.Count; i++) {
				if (i > 0)
					builder.Append(", ");
				builder.Append(arguments[i].ToString());
			}
			return Target + ".Sort(" + builder.ToString() + ")";
		}
	}
}
