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
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Finds connections between fields and properties.
	/// TODO: This currently only works if the field and the property are declared in the same source file.
	/// </summary>
	public class PropertyFieldAssociationVisitor : PositionTrackingAstVisitor
	{
		IMember memberToFind;
		IMember associatedMember;
		
		/// <summary>
		/// Gets the field that has been found to be associated with the property specified at the constructor call.
		/// </summary>
		public IField AssociatedField {
			get {
				return this.associatedMember as IField;
			}
		}
		
		/// <summary>
		/// Gets the property that has been found to be associated with the field specified at the constructor call.
		/// </summary>
		public IProperty AssociatedProperty {
			get {
				return this.associatedMember as IProperty;
			}
		}
		
		// ********************************************************************************************************************************
		
		private enum VisitorContext
		{
			Default,
			PropertyGetRegion,
			PropertySetRegion
		}
		
		private VisitorContext currentContext = VisitorContext.Default;
		
		protected override void BeginVisit(INode node)
		{
			base.BeginVisit(node);
			if (node is PropertyGetRegion) {
				this.currentContext = VisitorContext.PropertyGetRegion;
			} else if (node is PropertySetRegion) {
				this.currentContext = VisitorContext.PropertySetRegion;
			}
		}
		
		protected override void EndVisit(INode node)
		{
			if (node is PropertyGetRegion || node is PropertySetRegion) {
				this.currentContext = VisitorContext.Default;
			}
			base.EndVisit(node);
		}
		
		public override object TrackedVisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			
			if (this.memberToFind is IProperty) {
				
				// If we are looking for a specified property:
				// find out if this is the one by comparing the location.
				if (propertyDeclaration.StartLocation.X == this.memberToFind.Region.BeginColumn &&
				    propertyDeclaration.StartLocation.Y == this.memberToFind.Region.BeginLine) {
					data = true;
				}
				
			} else if (this.memberToFind is IField) {
				
				// If we are looking for a specifield field:
				// store the property info for future reference.
				data = propertyDeclaration;
				
			}
			
			return base.TrackedVisitPropertyDeclaration(propertyDeclaration, data);
		}
		
		public override object TrackedVisitReturnStatement(ReturnStatement returnStatement, object data)
		{
			// If we are in a property get region,
			// this may be the statement where the field value is returned.
			if (this.associatedMember == null &&	// skip if already found to improve performance
			    this.currentContext == VisitorContext.PropertyGetRegion && data != null) {
				
				// Fix some type casting and parenthesized expressions
				Expression expr = returnStatement.Expression;
				while (true) {
					CastExpression ce = expr as CastExpression;
					if (ce != null) {
						expr = ce.Expression;
						continue;
					}
					ParenthesizedExpression pe = expr as ParenthesizedExpression;
					if (pe != null) {
						expr = pe.Expression;
						continue;
					}
					break;
				}
				
				// Resolve the expression.
				if (!FileUtility.IsEqualFileName(this.FileName, this.memberToFind.DeclaringType.CompilationUnit.FileName)) {
					throw new InvalidOperationException("The PropertyFieldAssociationVisitor does currently not support the case that the field is declared in a different file than the property.");
				}
				MemberResolveResult mrr = this.Resolve(expr) as MemberResolveResult;
				if (mrr != null && mrr.ResolvedMember is IField) {
					
					PropertyDeclaration pd;
					
					#if DEBUG
					LoggingService.Debug("ResourceToolkit: PropertyFieldAssociationVisitor, inside PropertyGetRegion, resolved field: "+mrr.ResolvedMember.ToString());
					#endif
					
					if (data as bool? ?? false) {
						
						// We are looking for this property.
						#if DEBUG
						LoggingService.Debug("ResourceToolkit: PropertyFieldAssociationVisitor, inside PropertyGetRegion, this property seems to reference field "+mrr.ResolvedMember.ToString());
						#endif
						this.associatedMember = mrr.ResolvedMember;
						
					} else if ((pd = (data as PropertyDeclaration)) != null) {
						
						// We are looking for the field in this.memberToFind.
						if (this.memberToFind.CompareTo(mrr.ResolvedMember) == 0) {
							
							// Resolve the property.
							MemberResolveResult prr = NRefactoryAstCacheService.ResolveLowLevel(this.FileName, this.FileContent, pd.StartLocation.Y, pd.StartLocation.X+1, null, pd.Name, ExpressionContext.Default) as MemberResolveResult;
							if (prr != null) {
								
								#if DEBUG
								LoggingService.Debug("ResourceToolkit: PropertyFieldAssociationVisitor, inside PropertyGetRegion, resolved property: "+prr.ResolvedMember.ToString());
								#endif
								
								if (prr.ResolvedMember is IProperty) {
									#if DEBUG
									LoggingService.Debug("ResourceToolkit: PropertyFieldAssociationVisitor, inside PropertyGetRegion, property "+prr.ResolvedMember.ToString()+" seems to reference field "+mrr.ResolvedMember.ToString());
									#endif
									this.associatedMember = prr.ResolvedMember;
								}
								
							}
							
						}
						
					}
					
				}
				
			}
			
			return base.TrackedVisitReturnStatement(returnStatement, data);
		}
		
		public override object TrackedVisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			// If we are in a property set region,
			// this may be the statement where the field value is assigned.
			if (this.associatedMember == null &&	// skip if already found to improve performance
			    this.currentContext == VisitorContext.PropertySetRegion &&
			    assignmentExpression.Op == AssignmentOperatorType.Assign && data != null) {
				
				// Resolve the expression.
				if (!FileUtility.IsEqualFileName(this.FileName, this.memberToFind.DeclaringType.CompilationUnit.FileName)) {
					throw new InvalidOperationException("The PropertyFieldAssociationVisitor does currently not support the case that the field is declared in a different file than the property.");
				}
				MemberResolveResult mrr = this.Resolve(assignmentExpression.Left) as MemberResolveResult;
				if (mrr != null && mrr.ResolvedMember is IField && !((IField)mrr.ResolvedMember).IsLocalVariable) {
					
					PropertyDeclaration pd;
					
					#if DEBUG
					LoggingService.Debug("ResourceToolkit: PropertyFieldAssociationVisitor, inside PropertySetRegion, resolved field: "+mrr.ResolvedMember.ToString());
					#endif
					
					if (data as bool? ?? false) {
						
						// We are looking for this property.
						#if DEBUG
						LoggingService.Debug("ResourceToolkit: PropertyFieldAssociationVisitor, inside PropertySetRegion, this property seems to reference field "+mrr.ResolvedMember.ToString());
						#endif
						this.associatedMember = mrr.ResolvedMember;
						
					} else if ((pd = (data as PropertyDeclaration)) != null) {
						
						// We are looking for the field in this.memberToFind.
						if (this.memberToFind.CompareTo(mrr.ResolvedMember) == 0) {
							
							// Resolve the property.
							MemberResolveResult prr = NRefactoryAstCacheService.ResolveLowLevel(this.FileName, this.FileContent, pd.StartLocation.Y, pd.StartLocation.X+1, null, pd.Name, ExpressionContext.Default) as MemberResolveResult;
							if (prr != null) {
								
								#if DEBUG
								LoggingService.Debug("ResourceToolkit: PropertyFieldAssociationVisitor, inside PropertySetRegion, resolved property: "+prr.ResolvedMember.ToString());
								#endif
								
								if (prr.ResolvedMember is IProperty) {
									#if DEBUG
									LoggingService.Debug("ResourceToolkit: PropertyFieldAssociationVisitor, inside PropertySetRegion, property "+prr.ResolvedMember.ToString()+" seems to reference field "+mrr.ResolvedMember.ToString());
									#endif
									this.associatedMember = prr.ResolvedMember;
								}
								
							}
							
						}
						
					}
					
				}
				
			}
			
			return base.TrackedVisitAssignmentExpression(assignmentExpression, data);
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyFieldAssociationVisitor"/> class.
		/// </summary>
		/// <param name="property">The property to find the associated field for.</param>
		public PropertyFieldAssociationVisitor(IProperty property, string fileName, string fileContent)
			: base(fileName, fileContent)
		{
			if (property == null) {
				throw new ArgumentNullException("property");
			}
			this.memberToFind = property;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyFieldAssociationVisitor"/> class.
		/// </summary>
		/// <param name="field">The field to find the associated property for.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String,System.String)")]
		public PropertyFieldAssociationVisitor(IField field, string fileName, string fileContent)
			: base(fileName, fileContent)
		{
			if (field == null) {
				throw new ArgumentNullException("field");
			} else if (field.IsLocalVariable) {
				throw new ArgumentException("The specified IField must not be a local variable.", "field");
			}
			this.memberToFind = field;
		}
		
	}
}
