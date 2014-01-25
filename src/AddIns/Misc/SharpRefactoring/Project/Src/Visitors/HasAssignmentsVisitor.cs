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

using ICSharpCode.NRefactory;
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
		Location startRange, endRange;
		
		public bool HasAssignment {
			get { return hasAssignment; }
		}
		
		public HasAssignmentsVisitor(string name, TypeReference type, Location startRange, Location endRange)
		{
			this.name = name;
			this.type = type;
			this.startRange = startRange;
			this.endRange = endRange;
		}
		
		public override object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			if (!hasAssignment) {
				if (assignmentExpression.Left is IdentifierExpression) {
					hasAssignment = (((IdentifierExpression)assignmentExpression.Left).Identifier == name) &&
						(assignmentExpression.StartLocation >= startRange && assignmentExpression.EndLocation <= endRange);
				}
			}
			return base.VisitAssignmentExpression(assignmentExpression, data);
		}
	}
}
