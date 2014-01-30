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
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ICSharpCode.Profiler.Controller.Data.Linq
{
	/// <summary>
	/// Query node that limits the amount of data selected. Produces LIMIT in SQL.
	/// </summary>
	sealed class Limit : QueryNode
	{
		public int Start { get; private set; }
		public int Length { get; private set; }
		
		public Limit(QueryNode target, int start, int length)
			: base(target)
		{
			this.Start = start;
			this.Length = length;
		}
		
		protected override Expression VisitChildren(ExpressionVisitor visitor)
		{
			QueryNode newTarget = (QueryNode)visitor.Visit(Target);
			if (newTarget == Target)
				return this;
			else
				return new Limit(newTarget, Start, Length);
		}
		
		public override string ToString()
		{
			return Target + ".Limit(" + Start + "," + Length + ")";
		}
		
		public override SqlStatementKind BuildSql(StringBuilder b, SqlQueryContext context)
		{
			SqlStatementKind kind = Target.BuildSql(b, context);
			if (kind == SqlStatementKind.SelectLimit)
				WrapSqlIntoNestedStatement(b, context);
			
			b.Append(" LIMIT " + Length);
			b.AppendLine(" OFFSET " + Start);
			
			return SqlStatementKind.SelectLimit;
		}
	}
}
