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
using System.Collections.Generic;
using System.IO;

using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace X64Converter
{
	class Program
	{
		static int Main(string[] args)
		{
			try {
				File.Delete("conversion.log");
				
				List<string> map = new List<string>() {
					"..\\Controller\\Profiler",
					"..\\Controller\\Data\\UnmanagedCallTreeNode",
					"..\\Controller\\structs"
				};
				
				foreach (string path in map) {
					CSharpParser parser = new CSharpParser();
					#if DEBUG
					parser.CompilerSettings.ConditionalSymbols.Add("DEBUG");
					#endif
					string filePath = path + ".cs";
					
					if (File.Exists(filePath)) {
						using (StreamReader reader = new StreamReader(filePath)) {
							SyntaxTree syntaxTree = parser.Parse(reader, filePath);
							
							if (parser.HasErrors) {
								string message = "Parser errors in file " + filePath + ":\n";
								foreach (Error error in parser.Errors) {
									message += error.Message + "\n";
								}
								Console.WriteLine(message);
								File.WriteAllText(path + "64.cs", message);
								return 2;
							}
							
							syntaxTree.AcceptVisitor(new Converter());
							
							using (StreamWriter writer = new StreamWriter(path + "64.cs")) {
								CSharpOutputVisitor output = new CSharpOutputVisitor(writer, FormattingOptionsFactory.CreateSharpDevelop());
								syntaxTree.AcceptVisitor(output);
							}
						}
					}
				}

				return 0;
			} catch (Exception e) {
				try {
					File.WriteAllText("conversion.log", e.ToString());
				} catch (Exception) {
					return -2;
				}
				return -1;
			}
		}
	}

	class Converter : DepthFirstAstVisitor<object>
	{
		bool copyAllMembers;
		
		public override object VisitNewLine(NewLineNode newLineNode)
		{
			newLineNode.Remove();
			return base.VisitNewLine(newLineNode);
		}
		
		public override object VisitSimpleType(SimpleType simpleType)
		{
			simpleType.Identifier = simpleType.Identifier.Replace("32", "64");
			return base.VisitSimpleType(simpleType);
		}

		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression)
		{
			identifierExpression.Identifier = identifierExpression.Identifier.Replace("32", "64");
			return base.VisitIdentifierExpression(identifierExpression);
		}

		public override object VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
		{
			memberReferenceExpression.MemberName = memberReferenceExpression.MemberName.Replace("32", "64");
			return base.VisitMemberReferenceExpression(memberReferenceExpression);
		}

		public override object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression)
		{
			pointerReferenceExpression.MemberName = pointerReferenceExpression.MemberName.Replace("32", "64");
			return base.VisitPointerReferenceExpression(pointerReferenceExpression);
		}

		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration)
		{
			if (methodDeclaration.Name.EndsWith("32", StringComparison.Ordinal))
				methodDeclaration.Name = methodDeclaration.Name.Replace("32", "64");
			else {
				if (!copyAllMembers)
					methodDeclaration.Remove();
			}
			return base.VisitMethodDeclaration(methodDeclaration);
		}

		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
		{
			if (!copyAllMembers)
				propertyDeclaration.Remove();
			return base.VisitPropertyDeclaration(propertyDeclaration);
		}

		public override object VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
		{
			if (!copyAllMembers)
				fieldDeclaration.Remove();
			return base.VisitFieldDeclaration(fieldDeclaration);
		}

		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
		{
			if (!copyAllMembers)
				constructorDeclaration.Remove();
			return base.VisitConstructorDeclaration(constructorDeclaration);
		}

		public override object VisitEventDeclaration(EventDeclaration eventDeclaration)
		{
			if (!copyAllMembers)
				eventDeclaration.Remove();
			return base.VisitEventDeclaration(eventDeclaration);
		}

		public override object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression)
		{
			if (primitiveExpression.Value is string)
				primitiveExpression.Value = ((string)primitiveExpression.Value).Replace("32", "64");
			return base.VisitPrimitiveExpression(primitiveExpression);
		}

		public override object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
		{
			if (!copyAllMembers)
				destructorDeclaration.Remove();
			return base.VisitDestructorDeclaration(destructorDeclaration);
		}

		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration)
		{
			if (typeDeclaration.Name.EndsWith("32", StringComparison.Ordinal)) {
				copyAllMembers = true;
				typeDeclaration.Name = typeDeclaration.Name.Replace("32", "64");
			} else {
				if (!typeDeclaration.Modifiers.HasFlag(Modifiers.Partial))
					typeDeclaration.Remove();
				else
					typeDeclaration.Attributes.Clear();

				copyAllMembers = false;
			}
			return base.VisitTypeDeclaration(typeDeclaration);
		}
	}
}
