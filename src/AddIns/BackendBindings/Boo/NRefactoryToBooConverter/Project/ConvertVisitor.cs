// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;
using Boo.Lang.Compiler;
using B = Boo.Lang.Compiler.Ast;

namespace NRefactoryToBooConverter
{
	public partial class ConvertVisitor : IASTVisitor
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
			Point point = node.StartLocation;
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
			warnings.Add(new CompilerWarning(warningMessage));
		}
		
		void AddWarning(INode node, string warningMessage)
		{
			AddWarning(GetLexicalInfo(node), warningMessage);
		}
		
		B.SourceLocation GetEndLocation(INode node)
		{
			return GetLocation(node.EndLocation);
		}
		
		B.SourceLocation GetLocation(Point point)
		{
			return new B.SourceLocation(point.Y, point.X);
		}
		
		B.TypeMemberModifiers ConvertModifier(AttributedNode node, B.TypeMemberModifiers defaultVisibility)
		{
			Modifier m = node.Modifier;
			B.TypeMemberModifiers r = B.TypeMemberModifiers.None;
			if ((m & Modifier.Private) != 0)
				r |= B.TypeMemberModifiers.Private;
			if ((m & Modifier.Internal) != 0)
				r |= B.TypeMemberModifiers.Internal;
			if ((m & Modifier.Public) != 0)
				r |= B.TypeMemberModifiers.Public;
			if ((m & Modifier.Protected) != 0)
				r |= B.TypeMemberModifiers.Protected;
			if (r == B.TypeMemberModifiers.None)
				r = defaultVisibility;
			
			if ((m & Modifier.Abstract) != 0)
				r |= B.TypeMemberModifiers.Abstract;
			if ((m & Modifier.Virtual) != 0)
				r |= B.TypeMemberModifiers.Virtual;
			if ((m & Modifier.Sealed) != 0)
				r |= B.TypeMemberModifiers.Final;
			if ((m & Modifier.Static) != 0) {
				r |= B.TypeMemberModifiers.Static;
			} else if (currentType != null && currentType.IsStatic) {
				if (!(node is TypeDeclaration))
					r |= B.TypeMemberModifiers.Static;
			} else {
				if ((m & (Modifier.Abstract | Modifier.Virtual | Modifier.Override)) == 0) {
					if (node is MethodDeclaration || node is PropertyDeclaration)
						r |= B.TypeMemberModifiers.Final;
				}
			}
			if ((m & Modifier.Override) != 0)
				r |= B.TypeMemberModifiers.Override;
			if ((m & Modifier.Readonly) != 0 && !(node is PropertyDeclaration)) {
				// allow readonly on VB properties only
				AddWarning(node, "readonly modifier is ignored");
			}
			if ((m & Modifier.Const) != 0)
				r |= B.TypeMemberModifiers.Final;
			if ((m & Modifier.New) != 0) {
				AddError(node, "shadowing is not supported");
			}
			if ((m & Modifier.Partial) != 0) {
				AddError(node, "Partial types are not supported");
			}
			if ((m & Modifier.Extern) != 0) {
				AddError(node, "Extern modifier is not supported");
			}
			if ((m & Modifier.Volatile) != 0) {
				AddError(node, "Volatile modifier is not supported");
			}
			if ((m & Modifier.Unsafe) != 0) {
				AddError(node, "Unsafe modifier is not supported");
			}
			if ((m & Modifier.Overloads) != 0) {
				// not necessary in Boo
			}
			if ((m & Modifier.WithEvents) != 0) {
				// not necessary in Boo
			}
			if ((m & Modifier.Default) != 0) {
				ParametrizedNode parametrizedNode = node as ParametrizedNode;
				string name = null;
				if (node is IndexerDeclaration) {
					name = DefaultIndexerName;
				} else if (parametrizedNode != null) {
					name = parametrizedNode.Name;
				} else {
					AddError(node, "Default modifier is not supported on this member.");
				}
				if (name != null && currentType != null) {
					currentType.Attributes.Add(MakeAttribute("System.Reflection.DefaultMember", new B.StringLiteralExpression(name)));
				}
			}
			if ((m & Modifier.Narrowing) != 0) {
				AddError(node, "Narrowing modifier is not supported");
			}
			if ((m & Modifier.Widening) != 0) {
				AddError(node, "Widening modifier is not supported");
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
		
		class BooTypeSystemServices : Boo.Lang.Compiler.TypeSystem.TypeSystemServices
		{
			public System.Collections.Hashtable Primitives {
				get {
					return _primitives;
				}
			}
		}
		
		string GetIntrinsicTypeName(string typeName)
		{
			if (settings.SimplifyTypeNames) {
				if (intrinsicTypeDictionary == null) {
					intrinsicTypeDictionary = new Dictionary<string, string>();
					foreach (System.Collections.DictionaryEntry entry in new BooTypeSystemServices().Primitives) {
						try {
							intrinsicTypeDictionary.Add(((Boo.Lang.Compiler.TypeSystem.IEntity)entry.Value).FullName, (string)entry.Key);
						} catch (ArgumentException) {}
					}
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
			B.TypeReference r = new B.SimpleTypeReference(GetLexicalInfo(t), GetIntrinsicTypeName(t.SystemType));
			if (t.GenericTypes.Count > 0) {
				AddError(t, "Consuming generics is not supported by boo.");
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
		
		/// <summary>
		/// Convert TypeReference and wrap it into an TypeofExpression.
		/// </summary>
		B.TypeofExpression WrapTypeReference(TypeReference t)
		{
			return new B.TypeofExpression(GetLexicalInfo(t), ConvertTypeReference(t));
		}
		
		public object Visit(TypeReference typeReference, object data)
		{
			return ConvertTypeReference(typeReference);
		}
		
		public object Visit(InnerClassTypeReference typeReference, object data)
		{
			return ConvertTypeReference(typeReference);
		}
	}
}
