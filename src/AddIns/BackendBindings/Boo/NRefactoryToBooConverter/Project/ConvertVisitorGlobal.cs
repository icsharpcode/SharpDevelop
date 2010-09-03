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
		public object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			module = new B.Module();
			module.LexicalInfo = new B.LexicalInfo(fileName, 1, 1);
			compilationUnit.AcceptChildren(this, data);
			if (entryPointMethod != null) {
				bool allMembersAreStatic = true;
				foreach (B.TypeMember member in entryPointMethod.DeclaringType.Members) {
					allMembersAreStatic &= member.IsStatic;
				}
				if (allMembersAreStatic) {
					entryPointMethod.DeclaringType.Attributes.Add(MakeAttribute(("module")));
				} else {
					lastLexicalInfo = entryPointMethod.LexicalInfo;
					B.Expression expr = MakeReferenceExpression(entryPointMethod.DeclaringType.Name + ".Main");
					B.MethodInvocationExpression mie = new B.MethodInvocationExpression(lastLexicalInfo, expr);
					if (entryPointMethod.Parameters.Count > 0) {
						mie.Arguments.Add(MakeReferenceExpression("argv"));
					}
					B.SimpleTypeReference ret = entryPointMethod.ReturnType as B.SimpleTypeReference;
					if (ret.Name == "void" || ret.Name == "System.Void")
						module.Globals.Add(new B.ExpressionStatement(mie));
					else
						module.Globals.Add(new B.ReturnStatement(lastLexicalInfo, mie, null));
				}
			}
			B.Module tmp = module;
			module = null;
			return tmp;
		}
		
		public object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			if (module.Namespace != null) {
				AddError(namespaceDeclaration, "Only one namespace declaration per file is supported.");
				return null;
			}
			module.Namespace = new B.NamespaceDeclaration(GetLexicalInfo(namespaceDeclaration));
			module.Namespace.Name = namespaceDeclaration.Name;
			return namespaceDeclaration.AcceptChildren(this, data);
		}
		
		public object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			foreach (Using u in usingDeclaration.Usings) {
				VisitUsing(u, data);
			}
			return null;
		}
		
		public object VisitUsing(Using @using, object data)
		{
			B.Import import;
			if (@using.IsAlias) {
				import = new B.Import(@using.Alias.Type, null, new B.ReferenceExpression(@using.Name));
				import.LexicalInfo = GetLexicalInfo(@using);
			} else {
				import = new B.Import(GetLexicalInfo(@using), @using.Name);
			}
			module.Imports.Add(import);
			return import;
		}
		
		B.TypeDefinition currentType;
		
		public object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (typeDeclaration.Templates.Count > 0) {
				AddError(typeDeclaration, "Generic type definitions are not supported.");
			}
			B.TypeDefinition oldType = currentType;
			B.TypeDefinition typeDef;
			switch (typeDeclaration.Type) {
				case ClassType.Class:
					typeDef = new B.ClassDefinition(GetLexicalInfo(typeDeclaration));
					break;
				case ClassType.Interface:
					typeDef = new B.InterfaceDefinition(GetLexicalInfo(typeDeclaration));
					break;
				case ClassType.Enum:
					typeDef = new B.EnumDefinition(GetLexicalInfo(typeDeclaration));
					break;
				case ClassType.Struct:
					typeDef = new B.StructDefinition(GetLexicalInfo(typeDeclaration));
					break;
				case ClassType.Module:
					typeDef = new B.ClassDefinition(GetLexicalInfo(typeDeclaration));
					typeDeclaration.Modifier |= Modifiers.Static;
					break;
				default:
					AddError(typeDeclaration, "Unknown class type.");
					return null;
			}
			if (currentType != null)
				typeDef.Modifiers = ConvertModifier(typeDeclaration, B.TypeMemberModifiers.Private);
			else
				typeDef.Modifiers = ConvertModifier(typeDeclaration, B.TypeMemberModifiers.Internal);
			typeDef.Name = typeDeclaration.Name;
			typeDef.EndSourceLocation = GetLocation(typeDeclaration.EndLocation);
			ConvertAttributes(typeDeclaration.Attributes, typeDef.Attributes);
			ConvertTypeReferences(typeDeclaration.BaseTypes, typeDef.BaseTypes);
			
			if (currentType != null)
				currentType.Members.Add(typeDef);
			else
				module.Members.Add(typeDef);
			currentType = typeDef;
			typeDeclaration.AcceptChildren(this, data);
			currentType = oldType;
			return typeDef;
		}
		
		public object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			B.CallableDefinition cd = new B.CallableDefinition(GetLexicalInfo(delegateDeclaration));
			cd.Name = delegateDeclaration.Name;
			ConvertAttributes(delegateDeclaration.Attributes, cd.Attributes);
			cd.Modifiers = ConvertModifier(delegateDeclaration, B.TypeMemberModifiers.Private);
			ConvertParameters(delegateDeclaration.Parameters, cd.Parameters);
			cd.ReturnType = ConvertTypeReference(delegateDeclaration.ReturnType);
			if (currentType != null)
				currentType.Members.Add(cd);
			else
				module.Members.Add(cd);
			return cd;
		}
		
		void ConvertAttributes(List<AttributeSection> sections, B.AttributeCollection col)
		{
			foreach (AttributeSection s in sections) {
				if (s.AttributeTarget.Length > 0) {
					AddError(s, "Attribute target not supported");
					continue;
				}
				foreach (ICSharpCode.NRefactory.Ast.Attribute a in s.Attributes) {
					col.Add((B.Attribute)VisitAttribute(a, null));
				}
			}
		}
		
		public object VisitAttribute(ICSharpCode.NRefactory.Ast.Attribute a, object data)
		{
			B.Attribute att = new B.Attribute(GetLexicalInfo(a), a.Name);
			att.EndSourceLocation = GetLocation(a.EndLocation);
			ConvertExpressions(a.PositionalArguments, att.Arguments);
			foreach (NamedArgumentExpression nae in a.NamedArguments) {
				B.Expression expr = ConvertExpression(nae.Expression);
				if (expr != null) {
					att.NamedArguments.Add(new B.ExpressionPair(new B.ReferenceExpression(nae.Name), expr));
				}
			}
			return att;
		}
		
		public object VisitAttributeSection(AttributeSection s, object data)
		{
			if (s.AttributeTarget.Equals("assembly", StringComparison.OrdinalIgnoreCase)) {
				foreach (ICSharpCode.NRefactory.Ast.Attribute a in s.Attributes) {
					module.AssemblyAttributes.Add((B.Attribute)VisitAttribute(a, null));
				}
			} else {
				AddError(s, "Attribute must have the target 'assembly'");
			}
			return null;
		}
		
		// Some classes are handled by their parent (TemplateDefinition by TypeDeclaration/MethodDeclaration etc.)
		// so we don't need to implement Visit for them.
		public object VisitTemplateDefinition(TemplateDefinition templateDefinition, object data)
		{
			throw new ApplicationException("Visited TemplateDefinition.");
		}
		
		public object VisitInterfaceImplementation(InterfaceImplementation interfaceImplementation, object data)
		{
			throw new ApplicationException("Visited InterfaceImplementation.");
		}
		
		public object VisitOptionDeclaration(OptionDeclaration optionDeclaration, object data)
		{
			AddError(optionDeclaration, "Option statement is not supported.");
			return null;
		}
		
		public object VisitExternAliasDirective(ExternAliasDirective externAliasDirective, object data)
		{
			AddError(externAliasDirective, "'extern alias' directive is not supported.");
			return null;
		}
	}
}
