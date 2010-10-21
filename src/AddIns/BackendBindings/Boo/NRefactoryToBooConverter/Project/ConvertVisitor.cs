// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using Boo.Lang.Compiler;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using B = Boo.Lang.Compiler.Ast;

namespace NRefactoryToBooConverter
{
	public sealed partial class ConvertVisitor : IAstVisitor
	{
		string fileName;
		CompilerErrorCollection errors;
		CompilerWarningCollection warnings;
		StringComparer nameComparer;
		ConverterSettings settings;
		
		B.Module module;
		
		public ConvertVisitor(ConverterSettings settings)
		{
			this.settings = settings;
			this.fileName      = settings.FileName;
			this.errors        = settings.Errors;
			this.warnings      = settings.Warnings;
			this.nameComparer  = settings.NameComparer;
		}
		
		int generatedNameNumber;
		
		string GenerateName()
		{
			return settings.NameGenerationPrefix + (++generatedNameNumber).ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
		}
		
		B.LexicalInfo lastLexicalInfo;
		
		B.LexicalInfo GetLexicalInfo(INode node)
		{
			if (node == null)
				return new B.LexicalInfo(fileName);
			Location point = node.StartLocation;
			if (!point.IsEmpty) {
				lastLexicalInfo = new B.LexicalInfo(fileName, point.Y, point.X);
			}
			if (lastLexicalInfo == null) {
				lastLexicalInfo = new B.LexicalInfo(fileName);
			}
			return lastLexicalInfo;
		}
		
		void AddError(B.LexicalInfo lex, string errorMessage)
		{
			errors.Add(new CompilerError(lex, errorMessage));
		}
		
		void AddError(INode node, string errorMessage)
		{
			AddError(GetLexicalInfo(node), errorMessage);
		}
		
		void AddWarning(B.LexicalInfo lex, string warningMessage)
		{
			warnings.Add(new CompilerWarning(lex, warningMessage));
		}
		
		void AddWarning(INode node, string warningMessage)
		{
			AddWarning(GetLexicalInfo(node), warningMessage);
		}
		
		B.SourceLocation GetEndLocation(INode node)
		{
			return GetLocation(node.EndLocation);
		}
		
		B.SourceLocation GetLocation(Location point)
		{
			return new B.SourceLocation(point.Y, point.X);
		}
		
		B.TypeMemberModifiers ConvertModifier(AttributedNode node, B.TypeMemberModifiers defaultVisibility)
		{
			Modifiers m = node.Modifier;
			B.TypeMemberModifiers r = B.TypeMemberModifiers.None;
			if ((m & Modifiers.Private) != 0)
				r |= B.TypeMemberModifiers.Private;
			if ((m & Modifiers.Internal) != 0)
				r |= B.TypeMemberModifiers.Internal;
			if ((m & Modifiers.Public) != 0)
				r |= B.TypeMemberModifiers.Public;
			if ((m & Modifiers.Protected) != 0)
				r |= B.TypeMemberModifiers.Protected;
			if (r == B.TypeMemberModifiers.None)
				r = defaultVisibility;
			
			if ((m & Modifiers.Abstract) != 0)
				r |= B.TypeMemberModifiers.Abstract;
			if ((m & Modifiers.Virtual) != 0)
				r |= B.TypeMemberModifiers.Virtual;
			if ((m & Modifiers.Sealed) != 0)
				r |= B.TypeMemberModifiers.Final;
			if ((m & Modifiers.Static) != 0) {
				r |= B.TypeMemberModifiers.Static;
			} else if (currentType != null && currentType.IsStatic) {
				if (!(node is TypeDeclaration))
					r |= B.TypeMemberModifiers.Static;
			}
			if ((m & Modifiers.Override) != 0)
				r |= B.TypeMemberModifiers.Override;
			if ((m & Modifiers.ReadOnly) != 0 && !(node is PropertyDeclaration)) {
				r |= B.TypeMemberModifiers.Final;
			}
			if ((m & Modifiers.Const) != 0) {
				r |= B.TypeMemberModifiers.Final | B.TypeMemberModifiers.Static;
			}
			if ((m & Modifiers.New) != 0) {
				AddError(node, "shadowing is not supported");
			}
			if ((m & Modifiers.Partial) != 0) {
				r |= B.TypeMemberModifiers.Partial;
			}
			if ((m & Modifiers.Extern) != 0) {
				// not necessary in Boo
			}
			if ((m & Modifiers.Volatile) != 0) {
				AddError(node, "Volatile modifier is not supported");
			}
			if ((m & Modifiers.Unsafe) != 0) {
				AddError(node, "Unsafe modifier is not supported");
			}
			if ((m & Modifiers.Overloads) != 0) {
				// not necessary in Boo
			}
			if ((m & Modifiers.WithEvents) != 0) {
				// not necessary in Boo
			}
			return r;
		}
		
