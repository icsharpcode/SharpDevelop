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
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace SharpRefactoring.Visitors
{
	public class FindMemberVisitor : AbstractAstVisitor
	{
		Location start, end;
		AttributedNode member = null;
		
		public AttributedNode Member {
			get { return member; }
		}
		
		public FindMemberVisitor(Location start, Location end)
		{
			this.start = start;
			this.end = end;
		}
		
		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			if ((methodDeclaration.Body.StartLocation < start) &&
			    (methodDeclaration.Body.EndLocation > end)) {
				this.member = methodDeclaration;
			}
			
			return base.VisitMethodDeclaration(methodDeclaration, data);
		}
		
		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			if ((propertyDeclaration.BodyStart < start) &&
			    (propertyDeclaration.BodyEnd > end)) {
				this.member = propertyDeclaration;
			}
			return base.VisitPropertyDeclaration(propertyDeclaration, data);
		}
		
		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			if ((constructorDeclaration.Body.StartLocation < start) &&
			    (constructorDeclaration.Body.EndLocation > end)) {
				this.member = constructorDeclaration;
			}
			
			return base.VisitConstructorDeclaration(constructorDeclaration, data);
		}
		
		public override object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			if ((destructorDeclaration.Body.StartLocation < start) &&
			    (destructorDeclaration.Body.EndLocation > end)) {
				this.member = destructorDeclaration;
			}
			
			return base.VisitDestructorDeclaration(destructorDeclaration, data);
		}
		
		public override object VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
		{
			if ((operatorDeclaration.Body.StartLocation < start) &&
			    (operatorDeclaration.Body.EndLocation > end)) {
				this.member = operatorDeclaration;
			}
			
			return base.VisitOperatorDeclaration(operatorDeclaration, data);
		}
	}
}
