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

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ICSharpCode.NRefactory.Ast;
//using ICSharpCode.PackageManagement.EnvDTE;
//using ICSharpCode.SharpDevelop.Dom;
//using ICSharpCode.SharpDevelop.Dom.Refactoring;
//
//namespace ICSharpCode.PackageManagement
//{
//	public class ClassCodeGenerator
//	{
//		public ClassCodeGenerator(IClass c)
//			: this(c, new DocumentLoader())
//		{
//		}
//		
//		public ClassCodeGenerator(IClass c, IDocumentLoader documentLoader)
//		{
//			this.Class = c;
//			this.DocumentLoader = documentLoader;
//		}
//		
//		IClass Class { get; set; }
//		IDocumentLoader DocumentLoader { get; set; }
//		
//		ICompilationUnit CompilationUnit {
//			get { return Class.CompilationUnit; }
//		}
//		
//		public CodeVariable AddPublicVariable(string name, string type)
//		{
//			CodeGenerator codeGenerator = GetCodeGenerator();
//			IRefactoringDocumentView view = LoadClassDocumentView();
//			codeGenerator.InsertCodeAtEnd(Class.Region, view.RefactoringDocument, CreateField(name, type));
//			return GetVariableInserted(view);
//		}
//		
//		CodeGenerator GetCodeGenerator()
//		{
//			return CompilationUnit.Language.CodeGenerator;
//		}
//		
//		IRefactoringDocumentView LoadClassDocumentView()
//		{
//			return DocumentLoader.LoadRefactoringDocumentView(CompilationUnit.FileName);
//		}
//		
//		FieldDeclaration CreateField(string name, string type)
//		{
//			return new FieldDeclaration(new List<AttributeSection>()) {
//				Fields = CreateVariableDeclarations(name),
//				Modifier = Modifiers.Public,
//				TypeReference = new TypeReference(type)
//			};
//		}
//		
//		List<VariableDeclaration> CreateVariableDeclarations(string name)
//		{
//			var variables = new List<VariableDeclaration>();
//			variables.Add(new VariableDeclaration(name));
//			return variables;
//		}
//		
//		CodeVariable GetVariableInserted(IRefactoringDocumentView view)
//		{
//			IField field = FindField(view);
//			return new CodeVariable(field);
//		}
//		
//		IField FindField(IRefactoringDocumentView view)
//		{
//			return FindMatchingClass(view).Fields.Last();
//		}
//		
//		IClass FindMatchingClass(IRefactoringDocumentView view)
//		{
//			ICompilationUnit unit = view.Parse();
//			return FindMatchingClass(unit);
//		}
//		
//		IClass FindMatchingClass(ICompilationUnit unit)
//		{
//			foreach (IClass c in unit.Classes) {
//				if (c.FullyQualifiedName == Class.FullyQualifiedName) {
//					return c;
//				}
//			}
//			return null;
//		}
//		
//		/// <summary>
//		/// The fully qualified type will not be added as the method's return value since
//		/// NRefactory returns the incorrect start column for the method's region in this case.
//		/// Instead we add the last part of the type so for "System.Object" we add "Object" so the
//		/// method's region is correct. This only seems to affect methods where there is no modifier
//		/// (e.g. public, private) explicitly defined in code.
//		/// 
//		/// For MvcScaffolding we only need the start and end points to be correctly defined since the
//		/// function will be immediately replaced with different code.
//		/// </summary>
//		public CodeFunction AddPublicMethod(string name, string type)
//		{
//			CodeGenerator codeGenerator = GetCodeGenerator();
//			IRefactoringDocumentView view = LoadClassDocumentView();
//			codeGenerator.InsertCodeAtEnd(Class.Region, view.RefactoringDocument, CreateMethod(name, type));
//			return GetMethodInserted(view);
//		}
//		
//		MethodDeclaration CreateMethod(string name, string type)
//		{
//			return new MethodDeclaration {
//				Name = name,
//				TypeReference = new TypeReference(GetShortTypeName(type))
//			};
//		}
//		
//		string GetShortTypeName(string type)
//		{
//			int index = type.LastIndexOf('.');
//			if (index > 0) {
//				return type.Substring(index + 1);
//			}
//			return type;
//		}
//		
//		CodeFunction GetMethodInserted(IRefactoringDocumentView view)
//		{
//			IMethod method = FindMethod(view);
//			return new CodeFunction(method);
//		}
//		
//		IMethod FindMethod(IRefactoringDocumentView view)
//		{
//			return FindMatchingClass(view).Methods.Last();
//		}
//	}
//}
