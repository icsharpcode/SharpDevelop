// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	/// <summary>
	/// This class converts C# constructs to their VB.NET equivalents.
	/// </summary>
	public class VBNetToCSharpConvertVisitor : VBNetConstructsConvertVisitor
	{
		// Fixes identifier casing
		// Adds using statements for the default usings
		
		NRefactoryResolver _resolver;
		string _fileName;
		
		public VBNetToCSharpConvertVisitor(IProjectContent pc, string fileName)
		{
			_resolver = new NRefactoryResolver(pc, LanguageProperties.VBNet);
			_fileName = fileName;
		}
		
		public override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			base.VisitCompilationUnit(compilationUnit, data);
			ToCSharpConvertVisitor v = new ToCSharpConvertVisitor();
			compilationUnit.AcceptVisitor(v, data);
			if (_resolver.ProjectContent.DefaultImports != null) {
				int index = 0;
				foreach (string u in _resolver.ProjectContent.DefaultImports.Usings) {
					compilationUnit.Children.Insert(index++, new UsingDeclaration(u));
				}
			}
			return null;
		}
		
		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			// Initialize resolver for method:
			if (!methodDeclaration.Body.IsNull) {
				if (_resolver.Initialize(_fileName, methodDeclaration.Body.StartLocation.Line, methodDeclaration.Body.StartLocation.Column)) {
					_resolver.RunLookupTableVisitor(methodDeclaration);
				}
			}
			return base.VisitMethodDeclaration(methodDeclaration, data);
		}
		
		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			if (!constructorDeclaration.Body.IsNull) {
				if (_resolver.Initialize(_fileName, constructorDeclaration.Body.StartLocation.Line, constructorDeclaration.Body.StartLocation.Column)) {
					_resolver.RunLookupTableVisitor(constructorDeclaration);
				}
			}
			return base.VisitConstructorDeclaration(constructorDeclaration, data);
		}
		
		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			if (_resolver.Initialize(_fileName, propertyDeclaration.BodyStart.Line, propertyDeclaration.BodyStart.Column)) {
				_resolver.RunLookupTableVisitor(propertyDeclaration);
			}
			return base.VisitPropertyDeclaration(propertyDeclaration, data);
		}
		
		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			base.VisitIdentifierExpression(identifierExpression, data);
			if (_resolver.CompilationUnit == null)
				return null;
			
			ResolveResult rr = _resolver.ResolveInternal(identifierExpression, ExpressionContext.Default);
			string ident = GetIdentifierFromResult(rr);
			if (ident != null) {
				identifierExpression.Identifier = ident;
			}
			return null;
		}
		
		public override object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			base.VisitFieldReferenceExpression(fieldReferenceExpression, data);
			
			if (_resolver.CompilationUnit == null)
				return null;
			
			ResolveResult rr = _resolver.ResolveInternal(fieldReferenceExpression, ExpressionContext.Default);
			string ident = GetIdentifierFromResult(rr);
			if (ident != null) {
				fieldReferenceExpression.FieldName = ident;
			}
			if (rr is MethodResolveResult
			    && !(fieldReferenceExpression.Parent is AddressOfExpression)
			    && !(fieldReferenceExpression.Parent is InvocationExpression))
			{
				ReplaceCurrentNode(new InvocationExpression(fieldReferenceExpression));
			}
			
			return rr;
		}
		
		public override object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			base.VisitInvocationExpression(invocationExpression, data);
			
			if (_resolver.CompilationUnit == null)
				return null;
			
			if (invocationExpression.Arguments.Count > 0
			    && !(invocationExpression.Parent is ReDimStatement))
			{
				MemberResolveResult rr = _resolver.ResolveInternal(invocationExpression, ExpressionContext.Default) as MemberResolveResult;
				if (rr != null) {
					IProperty p = rr.ResolvedMember as IProperty;
					if (p != null) {
						ReplaceCurrentNode(new IndexerExpression(invocationExpression.TargetObject, invocationExpression.Arguments));
					}
				}
			}
			
			return null;
		}
		
		ClassFinder CreateContext()
		{
			return new ClassFinder(_resolver.CallingClass, _resolver.CallingMember, _resolver.CaretLine, _resolver.CaretColumn);
		}
		
		public override object VisitReDimStatement(ReDimStatement reDimStatement, object data)
		{
			base.VisitReDimStatement(reDimStatement, data);
			
			if (_resolver.CompilationUnit == null)
				return null;
			
			if (!reDimStatement.IsPreserve && reDimStatement.ReDimClauses.Count == 1) {
				// replace with array create expression
				
				ResolveResult rr = _resolver.ResolveInternal(reDimStatement.ReDimClauses[0].TargetObject, ExpressionContext.Default);
				if (rr != null && rr.ResolvedType != null && rr.ResolvedType.IsArrayReturnType) {
					ArrayCreateExpression ace = new ArrayCreateExpression(
						Refactoring.CodeGenerator.ConvertType(rr.ResolvedType, CreateContext())
					);
					foreach (Expression arg in reDimStatement.ReDimClauses[0].Arguments) {
						ace.Arguments.Add(Expression.AddInteger(arg, 1));
					}
					
					ReplaceCurrentNode(new ExpressionStatement(
						new AssignmentExpression(reDimStatement.ReDimClauses[0].TargetObject, AssignmentOperatorType.Assign, ace)));
				}
			}
			return null;
		}
		
		public override object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			FixTypeReferenceCasing(localVariableDeclaration.TypeReference, localVariableDeclaration.StartLocation);
			return base.VisitLocalVariableDeclaration(localVariableDeclaration, data);
		}
		
		public override object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			FixTypeReferenceCasing(variableDeclaration.TypeReference, variableDeclaration.StartLocation);
			return base.VisitVariableDeclaration(variableDeclaration, data);
		}
		
		void FixTypeReferenceCasing(TypeReference tr, Location loc)
		{
			if (_resolver.CompilationUnit == null) return;
			if (tr.IsNull) return;
			IReturnType rt = _resolver.SearchType(tr.SystemType, loc);
			if (rt != null) {
				IClass c = rt.GetUnderlyingClass();
				if (c != null) {
					if (string.Equals(tr.Type, c.Name, StringComparison.InvariantCultureIgnoreCase)) {
						tr.Type = c.Name;
					}
				}
			}
		}
		
		string GetIdentifierFromResult(ResolveResult rr)
		{
			LocalResolveResult lrr = rr as LocalResolveResult;
			if (lrr != null)
				return lrr.Field.Name;
			MemberResolveResult mrr = rr as MemberResolveResult;
			if (mrr != null)
				return mrr.ResolvedMember.Name;
			MethodResolveResult mtrr = rr as MethodResolveResult;
			if (mtrr != null)
				return mtrr.Name;
			TypeResolveResult trr = rr as TypeResolveResult;
			if (trr != null && trr.ResolvedClass != null)
				return trr.ResolvedClass.Name;
			return null;
		}
	}
}
