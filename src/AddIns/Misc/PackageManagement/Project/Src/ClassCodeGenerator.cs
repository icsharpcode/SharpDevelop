// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace ICSharpCode.PackageManagement
{
	public class ClassCodeGenerator
	{
		public ClassCodeGenerator(IClass c)
			: this(c, new DocumentLoader())
		{
		}
		
		public ClassCodeGenerator(IClass c, IDocumentLoader documentLoader)
		{
			this.Class = c;
			this.DocumentLoader = documentLoader;
		}
		
		IClass Class { get; set; }
		IDocumentLoader DocumentLoader { get; set; }
		
		ICompilationUnit CompilationUnit {
			get { return Class.CompilationUnit; }
		}
		
		public CodeVariable AddPublicVariable(string name, string type)
		{
			CodeGenerator codeGenerator = GetCodeGenerator();
			IRefactoringDocumentView view = LoadClassDocumentView();
			codeGenerator.InsertCodeAtEnd(Class.Region, view.RefactoringDocument, CreateField(name, type));
			return GetVariableInserted(view);
		}
		
		CodeGenerator GetCodeGenerator()
		{
			return CompilationUnit.Language.CodeGenerator;
		}
		
		IRefactoringDocumentView LoadClassDocumentView()
		{
			return DocumentLoader.LoadRefactoringDocumentView(CompilationUnit.FileName);
		}
		
		FieldDeclaration CreateField(string name, string type)
		{
			return new FieldDeclaration(new List<AttributeSection>()) {
				TypeReference = new TypeReference(type),
				Modifier = Modifiers.Public,
				Fields = CreateVariableDeclarations(name)
			};
		}
		
		List<VariableDeclaration> CreateVariableDeclarations(string name)
		{
			var variables = new List<VariableDeclaration>();
			variables.Add(new VariableDeclaration(name));
			return variables;
		}
		
		CodeVariable GetVariableInserted(IRefactoringDocumentView view)
		{
			IField field = FindField(view);
			return new CodeVariable(field);
		}
		
		IField FindField(IRefactoringDocumentView view)
		{
			ICompilationUnit unit = view.Parse();
			IClass matchedClass = FindMatchingClass(unit);
			return matchedClass.Fields.Last();
		}
		
		IClass FindMatchingClass(ICompilationUnit unit)
		{
			foreach (IClass c in unit.Classes) {
				if (c.FullyQualifiedName == Class.FullyQualifiedName) {
					return c;
				}
			}
			return null;
		}
	}
}
