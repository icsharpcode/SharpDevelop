// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// C# ambience.
	/// </summary>
	public class CSharpAmbience : IAmbience
	{
		public ConversionFlags ConversionFlags { get; set; }
		
		#region ConvertEntity
		public string ConvertEntity(IEntity e, ITypeResolveContext context)
		{
			using (var ctx = context.Synchronize()) {
				StringWriter writer = new StringWriter();
				
				switch (e.EntityType) {
					case EntityType.None:
						break;
					case EntityType.TypeDefinition:
						ConvertTypeDeclaration((ITypeDefinition)e, ctx, writer);
						break;
					case EntityType.Field:
						ConvertField((IField)e, ctx, writer);
						break;
					case EntityType.Property:
						
						break;
					case EntityType.Indexer:
						
						break;
					case EntityType.Event:
						
						break;
					case EntityType.Method:
						
						break;
					case EntityType.Operator:
						
						break;
					case EntityType.Constructor:
						
						break;
					case EntityType.Destructor:
						
						break;
					default:
						throw new Exception("Invalid value for EntityType");
				}
				
				return writer.ToString().TrimEnd();
			}
		}
		
		void ConvertField(IField field, ISynchronizedTypeResolveContext ctx, StringWriter writer)
		{
			TypeSystemAstBuilder astBuilder = new TypeSystemAstBuilder(ctx);
			astBuilder.ShowModifiers = (ConversionFlags & ConversionFlags.ShowModifiers) == ConversionFlags.ShowModifiers;
			astBuilder.ShowAccessibility = (ConversionFlags & ConversionFlags.ShowAccessibility) == ConversionFlags.ShowAccessibility;
			FieldDeclaration fieldDecl = (FieldDeclaration)astBuilder.ConvertEntity(field);
			PrintModifiers(fieldDecl.Modifiers, writer);
			if ((ConversionFlags & ConversionFlags.ShowReturnType) == ConversionFlags.ShowReturnType) {
				writer.Write(ConvertType(field.Type, ctx));
				writer.Write(' ');
			}
			WriteFieldDeclarationName(field, ctx, writer);
		}
		
		void ConvertTypeDeclaration(ITypeDefinition typeDef, ITypeResolveContext ctx, StringWriter writer)
		{
			TypeSystemAstBuilder astBuilder = new TypeSystemAstBuilder(ctx);
			astBuilder.ShowModifiers = (ConversionFlags & ConversionFlags.ShowModifiers) == ConversionFlags.ShowModifiers;
			astBuilder.ShowAccessibility = (ConversionFlags & ConversionFlags.ShowAccessibility) == ConversionFlags.ShowAccessibility;
			TypeDeclaration typeDeclaration = (TypeDeclaration)astBuilder.ConvertEntity(typeDef);
			PrintModifiers(typeDeclaration.Modifiers, writer);
			if ((ConversionFlags & ConversionFlags.ShowDefinitionKeyWord) == ConversionFlags.ShowDefinitionKeyWord) {
				switch (typeDeclaration.ClassType) {
					case ClassType.Class:
						writer.Write("class");
						break;
					case ClassType.Struct:
						writer.Write("struct");
						break;
					case ClassType.Interface:
						writer.Write("interface");
						break;
					case ClassType.Enum:
						writer.Write("enum");
						break;
					default:
						throw new Exception("Invalid value for ClassType");
				}
				writer.Write(' ');
			}
			WriteTypeDeclarationName(typeDef, ctx, writer);
		}

		void WriteTypeDeclarationName(ITypeDefinition typeDef, ITypeResolveContext ctx, StringWriter writer)
		{
			TypeSystemAstBuilder astBuilder = new TypeSystemAstBuilder(ctx);
			if (typeDef.DeclaringTypeDefinition != null) {
				WriteTypeDeclarationName(typeDef.DeclaringTypeDefinition, ctx, writer);
				writer.Write('.');
			} else if ((ConversionFlags & ConversionFlags.UseFullyQualifiedMemberNames) == ConversionFlags.UseFullyQualifiedMemberNames) {
				writer.Write(typeDef.Namespace);
				writer.Write('.');
			}
			writer.Write(typeDef.Name);
			if ((ConversionFlags & ConversionFlags.ShowTypeParameterList) == ConversionFlags.ShowTypeParameterList) {
				CreatePrinter(writer).WriteTypeParameters(((TypeDeclaration)astBuilder.ConvertEntity(typeDef)).TypeParameters);
			}
		}
		
		void WriteFieldDeclarationName(IField field, ITypeResolveContext ctx, StringWriter writer)
		{
			if ((ConversionFlags & ConversionFlags.UseFullyQualifiedMemberNames) == ConversionFlags.UseFullyQualifiedMemberNames) {
				writer.Write(ConvertType(field.DeclaringType));
				writer.Write('.');
			}
			writer.Write(field.Name);
		}
		
		void PrintNode(AstNode node, StringWriter writer)
		{
			node.AcceptVisitor(CreatePrinter(writer), null);
		}
		
		OutputVisitor CreatePrinter(StringWriter writer)
		{
			return new OutputVisitor(writer, new CSharpFormattingOptions());
		}
		
		void PrintModifiers(Modifiers modifiers, StringWriter writer)
		{
			foreach (var m in CSharpModifierToken.AllModifiers) {
				if ((modifiers & m) == m) {
					writer.Write(CSharpModifierToken.GetModifierName(m));
					writer.Write(' ');
				}
			}
		}
		#endregion
		
		public string ConvertVariable(IVariable v, ITypeResolveContext context)
		{
			using (var ctx = context.Synchronize()) {
				TypeSystemAstBuilder astBuilder = new TypeSystemAstBuilder(ctx);
				AstNode astNode = astBuilder.ConvertVariable(v);
				CSharpFormattingOptions formatting = new CSharpFormattingOptions();
				StringWriter writer = new StringWriter();
				astNode.AcceptVisitor(new OutputVisitor(writer, formatting), null);
				return writer.ToString();
			}
		}
		
		public string ConvertType(IType type)
		{
			using (var ctx = ParserService.CurrentTypeResolveContext.Synchronize()) {
				TypeSystemAstBuilder astBuilder = new TypeSystemAstBuilder(ctx);
				AstType astType = astBuilder.ConvertType(type);
				CSharpFormattingOptions formatting = new CSharpFormattingOptions();
				StringWriter writer = new StringWriter();
				astType.AcceptVisitor(new OutputVisitor(writer, formatting), null);
				return writer.ToString();
			}
		}
		
		public string ConvertType(ITypeReference type, ITypeResolveContext context)
		{
			TypeSystemAstBuilder astBuilder = new TypeSystemAstBuilder(context);
			AstType astType = astBuilder.ConvertTypeReference(type);
			CSharpFormattingOptions formatting = new CSharpFormattingOptions();
			StringWriter writer = new StringWriter();
			astType.AcceptVisitor(new OutputVisitor(writer, formatting), null);
			return writer.ToString();
		}
		
		public string WrapAttribute(string attribute)
		{
			return "[" + attribute + "]";
		}
		
		public string WrapComment(string comment)
		{
			return "// " + comment;
		}
	}
}
