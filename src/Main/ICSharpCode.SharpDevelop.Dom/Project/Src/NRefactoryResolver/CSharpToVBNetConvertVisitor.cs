// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
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
		IProjectContent _pc;
		
		public CSharpToVBNetConvertVisitor(IProjectContent pc, ParseInformation parseInfo)
		{
			_resolver = new NRefactoryResolver(LanguageProperties.CSharp);
			_pc = pc;
			_parseInfo = parseInfo;
		}
		
		IReturnType ResolveType(TypeReference typeRef)
		{
			return TypeVisitor.CreateReturnType(typeRef, _resolver);
		}
		
		public override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			base.VisitCompilationUnit(compilationUnit, data);
			ToVBNetConvertVisitor v = new ToVBNetConvertVisitor();
			compilationUnit.AcceptVisitor(v, data);
			return null;
		}
		
		struct BaseType
		{
			internal readonly TypeReference TypeReference;
			internal readonly IReturnType ReturnType;
			internal readonly IClass UnderlyingClass;
			
			public BaseType(TypeReference typeReference, IReturnType returnType)
			{
				this.TypeReference = typeReference;
				this.ReturnType = returnType;
				this.UnderlyingClass = returnType.GetUnderlyingClass();
			}
		}
		
		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			// Initialize resolver for method:
			if (!methodDeclaration.Body.IsNull) {
				if (_resolver.Initialize(_parseInfo, methodDeclaration.Body.StartLocation.Y, methodDeclaration.Body.StartLocation.X)) {
					_resolver.RunLookupTableVisitor(methodDeclaration);
				}
			}
			IMethod currentMethod = _resolver.CallingMember as IMethod;
			CreateInterfaceImplementations(currentMethod, methodDeclaration, methodDeclaration.InterfaceImplementations);
			return base.VisitMethodDeclaration(methodDeclaration, data);
		}
		
		ClassFinder CreateContext()
		{
			return new ClassFinder(_resolver.CallingClass, _resolver.CallingMember, _resolver.CaretLine, _resolver.CaretColumn);
		}
		
		void CreateInterfaceImplementations(IMember currentMember, ParametrizedNode memberDecl, List<InterfaceImplementation> interfaceImplementations)
		{
			if (currentMember != null && interfaceImplementations.Count == 1) {
				// member is explicitly implementing an interface member
				// to convert explicit interface implementations to VB, make the member private
				// and ensure its name does not collide with another member
				memberDecl.Modifier |= Modifiers.Private;
				memberDecl.Name = interfaceImplementations[0].InterfaceType.Type.Replace('.', '_') + "_" + memberDecl.Name;
			}
			
			if (currentMember != null && currentMember.IsPublic
			    && currentMember.DeclaringType.ClassType != ClassType.Interface)
			{
				// member could be implicitly implementing an interface member,
				// search for interfaces containing the member
				foreach (IReturnType directBaseType in currentMember.DeclaringType.GetCompoundClass().BaseTypes) {
					IClass directBaseClass = directBaseType.GetUnderlyingClass();
					if (directBaseClass != null && directBaseClass.ClassType == ClassType.Interface) {
						// include members inherited from other interfaces in the search:
						foreach (IReturnType baseType in MemberLookupHelper.GetTypeInheritanceTree(directBaseType)) {
							IClass baseClass = baseType.GetUnderlyingClass();
							if (baseClass != null && baseClass.ClassType == ClassType.Interface) {
								IMember similarMember = MemberLookupHelper.FindSimilarMember(baseClass, currentMember);
								// add an interface implementation for similarMember
								// only when similarMember is not explicitly implemented by another member in this class
								if (similarMember != null && !HasExplicitImplementationFor(similarMember, baseType, memberDecl.Parent)) {
									interfaceImplementations.Add(new InterfaceImplementation(
										Refactoring.CodeGenerator.ConvertType(baseType, CreateContext()),
										currentMember.Name));
								}
							}
						}
					}
				}
			}
		}
		
		bool HasExplicitImplementationFor(IMember interfaceMember, IReturnType interfaceReference, INode typeDecl)
		{
			if (typeDecl == null)
				return false;
			foreach (INode node in typeDecl.Children) {
				MemberNode memberNode = node as MemberNode;
				if (memberNode != null && memberNode.InterfaceImplementations.Count > 0) {
					foreach (InterfaceImplementation impl in memberNode.InterfaceImplementations) {
						if (impl.MemberName == interfaceMember.Name
						    && object.Equals(ResolveType(impl.InterfaceType), interfaceReference)) {
							return true;
						}
					}
				}
			}
			return false;
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
			IProperty currentProperty = _resolver.CallingMember as IProperty;
			CreateInterfaceImplementations(currentProperty, propertyDeclaration, propertyDeclaration.InterfaceImplementations);
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
