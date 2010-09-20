// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;
using B = Boo.Lang.Compiler.Ast;

namespace NRefactoryToBooConverter
{
	partial class ConvertVisitor
	{
		public object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			for (int i = 0; i < fieldDeclaration.Fields.Count; i++) {
				ConvertField(fieldDeclaration.GetTypeForField(i), fieldDeclaration.Fields[i], fieldDeclaration);
			}
			return null;
		}
		
		public object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			throw new ApplicationException("Visited VariableDeclaration.");
		}
		
		void ConvertField(TypeReference typeRef, VariableDeclaration variable, FieldDeclaration fieldDeclaration)
		{
			B.TypeMember m;
			if (currentType is B.EnumDefinition) {
				if (variable.Initializer.IsNull) {
					m = new B.EnumMember(GetLexicalInfo(fieldDeclaration));
				} else {
					PrimitiveExpression p = variable.Initializer as PrimitiveExpression;
					if (p == null || !(p.Value is int)) {
						AddError(fieldDeclaration, "enum member initializer must be integer value");
						return;
					}
					m = new B.EnumMember(GetLexicalInfo(fieldDeclaration), new B.IntegerLiteralExpression((int)p.Value));
				}
			} else {
				m = new B.Field(GetLexicalInfo(fieldDeclaration), ConvertTypeReference(typeRef), ConvertExpression(variable.Initializer));
				m.Modifiers = ConvertModifier(fieldDeclaration, B.TypeMemberModifiers.Private);
			}
			m.Name = variable.Name;
			ConvertAttributes(fieldDeclaration.Attributes, m.Attributes);
			if (currentType != null) currentType.Members.Add(m);
		}
		
		B.Block ConvertMethodBlock(BlockStatement block)
		{
			B.Block b = ConvertBlock(block);
			RenameLocalsVisitor.RenameLocals(b, nameComparer);
			return b;
		}
		
		B.ExplicitMemberInfo ConvertInterfaceImplementations(List<InterfaceImplementation> implementations, AttributedNode node, B.TypeMember targetMember)
		{
			if (implementations.Count == 0)
				return null;
			if (implementations.Count > 1) {
				AddError(node, "Multiple explicit interface implementations are not supported");
			}
			if (implementations[0].MemberName != targetMember.Name) {
				AddError(node, "Explicit interface implementation: Implementing member with different name is not supported");
			}
			if (targetMember.Modifiers == B.TypeMemberModifiers.Private) {
				targetMember.Modifiers = B.TypeMemberModifiers.None;
			} else {
				AddError(node, "Explicit interface implementation: Only private methods can explicitly implement interfaces");
			}
			B.TypeReference tr = ConvertTypeReference(implementations[0].InterfaceType);
			if (tr is B.SimpleTypeReference) {
				B.ExplicitMemberInfo explicitInfo = new B.ExplicitMemberInfo(GetLexicalInfo(node));
				explicitInfo.InterfaceType = (B.SimpleTypeReference)tr;
				return explicitInfo;
			} else {
				AddError(node, "Explicit interface implementation: invalid base type, expecting SimpleTypeReference");
				return null;
			}
		}
		
		B.Method entryPointMethod;
		
		public object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			B.Method m = new B.Method(GetLexicalInfo(methodDeclaration));
			m.Name = methodDeclaration.Name;
			m.Modifiers = ConvertModifier(methodDeclaration, B.TypeMemberModifiers.Private);
			ConvertAttributes(methodDeclaration.Attributes, m.Attributes);
			if (currentType != null) currentType.Members.Add(m);
			if (methodDeclaration.HandlesClause.Count > 0) {
				// TODO: Convert handles clauses to [Handles] attribute
				AddError(methodDeclaration, "Handles-clause is not supported.");
			}
			m.ExplicitInfo = ConvertInterfaceImplementations(methodDeclaration.InterfaceImplementations, methodDeclaration, m);
			if (methodDeclaration.Templates.Count > 0) {
				AddError(methodDeclaration, "Declaring generic methods is not supported.");
			}
			ConvertParameters(methodDeclaration.Parameters, m.Parameters);
			m.EndSourceLocation = GetEndLocation((INode)methodDeclaration.Body ?? methodDeclaration);
			m.ReturnType = ConvertTypeReference(methodDeclaration.TypeReference);
			m.Body = ConvertMethodBlock(methodDeclaration.Body);
			if (m.Name == "Main" && m.IsStatic && m.Parameters.Count <= 1 &&
			    (methodDeclaration.TypeReference.Type == "System.Void" || methodDeclaration.TypeReference.Type == "System.Int32"))
			{
				entryPointMethod = m;
			}
			return m;
		}
		
		
		public object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			B.Constructor m = new B.Constructor(GetLexicalInfo(constructorDeclaration));
			m.Modifiers = ConvertModifier(constructorDeclaration, B.TypeMemberModifiers.Private);
			ConvertAttributes(constructorDeclaration.Attributes, m.Attributes);
			if (currentType != null) currentType.Members.Add(m);
			ConvertParameters(constructorDeclaration.Parameters, m.Parameters);
			m.EndSourceLocation = GetEndLocation((INode)constructorDeclaration.Body ?? constructorDeclaration);
			m.Body = ConvertMethodBlock(constructorDeclaration.Body);
			ConstructorInitializer ci = constructorDeclaration.ConstructorInitializer;
			if (ci != null && !ci.IsNull) {
				B.Expression initializerBase;
				if (ci.ConstructorInitializerType == ConstructorInitializerType.Base)
					initializerBase = new B.SuperLiteralExpression();
				else
					initializerBase = new B.SelfLiteralExpression();
				B.MethodInvocationExpression initializer = new B.MethodInvocationExpression(initializerBase);
				ConvertExpressions(ci.Arguments, initializer.Arguments);
				m.Body.Insert(0, new B.ExpressionStatement(initializer));
			}
			return m;
		}
		
		public object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			B.Destructor m = new B.Destructor(GetLexicalInfo(destructorDeclaration));
			ConvertAttributes(destructorDeclaration.Attributes, m.Attributes);
			if (currentType != null) currentType.Members.Add(m);
			m.EndSourceLocation = GetLocation(destructorDeclaration.EndLocation);
			m.Body = ConvertMethodBlock(destructorDeclaration.Body);
			return m;
		}
		
		void ConvertParameters(List<ParameterDeclarationExpression> input, B.ParameterDeclarationCollection output)
		{
			bool isParams = false;
			foreach (ParameterDeclarationExpression pde in input) {
				B.ParameterDeclaration para = ConvertParameter(pde, out isParams);
				if (para != null)
					output.Add(para);
			}
			output.HasParamArray = isParams;
		}
		
		B.ParameterDeclaration ConvertParameter(ParameterDeclarationExpression pde, out bool isParams)
		{
			B.ParameterDeclaration para = new B.ParameterDeclaration(pde.ParameterName, ConvertTypeReference(pde.TypeReference));
			if ((pde.ParamModifier & ParameterModifiers.Optional) != 0) {
				AddError(pde, "Optional parameters are not supported.");
			}
			if ((pde.ParamModifier & ParameterModifiers.Out) != 0) {
				para.Modifiers |= B.ParameterModifiers.Ref;
			}
			if ((pde.ParamModifier & ParameterModifiers.Ref) != 0) {
				para.Modifiers |= B.ParameterModifiers.Ref;
			}
			isParams = (pde.ParamModifier & ParameterModifiers.Params) != 0;
			ConvertAttributes(pde.Attributes, para.Attributes);
			return para;
		}
		
		public object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			bool tmp;
			return ConvertParameter(parameterDeclarationExpression, out tmp);
		}
		
		public object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			B.Property m = new B.Property(GetLexicalInfo(propertyDeclaration));
			m.Name = propertyDeclaration.Name;
			m.Modifiers = ConvertModifier(propertyDeclaration, B.TypeMemberModifiers.Private);
			ConvertAttributes(propertyDeclaration.Attributes, m.Attributes);
			if (currentType != null) currentType.Members.Add(m);
			ConvertParameters(propertyDeclaration.Parameters, m.Parameters);
			if (propertyDeclaration.IsIndexer) m.Name = "self";
			m.EndSourceLocation = GetLocation(propertyDeclaration.EndLocation);
			m.Type = ConvertTypeReference(propertyDeclaration.TypeReference);
			m.ExplicitInfo = ConvertInterfaceImplementations(propertyDeclaration.InterfaceImplementations, propertyDeclaration, m);
			if (!propertyDeclaration.IsWriteOnly) {
				m.Getter = new B.Method(GetLexicalInfo(propertyDeclaration.GetRegion));
				if (propertyDeclaration.GetRegion != null) {
					ConvertAttributes(propertyDeclaration.GetRegion.Attributes, m.Getter.Attributes);
					m.Getter.Modifiers = ConvertModifier(propertyDeclaration.GetRegion, B.TypeMemberModifiers.None);
					m.Getter.Body = ConvertMethodBlock(propertyDeclaration.GetRegion.Block);
					m.Getter.ReturnType = m.Type;
				}
			}
			if (!propertyDeclaration.IsReadOnly) {
				m.Setter = new B.Method(GetLexicalInfo(propertyDeclaration.SetRegion));
				if (propertyDeclaration.SetRegion != null) {
					ConvertAttributes(propertyDeclaration.SetRegion.Attributes, m.Setter.Attributes);
					m.Setter.Modifiers = ConvertModifier(propertyDeclaration.SetRegion, B.TypeMemberModifiers.None);
					m.Setter.Body = ConvertMethodBlock(propertyDeclaration.SetRegion.Block);
				}
			}
			return m;
		}
		
		public object VisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data)
		{
			throw new ApplicationException("PropertyGetRegion visited.");
		}
		
		public object VisitPropertySetRegion(PropertySetRegion propertySetRegion, object data)
		{
			throw new ApplicationException("PropertySetRegion visited.");
		}
		
		public object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			B.Event m = new B.Event(GetLexicalInfo(eventDeclaration));
			m.Name = eventDeclaration.Name;
			m.Modifiers = ConvertModifier(eventDeclaration, B.TypeMemberModifiers.Private);
			ConvertAttributes(eventDeclaration.Attributes, m.Attributes);
			if (currentType != null) currentType.Members.Add(m);
			m.EndSourceLocation = GetLocation(eventDeclaration.EndLocation);
			m.Type = ConvertTypeReference(eventDeclaration.TypeReference);
			if (eventDeclaration.InterfaceImplementations.Count > 0) {
				AddError(eventDeclaration, "Explicit interface implementation is not supported for events.");
			}
			if (eventDeclaration.Parameters.Count > 0) {
				AddError(eventDeclaration, "Events with parameters are not supported.");
			}
			if (eventDeclaration.HasAddRegion) {
				m.Add = new B.Method(GetLexicalInfo(eventDeclaration.AddRegion));
				ConvertAttributes(eventDeclaration.AddRegion.Attributes, m.Add.Attributes);
				m.Modifiers = ConvertModifier(eventDeclaration.AddRegion, m.Visibility);
				m.Add.Body = ConvertMethodBlock(eventDeclaration.AddRegion.Block);
			}
			if (eventDeclaration.HasRemoveRegion) {
				m.Remove = new B.Method(GetLexicalInfo(eventDeclaration.RemoveRegion));
				ConvertAttributes(eventDeclaration.RemoveRegion.Attributes, m.Remove.Attributes);
				m.Modifiers = ConvertModifier(eventDeclaration.RemoveRegion, m.Visibility);
				m.Remove.Body = ConvertMethodBlock(eventDeclaration.RemoveRegion.Block);
			}
			if (eventDeclaration.HasRaiseRegion) {
				m.Raise = new B.Method(GetLexicalInfo(eventDeclaration.RaiseRegion));
				ConvertAttributes(eventDeclaration.RaiseRegion.Attributes, m.Raise.Attributes);
				m.Modifiers = ConvertModifier(eventDeclaration.RaiseRegion, m.Visibility);
				m.Raise.Body = ConvertMethodBlock(eventDeclaration.RaiseRegion.Block);
			}
			return m;
		}
		
		public object VisitEventAddRegion(EventAddRegion eventAddRegion, object data)
		{
			throw new ApplicationException("EventAddRegion visited.");
		}
		
		public object VisitEventRemoveRegion(EventRemoveRegion eventRemoveRegion, object data)
		{
			throw new ApplicationException("EventRemoveRegion visited.");
		}
		
		public object VisitEventRaiseRegion(EventRaiseRegion eventRaiseRegion, object data)
		{
			throw new ApplicationException("EventRaiseRegion visited.");
		}
		
		public object VisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
		{
			throw new ApplicationException("ConstructorInitializer visited.");
		}
		
		public object VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
		{
			AddError(operatorDeclaration, "Declaring operators is not supported (BOO-223).");
			return null;
		}
		
		public object VisitDeclareDeclaration(DeclareDeclaration declareDeclaration, object data)
		{
			throw new NotImplementedException();
		}
	}
}