		B.Attribute MakeAttribute(string name, params B.Expression[] arguments)
		{
			B.Attribute a = new B.Attribute(lastLexicalInfo, name);
			foreach (B.Expression arg in arguments) {
				a.Arguments.Add(arg);
			}
			return a;
		}
		
		void ConvertTypeReferences(List<TypeReference> input, B.TypeReferenceCollection output)
		{
			foreach (TypeReference t in input) {
				B.TypeReference r = ConvertTypeReference(t);
				if (r != null) {
					output.Add(r);
				}
			}
		}
		
		Dictionary<string, string> intrinsicTypeDictionary;
		
		string GetIntrinsicTypeName(string typeName)
		{
			if (settings.SimplifyTypeNames) {
				if (intrinsicTypeDictionary == null) {
					intrinsicTypeDictionary = new Dictionary<string, string>();
					intrinsicTypeDictionary.Add("System.Void", "void");
					intrinsicTypeDictionary.Add("System.Object", "object");
					intrinsicTypeDictionary.Add("System.Boolean", "bool");
					intrinsicTypeDictionary.Add("System.Byte", "byte");
					intrinsicTypeDictionary.Add("System.SByte", "sbyte");
					intrinsicTypeDictionary.Add("System.Char", "char");
					intrinsicTypeDictionary.Add("System.Int16", "short");
					intrinsicTypeDictionary.Add("System.Int32", "int");
					intrinsicTypeDictionary.Add("System.Int64", "long");
					intrinsicTypeDictionary.Add("System.UInt16", "ushort");
					intrinsicTypeDictionary.Add("System.UInt32", "uint");
					intrinsicTypeDictionary.Add("System.UInt64", "ulong");
					intrinsicTypeDictionary.Add("System.Single", "single");
					intrinsicTypeDictionary.Add("System.Double", "double");
					intrinsicTypeDictionary.Add("System.Decimal", "decimal");
					intrinsicTypeDictionary.Add("System.String", "string");
					intrinsicTypeDictionary.Add("System.DateTime", "date");
					intrinsicTypeDictionary.Add("System.TimeSpan", "timespan");
					intrinsicTypeDictionary.Add("System.Type", "type");
					intrinsicTypeDictionary.Add("System.Array", "array");
					intrinsicTypeDictionary.Add("System.Text.RegularExpressions.Regex", "regex");
				}
				string result;
				if (intrinsicTypeDictionary.TryGetValue(typeName, out result))
					return result;
			}
			return typeName;
		}
		
		B.TypeReference ConvertTypeReference(TypeReference t)
		{
			if (t == null || t.IsNull)
				return null;
			B.TypeReference r;
			if (t.GenericTypes.Count > 0) {
				r = new B.GenericTypeReference(GetLexicalInfo(t), GetIntrinsicTypeName(t.Type));
				foreach (TypeReference ta in t.GenericTypes) {
					((B.GenericTypeReference)r).GenericArguments.Add(ConvertTypeReference(ta));
				}
			} else {
				r = new B.SimpleTypeReference(GetLexicalInfo(t), GetIntrinsicTypeName(t.Type));
			}
			if (t.IsArrayType) {
				for (int i = t.RankSpecifier.Length - 1; i >= 0; --i) {
					r = new B.ArrayTypeReference(GetLexicalInfo(t), r, new B.IntegerLiteralExpression(t.RankSpecifier[i] + 1));
				}
			}
			if (t.PointerNestingLevel != 0) {
				AddError(t, "Pointers are not supported by boo.");
			}
			return r;
		}
		
		B.TypeReference ConvertTypeReference(B.Expression expr)
		{
			B.TypeofExpression te = expr as B.TypeofExpression;
			if (te != null)
				return te.Type;
			if (expr is B.ReferenceExpression) {
				return new B.SimpleTypeReference(expr.ToCodeString());
			}
			AddError(expr.LexicalInfo, "Expected type, but found expression.");
			return null;
		}
		
		public object VisitTypeReference(TypeReference typeReference, object data)
		{
			return ConvertTypeReference(typeReference);
		}
		
		public object VisitInnerClassTypeReference(InnerClassTypeReference typeReference, object data)
		{
			return ConvertTypeReference(typeReference);
		}
		
		public object VisitCollectionRangeVariable(CollectionRangeVariable collectionRangeVariable, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitMemberInitializerExpression(MemberInitializerExpression memberInitializerExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitXmlAttributeExpression(XmlAttributeExpression xmlAttributeExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitXmlContentExpression(XmlContentExpression xmlContentExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitXmlDocumentExpression(XmlDocumentExpression xmlDocumentExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitXmlElementExpression(XmlElementExpression xmlElementExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitXmlEmbeddedExpression(XmlEmbeddedExpression xmlEmbeddedExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitXmlMemberAccessExpression(XmlMemberAccessExpression xmlMemberAccessExpression, object data)
		{
			throw new NotImplementedException();
		}
	}
}
