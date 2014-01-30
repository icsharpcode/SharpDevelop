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
		Location rangeStart, rangeEnd;
		StringComparer comparer;
		
		public List<IdentifierExpression> Identifiers {
			get { return identifiers; }
		}
		
		public FindReferenceVisitor(StringComparer nameComparer, string name, Location rangeStart, Location rangeEnd)
		{
			this.identifiers = new List<IdentifierExpression>();
			this.name = name;
			this.rangeEnd = rangeEnd;
			this.rangeStart = rangeStart;
			this.comparer = nameComparer;
		}
		
		public override object VisitIdentifierExpression(ICSharpCode.NRefactory.Ast.IdentifierExpression identifierExpression, object data)
		{
			if (Compare(identifierExpression)) {
				identifiers.Add(identifierExpression);
			}
			return base.VisitIdentifierExpression(identifierExpression, data);
		}
		
		bool Compare(IdentifierExpression ie) {
			return this.comparer.Equals(ie.Identifier, this.name) &&
			        Inside(ie.StartLocation, ie.EndLocation);
		}
		
		bool IsIn(TypeReference type, List<TypeReference> list)
		{
			foreach (TypeReference tr in list) {
				if (tr.Type == type.Type)
					return true;
			}
			
			return false;
		}
		
		bool Inside(Location start, Location end)
		{
			return start >= this.rangeStart && end <= this.rangeEnd;
		}
	}
}
