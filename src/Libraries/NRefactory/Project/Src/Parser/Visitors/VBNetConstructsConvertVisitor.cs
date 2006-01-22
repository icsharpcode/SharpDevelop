// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.NRefactory.Parser.AST;
using Attribute = ICSharpCode.NRefactory.Parser.AST.Attribute;

namespace ICSharpCode.NRefactory.Parser
{
	/// <summary>
	/// Converts special VB constructs to use more general AST classes.
	/// </summary>
	public class VBNetConstructsConvertVisitor : AbstractAstTransformer
	{
		// The following conversions are implemented:
		//   MyBase.New() and MyClass.New() calls inside the constructor are converted to :base() and :this()
		//   Add Public Modifier to methods and properties
		
		// The following conversions should be implemented in the future:
		//   Function A() \n A = SomeValue \n End Function -> convert to return statement
		
		Dictionary<string, string> usings;
		List<UsingDeclaration> addedUsings;
		
		public override object Visit(CompilationUnit compilationUnit, object data)
		{
			usings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			addedUsings = new List<UsingDeclaration>();
			base.Visit(compilationUnit, data);
			int i;
			for (i = 0; i < compilationUnit.Children.Count; i++) {
				if (!(compilationUnit.Children[i] is UsingDeclaration))
					break;
			}
			foreach (UsingDeclaration decl in addedUsings) {
				decl.Parent = compilationUnit;
				compilationUnit.Children.Insert(i++, decl);
			}
			usings = null;
			addedUsings = null;
			return null;
		}
		
		public override object Visit(Using @using, object data)
		{
			if (usings != null && !@using.IsAlias) {
				usings[@using.Name] = @using.Name;
			}
			return base.Visit(@using, data);
		}
		
		public override object Visit(ConstructorDeclaration constructorDeclaration, object data)
		{
			if ((constructorDeclaration.Modifier & Modifier.Visibility) == 0)
				constructorDeclaration.Modifier = Modifier.Public;
			
			// MyBase.New() and MyClass.New() calls inside the constructor are converted to :base() and :this()
			BlockStatement body = constructorDeclaration.Body;
			if (body != null && body.Children.Count > 0) {
				StatementExpression se = body.Children[0] as StatementExpression;
				if (se != null) {
					InvocationExpression ie = se.Expression as InvocationExpression;
					if (ie != null) {
						FieldReferenceExpression fre = ie.TargetObject as FieldReferenceExpression;
						if (fre != null && "New".Equals(fre.FieldName, StringComparison.InvariantCultureIgnoreCase)) {
							if (fre.TargetObject is BaseReferenceExpression || fre.TargetObject is ClassReferenceExpression) {
								body.Children.RemoveAt(0);
								ConstructorInitializer ci = new ConstructorInitializer();
								ci.Arguments = ie.Arguments;
								if (fre.TargetObject is BaseReferenceExpression)
									ci.ConstructorInitializerType = ConstructorInitializerType.Base;
								else
									ci.ConstructorInitializerType = ConstructorInitializerType.This;
								constructorDeclaration.ConstructorInitializer = ci;
							}
						}
					}
				}
			}
			return base.Visit(constructorDeclaration, data);
		}
		
		public override object Visit(DeclareDeclaration declareDeclaration, object data)
		{
			if (usings != null && !usings.ContainsKey("System.Runtime.InteropServices")) {
				UsingDeclaration @using = new UsingDeclaration("System.Runtime.InteropServices");
				addedUsings.Add(@using);
				base.Visit(@using, data);
			}
			
			MethodDeclaration method = new MethodDeclaration(declareDeclaration.Name, declareDeclaration.Modifier,
			                                                 declareDeclaration.TypeReference, declareDeclaration.Parameters,
			                                                 declareDeclaration.Attributes);
			method.Modifier |= Modifier.Extern | Modifier.Static;
			ICSharpCode.NRefactory.Parser.AST.Attribute att = new Attribute("DllImport", null, null);
			att.PositionalArguments.Add(CreateStringLiteral(declareDeclaration.Library));
			if (declareDeclaration.Alias.Length > 0) {
				att.NamedArguments.Add(new NamedArgumentExpression("EntryPoint", CreateStringLiteral(declareDeclaration.Alias)));
			}
			switch (declareDeclaration.Charset) {
				case CharsetModifier.Auto:
					att.NamedArguments.Add(new NamedArgumentExpression("CharSet",
					                                                   new FieldReferenceExpression(new IdentifierExpression("CharSet"),
					                                                                                "Auto")));
					break;
				case CharsetModifier.Unicode:
					att.NamedArguments.Add(new NamedArgumentExpression("CharSet",
					                                                   new FieldReferenceExpression(new IdentifierExpression("CharSet"),
					                                                                                "Unicode")));
					break;
				default:
					att.NamedArguments.Add(new NamedArgumentExpression("CharSet",
					                                                   new FieldReferenceExpression(new IdentifierExpression("CharSet"),
					                                                                                "Ansi")));
					break;
			}
			att.NamedArguments.Add(new NamedArgumentExpression("SetLastError", new PrimitiveExpression(true, true.ToString())));
			att.NamedArguments.Add(new NamedArgumentExpression("ExactSpelling", new PrimitiveExpression(true, true.ToString())));
			AttributeSection sec = new AttributeSection(null, null);
			sec.Attributes.Add(att);
			method.Attributes.Add(sec);
			ReplaceCurrentNode(method);
			return base.Visit(method, data);
		}
		
		static PrimitiveExpression CreateStringLiteral(string text)
		{
			return new PrimitiveExpression(text, text);
		}
		
		public override object Visit(MethodDeclaration methodDeclaration, object data)
		{
			if ((methodDeclaration.Modifier & Modifier.Visibility) == 0)
				methodDeclaration.Modifier |= Modifier.Public;
			
			if ((methodDeclaration.Modifier & (Modifier.Static | Modifier.Extern)) == Modifier.Static
			    && methodDeclaration.Body.Children.Count == 0)
			{
				foreach (AttributeSection sec in methodDeclaration.Attributes) {
					foreach (Attribute att in sec.Attributes) {
						if ("DllImport".Equals(att.Name, StringComparison.InvariantCultureIgnoreCase)) {
							methodDeclaration.Modifier |= Modifier.Extern;
							methodDeclaration.Body = null;
						}
					}
				}
			}
			
			return base.Visit(methodDeclaration, data);
		}
		
		public override object Visit(PropertyDeclaration propertyDeclaration, object data)
		{
			if ((propertyDeclaration.Modifier & Modifier.Visibility) == 0)
				propertyDeclaration.Modifier = Modifier.Public;
			
			return base.Visit(propertyDeclaration, data);
		}
	}
}
