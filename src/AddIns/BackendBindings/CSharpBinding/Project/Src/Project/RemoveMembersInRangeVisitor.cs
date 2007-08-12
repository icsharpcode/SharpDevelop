// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2644$</version>
// </file>

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop.Dom;

namespace CSharpBinding
{
	/// <summary>
	/// Removes members that are in the specified range.
	/// </summary>
	sealed class RemoveMembersInRangeVisitor : AbstractAstTransformer
	{
		DomRegion region;
		
		public RemoveMembersInRangeVisitor(DomRegion region)
		{
			this.region = region;
		}
		
		bool RemoveIfInRange(INode node)
		{
			if (node.StartLocation.IsEmpty)
				return false;
			if (region.IsInside(node.StartLocation.Line, node.StartLocation.Column)) {
				if (node.EndLocation.IsEmpty || region.IsInside(node.EndLocation.Line, node.EndLocation.Column)) {
					RemoveCurrentNode();
					return true;
				}
			}
			return false;
		}
		
		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			return RemoveIfInRange(propertyDeclaration) ? null : base.VisitPropertyDeclaration(propertyDeclaration, data);
		}
		
		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			return RemoveIfInRange(methodDeclaration) ? null : base.VisitMethodDeclaration(methodDeclaration, data);
		}
		
		public override object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			return RemoveIfInRange(fieldDeclaration) ? null : base.VisitFieldDeclaration(fieldDeclaration, data);
		}
		
		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			return RemoveIfInRange(typeDeclaration) ? null : base.VisitTypeDeclaration(typeDeclaration, data);
		}
	}
}
