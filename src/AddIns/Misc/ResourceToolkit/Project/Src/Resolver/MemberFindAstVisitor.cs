// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop.Dom;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Finds a certain member inside the AST.
	/// </summary>
	public class MemberFindAstVisitor : AbstractAstVisitor
	{
		readonly IMember memberToFind;
		INode memberNode;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MemberFindAstVisitor"/> class.
		/// </summary>
		/// <param name="member">The member to find.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String,System.String)")]
		public MemberFindAstVisitor(IMember member)
		{
			if (member == null) {
				throw new ArgumentNullException("member");
			}
			if (member.Region.IsEmpty) {
				throw new ArgumentException("Cannot find this member because its region is empty."+Environment.NewLine+"member: '"+member.ToString()+"'", "member");
			}
			this.memberToFind = member;
		}
		
		/// <summary>
		/// Gets the INode that belongs to the member specified in the constructor call,
		/// or a null reference, if the member cannot be found.
		/// </summary>
		public INode MemberNode {
			get {
				return this.memberNode;
			}
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Tests whether the specified node is the node belonging to the member we are
		/// looking for.
		/// </summary>
		/// <returns><c>true</c>, if this is the right node or the node has already been found before, otherwise <c>false</c>.</returns>
		bool CheckNode(INode node)
		{
			if (this.memberNode != null) {
				return true;
			}
			if (!node.StartLocation.IsEmpty &&
			    node.StartLocation.Y == this.memberToFind.Region.BeginLine &&
			    node.StartLocation.X == this.memberToFind.Region.BeginColumn) {
				this.memberNode = node;
				return true;
			}
			return false;
		}
		
		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			if (this.CheckNode(constructorDeclaration)) {
				return null;
			}
			return base.VisitConstructorDeclaration(constructorDeclaration, data);
		}
		
		public override object VisitDeclareDeclaration(DeclareDeclaration declareDeclaration, object data)
		{
			if (this.CheckNode(declareDeclaration)) {
				return null;
			}
			return base.VisitDeclareDeclaration(declareDeclaration, data);
		}
		
		public override object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			if (this.CheckNode(delegateDeclaration)) {
				return null;
			}
			return base.VisitDelegateDeclaration(delegateDeclaration, data);
		}
		
		public override object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			if (this.CheckNode(destructorDeclaration)) {
				return null;
			}
			return base.VisitDestructorDeclaration(destructorDeclaration, data);
		}
		
		public override object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			if (this.CheckNode(eventDeclaration)) {
				return null;
			}
			return base.VisitEventDeclaration(eventDeclaration, data);
		}
		
		public override object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			if (this.CheckNode(fieldDeclaration)) {
				return null;
			}
			return base.VisitFieldDeclaration(fieldDeclaration, data);
		}
		
		public override object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			if (this.CheckNode(localVariableDeclaration)) {
				return null;
			}
			return base.VisitLocalVariableDeclaration(localVariableDeclaration, data);
		}
		
		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			if (this.CheckNode(methodDeclaration)) {
				return null;
			}
			return base.VisitMethodDeclaration(methodDeclaration, data);
		}
		
		public override object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			if (this.CheckNode(namespaceDeclaration)) {
				return null;
			}
			return base.VisitNamespaceDeclaration(namespaceDeclaration, data);
		}
		
		public override object VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
		{
			if (this.CheckNode(operatorDeclaration)) {
				return null;
			}
			return base.VisitOperatorDeclaration(operatorDeclaration, data);
		}
		
		public override object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			if (this.CheckNode(parameterDeclarationExpression)) {
				return null;
			}
			return base.VisitParameterDeclarationExpression(parameterDeclarationExpression, data);
		}
		
		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			if (this.CheckNode(propertyDeclaration)) {
				return null;
			}
			return base.VisitPropertyDeclaration(propertyDeclaration, data);
		}
		
		public override object VisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data)
		{
			if (this.CheckNode(propertyGetRegion)) {
				return null;
			}
			return base.VisitPropertyGetRegion(propertyGetRegion, data);
		}
		
		public override object VisitPropertySetRegion(PropertySetRegion propertySetRegion, object data)
		{
			if (this.CheckNode(propertySetRegion)) {
				return null;
			}
			return base.VisitPropertySetRegion(propertySetRegion, data);
		}
		
		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (this.CheckNode(typeDeclaration)) {
				return null;
			}
			return base.VisitTypeDeclaration(typeDeclaration, data);
		}
		
		public override object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			if (this.CheckNode(variableDeclaration)) {
				return null;
			}
			return base.VisitVariableDeclaration(variableDeclaration, data);
		}
	}
}
