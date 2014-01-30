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
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler.Ast;

namespace ICSharpCode.PythonBinding
{
	public class PythonClass : DefaultClass
	{
		public PythonClass(ICompilationUnit compilationUnit, ClassDefinition classDefinition)
			: base(compilationUnit, String.Empty)
		{
			GetFullyQualifiedName(classDefinition);
			GetClassRegions(classDefinition);
			AddBaseTypes(classDefinition.Bases);
			
			compilationUnit.Classes.Add(this);
		}
		
		/// <summary>
		/// Adds the namespace to the class name taken from the class definition.
		/// </summary>
		void GetFullyQualifiedName(ClassDefinition classDefinition)
		{
			string ns = CompilationUnit.UsingScope.NamespaceName;
			FullyQualifiedName = String.Format("{0}.{1}", ns, classDefinition.Name);
		}
		
		void GetClassRegions(ClassDefinition classDefinition)
		{
			GetRegion(classDefinition);
			BodyRegion = PythonMethodOrClassBodyRegion.GetBodyRegion(classDefinition);
		}
		
		/// <summary>
		/// Gets the region of the scope statement (ClassDefinition). 
		/// </summary>
		/// <remarks>
		/// A class region includes the body.
		/// </remarks>
		void GetRegion(ScopeStatement statement)
		{
			Region = new DomRegion(statement.Start.Line, statement.Start.Column, statement.End.Line, statement.End.Column);
		}
		
		/// <summary>
		/// Looks for any base types for the class defined in the
		/// list of expressions and adds them to the class.
		/// </summary>
		void AddBaseTypes(IList<Expression> baseTypes)
		{
			foreach (Expression baseTypeExpression in baseTypes) {
				AddBaseType(baseTypeExpression);
			}
		}
		
		void AddBaseType(Expression baseTypeExpression)
		{
			NameExpression nameExpression = baseTypeExpression as NameExpression;
			MemberExpression memberExpression = baseTypeExpression as MemberExpression;
			if (nameExpression != null) {
				AddBaseType(nameExpression.Name);
			} else if (memberExpression != null) {
				AddBaseType(memberExpression);
			}
		}
		
		/// <summary>
		/// Adds the named base type to the class.
		/// </summary>
		void AddBaseType(string name)
		{
			IReturnType returnType = CreateSearchClassReturnType(name);
			BaseTypes.Add(returnType);
		}
		
		void AddBaseType(MemberExpression memberExpression)
		{
			string name = PythonControlFieldExpression.GetMemberName(memberExpression);
			AddBaseType(name);
		}
		
		SearchClassReturnType CreateSearchClassReturnType(string name)
		{
			return new SearchClassReturnType(ProjectContent, this, 0, 0, name, 0);
		}
	}
}
