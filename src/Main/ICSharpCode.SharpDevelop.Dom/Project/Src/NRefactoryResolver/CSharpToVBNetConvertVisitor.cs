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
	public class CSharpToVBNetConvertVisitor : CSharpConstructsVisitor
	{
		NRefactoryResolver _resolver;
		ParseInformation _parseInfo;
		
		public CSharpToVBNetConvertVisitor(IProjectContent pc, ParseInformation parseInfo)
		{
			_resolver = new NRefactoryResolver(LanguageProperties.CSharp);
			_parseInfo = parseInfo;
		}
		
		public override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			base.VisitCompilationUnit(compilationUnit, data);
			ToVBNetConvertVisitor v = new ToVBNetConvertVisitor();
			compilationUnit.AcceptVisitor(v, data);
			return null;
		}
		
		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			// Initialize resolver for method:
			if (!methodDeclaration.Body.IsNull) {
				if (_resolver.Initialize(_parseInfo, methodDeclaration.Body.StartLocation.Y, methodDeclaration.Body.StartLocation.X)) {
					_resolver.RunLookupTableVisitor(methodDeclaration);
				}
			}
			return base.VisitMethodDeclaration(methodDeclaration, data);
		}
		
		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			if (!constructorDeclaration.Body.IsNull) {
				if (_resolver.Initialize(_parseInfo, constructorDeclaration.Body.StartLocation.Y, constructorDeclaration.Body.StartLocation.X)) {
					_resolver.RunLookupTableVisitor(constructorDeclaration);
				}
			}
			return base.VisitConstructorDeclaration(constructorDeclaration, data);
		}
		
		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			if (_resolver.Initialize(_parseInfo, propertyDeclaration.BodyStart.Y, propertyDeclaration.BodyStart.X)) {
				_resolver.RunLookupTableVisitor(propertyDeclaration);
			}
			return base.VisitPropertyDeclaration(propertyDeclaration, data);
		}
		
		public override object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
		{
			if (_resolver.CompilationUnit == null)
				return base.VisitExpressionStatement(expressionStatement, data);
			
			// Transform event invocations that aren't already transformed by a parent IfStatement to RaiseEvent statement
			InvocationExpression eventInvocation = expressionStatement.Expression as InvocationExpression;
			if (eventInvocation != null && eventInvocation.TargetObject is IdentifierExpression) {
				MemberResolveResult mrr = _resolver.ResolveInternal(eventInvocation.TargetObject, ExpressionContext.Default) as MemberResolveResult;
				if (mrr != null && mrr.ResolvedMember is IEvent) {
					ReplaceCurrentNode(new RaiseEventStatement(
						((IdentifierExpression)eventInvocation.TargetObject).Identifier,
						eventInvocation.Arguments));
				}
			}
			return base.VisitExpressionStatement(expressionStatement, data);
		}
		
		public override object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			if (_resolver.CompilationUnit == null)
				return base.VisitBinaryOperatorExpression(binaryOperatorExpression, data);
			
			base.VisitBinaryOperatorExpression(binaryOperatorExpression, data);
			if (binaryOperatorExpression.Op == BinaryOperatorType.Equality || binaryOperatorExpression.Op == BinaryOperatorType.InEquality) {
				// maybe we have to convert Equality operator to ReferenceEquality
				ResolveResult left = _resolver.ResolveInternal(binaryOperatorExpression.Left, ExpressionContext.Default);
				ResolveResult right = _resolver.ResolveInternal(binaryOperatorExpression.Right, ExpressionContext.Default);
				if (left != null && right != null && left.ResolvedType != null && right.ResolvedType != null) {
					IClass cLeft = left.ResolvedType.GetUnderlyingClass();
					IClass cRight = right.ResolvedType.GetUnderlyingClass();
					if (cLeft != null && cRight != null) {
						if ((cLeft.ClassType != ClassType.Struct && cLeft.ClassType != ClassType.Enum)
						    || (cRight.ClassType != ClassType.Struct && cRight.ClassType != ClassType.Enum))
						{
							// this is a reference comparison
							if (cLeft.FullyQualifiedName != "System.String") {
								// and it's not a string comparison, so we'll use reference equality
								if (binaryOperatorExpression.Op == BinaryOperatorType.Equality) {
									binaryOperatorExpression.Op = BinaryOperatorType.ReferenceEquality;
								} else {
									binaryOperatorExpression.Op = BinaryOperatorType.ReferenceInequality;
								}
							}
						}
					}
				}
			}
			return null;
		}
	}
}
