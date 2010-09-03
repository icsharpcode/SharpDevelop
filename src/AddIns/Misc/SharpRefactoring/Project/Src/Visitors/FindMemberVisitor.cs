// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
