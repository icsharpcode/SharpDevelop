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

using Microsoft.Scripting;
using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler;
using IronPython.Compiler.Ast;
using IronPython.Runtime;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Walks the python parse tree.
	/// </summary>
	public class PythonAstWalker : PythonWalker
	{
		PythonCompilationUnit compilationUnit;
		IClass currentClass;
		PythonModule module;
		
		/// <summary>
		/// All classes in a file take the namespace of the filename. 
		/// </summary>
		public PythonAstWalker(IProjectContent projectContent, string fileName)
		{
			compilationUnit = new PythonCompilationUnit(projectContent, fileName);
		}
		
		/// <summary>
		/// Returns the compilation unit created after the Walk method
		/// has been called.
		/// </summary>
		public ICompilationUnit CompilationUnit {
			get { return compilationUnit; }
		}
		
		/// <summary>
		/// Walks the python statement returned from the parser.
		/// </summary>
		public void Walk(Statement statement)
		{
			statement.Walk(this);
		}
		
		public override bool Walk(ClassDefinition classDefinition)
		{
			if (classDefinition.Parent != null) {
				PythonClass c = new PythonClass(compilationUnit, classDefinition);
				WalkClassBody(c, classDefinition.Body);
			}
			return false;
		}
		
		void WalkClassBody(IClass c, Statement classBody)
		{
			currentClass = c;
			classBody.Walk(this);
			currentClass = null;
		}
		
		public override bool Walk(FunctionDefinition functionDefinition)
		{
			if (functionDefinition.Body == null) {
				return false;
			}
			
			IClass c = GetClassBeingWalked();
			
			PythonMethodDefinition methodDefinition = new PythonMethodDefinition(functionDefinition);
			PythonMethod method = methodDefinition.CreateMethod(c);
			if (method is PythonConstructor) {
				FindFields(c, functionDefinition);
			}
			return false;
		}
		
		/// <summary>
		/// If the current class is null then create a module so a method outside of a class can be
		/// parsed.
		/// </summary>
		IClass GetClassBeingWalked()
		{
			if (currentClass == null) {
				// Walking a method outside a class.
				CreateModule();
				return module;
			}
			return currentClass;
		}
		
		/// <summary>
		/// Creates the module which will act as a class so it can hold any methods defined in the module.
		/// </summary>
		void CreateModule()
		{
			if (module == null) {
				module = new PythonModule(compilationUnit);
			}
		}
		
		void FindFields(IClass c, FunctionDefinition functionDefinition)
		{
			PythonClassFields fields = new PythonClassFields(functionDefinition);
			fields.AddFields(c);
		}
		
		/// <summary>
		/// Walks an import statement and adds it to the compilation unit's
		/// Usings.
		/// </summary>
		public override bool Walk(ImportStatement node)
		{
			PythonImport import = new PythonImport(compilationUnit.ProjectContent, node);
			compilationUnit.UsingScope.Usings.Add(import);
			return false;
		}
		
		public override bool Walk(FromImportStatement node)
		{
			PythonFromImport import = new PythonFromImport(compilationUnit.ProjectContent, node);
			compilationUnit.UsingScope.Usings.Add(import);
			return false;
		}
		
		public override bool Walk(AssignmentStatement node)
		{
			if (currentClass != null) {
				FindProperty(node);
				return false;
			}
			return base.Walk(node);
		}
		
		void FindProperty(AssignmentStatement node)
		{
			PythonPropertyAssignment propertyAssignment = new PythonPropertyAssignment(node);
			propertyAssignment.CreateProperty(currentClass);
		}
	}
}
