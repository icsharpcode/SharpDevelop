#region license
// Copyright (c) 2005, Daniel Grunwald (daniel@danielgrunwald.de)
// All rights reserved.
//
// NRefactoryToBoo is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// NRefactoryToBoo is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with NRefactoryToBoo; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;
using Boo.Lang.Compiler;
using B = Boo.Lang.Compiler.Ast;

namespace NRefactoryToBooConverter
{
	partial class ConvertVisitor
	{
		public object Visit(INode node, object data)
		{
			AddError(node, "Visited INode: " + node);
			return null;
		}
		
		public object Visit(CompilationUnit compilationUnit, object data)
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
		
		public object Visit(NamespaceDeclaration namespaceDeclaration, object data)
		{
			if (module.Namespace != null) {
				AddError(namespaceDeclaration, "Only one namespace declaration per file is supported.");
				return null;
			}
			module.Namespace = new B.NamespaceDeclaration(GetLexicalInfo(namespaceDeclaration));
			module.Namespace.Name = namespaceDeclaration.Name;
			return namespaceDeclaration.AcceptChildren(this, data);
		}
		
		public object Visit(UsingDeclaration usingDeclaration, object data)
		{
			foreach (Using u in usingDeclaration.Usings) {
				Visit(u, data);
			}
			return null;
		}
		
		public object Visit(Using @using, object data)
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
		
		public object Visit(TypeDeclaration typeDeclaration, object data)
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
					typeDeclaration.Modifier |= Modifier.Static;
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
		
		public object Visit(DelegateDeclaration delegateDeclaration, object data)
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
				foreach (ICSharpCode.NRefactory.Parser.AST.Attribute a in s.Attributes) {
					col.Add((B.Attribute)Visit(a, null));
				}
			}
		}
		
		public object Visit(ICSharpCode.NRefactory.Parser.AST.Attribute a, object data)
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
		
		public object Visit(AttributeSection s, object data)
		{
			if (s.AttributeTarget.ToLower() == "assembly") {
				foreach (ICSharpCode.NRefactory.Parser.AST.Attribute a in s.Attributes) {
					module.AssemblyAttributes.Add((B.Attribute)Visit(a, null));
				}
			} else {
				AddError(s, "Attribute must have the target 'assembly'");
			}
			return null;
		}
		
		// Some classes are handled by their parent (NamedArgumentExpression by Attribute etc.)
		// so we don't need to implement Visit for them.
		public object Visit(TemplateDefinition templateDefinition, object data)
		{
			throw new ApplicationException("Visited TemplateDefinition.");
		}
		
		public object Visit(NamedArgumentExpression namedArgumentExpression, object data)
		{
			throw new ApplicationException("Visited NamedArgumentExpression.");
		}
		
		public object Visit(OptionDeclaration optionDeclaration, object data)
		{
			AddError(optionDeclaration, "Option statement is not supported.");
			return null;
		}
	}
}
