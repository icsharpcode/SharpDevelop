using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory;
using System.IO;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace X64Converter
{
	class Program
	{
		static int Main(string[] args)
		{
			List<string> map = new List<string>()
			{
				"..\\Controller\\Profiler",
				"..\\Controller\\Data\\UnmanagedCallTreeNode",
				"..\\Controller\\structs"
			};

			foreach (string path in map)
			{
				using (IParser parser = ParserFactory.CreateParser(path + ".cs"))
				{
					parser.Parse();

					if (parser.Errors.Count > 0)
					{
						string message = "Parser errors in file " + path + ":\n" + parser.Errors.ErrorOutput;
						Console.WriteLine(message);
						File.WriteAllText(path + "64.cs", message);
						return 2;
					}
					
					var specials = parser.Lexer.SpecialTracker.RetrieveSpecials().Where(item => item is PreprocessingDirective);
					
					parser.CompilationUnit.AcceptVisitor(new Converter(), null);
					CSharpOutputVisitor output = new CSharpOutputVisitor();					
					SpecialNodesInserter.Install(specials, output);
					parser.CompilationUnit.AcceptVisitor(output, null);

					if (!File.Exists(path + "64.cs") || File.ReadAllText(path + "64.cs") != output.Text) {
						File.WriteAllText(path + "64.cs", output.Text);
					}
				}
			}

			return 0;
		}
	}

	class Converter : AbstractAstTransformer
	{
		bool copyAllMembers;

		public override object VisitTypeReference(TypeReference typeReference, object data)
		{
			if (!typeReference.IsKeyword)
				typeReference.Type = typeReference.Type.Replace("32", "64");
			return base.VisitTypeReference(typeReference, data);
		}

		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			identifierExpression.Identifier = identifierExpression.Identifier.Replace("32", "64");
			return base.VisitIdentifierExpression(identifierExpression, data);
		}

		public override object VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
		{
			memberReferenceExpression.MemberName = memberReferenceExpression.MemberName.Replace("32", "64");
			return base.VisitMemberReferenceExpression(memberReferenceExpression, data);
		}

		public override object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			pointerReferenceExpression.MemberName = pointerReferenceExpression.MemberName.Replace("32", "64");
			return base.VisitPointerReferenceExpression(pointerReferenceExpression, data);
		}

		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			if (methodDeclaration.Name.EndsWith("32"))
				methodDeclaration.Name = methodDeclaration.Name.Replace("32", "64");
			else {
				if (!this.copyAllMembers)
					this.RemoveCurrentNode();
			}
			return base.VisitMethodDeclaration(methodDeclaration, data);
		}

		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			if (!this.copyAllMembers)
				this.RemoveCurrentNode();
			return base.VisitPropertyDeclaration(propertyDeclaration, data);
		}

		public override object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			if (!this.copyAllMembers)
				this.RemoveCurrentNode();
			return base.VisitFieldDeclaration(fieldDeclaration, data);
		}

		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			if (!this.copyAllMembers)
				this.RemoveCurrentNode();
			return base.VisitConstructorDeclaration(constructorDeclaration, data);
		}

		public override object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			if (!this.copyAllMembers)
				this.RemoveCurrentNode();
			return base.VisitEventDeclaration(eventDeclaration, data);
		}

		public override object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			if (primitiveExpression.Value is string)
				primitiveExpression.Value = ((string)primitiveExpression.Value).Replace("32", "64");
			return base.VisitPrimitiveExpression(primitiveExpression, data);
		}

		public override object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			if (!this.copyAllMembers)
				this.RemoveCurrentNode();
			return base.VisitDestructorDeclaration(destructorDeclaration, data);
		}

		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			if (typeDeclaration.Name.EndsWith("32")) {
				this.copyAllMembers = true;
				typeDeclaration.Name = typeDeclaration.Name.Replace("32", "64");
			} else {
				if ((typeDeclaration.Modifier & Modifiers.Partial) != Modifiers.Partial)
					this.RemoveCurrentNode();
				else
					typeDeclaration.Attributes.Clear();

				this.copyAllMembers = false;
			}
			return base.VisitTypeDeclaration(typeDeclaration, data);
		}
	}
}
