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
using IronPython.Compiler.Ast;

namespace ICSharpCode.PythonBinding
{
	public class PythonLocalVariableAssignment
	{
		AssignmentStatement assignment;
		string variableName = String.Empty;
		string typeName = String.Empty;
		
		public PythonLocalVariableAssignment(AssignmentStatement assignment)
		{
			this.assignment = assignment;
			ParseAssignment();
		}
		
		public string TypeName {
			get { return typeName; }
		}
		
		public string VariableName {
			get { return variableName; }
		}
		
		public bool IsLocalVariableAssignment()
		{
			return !String.IsNullOrEmpty(variableName);
		}
		
		void ParseAssignment()
		{
			NameExpression nameExpression = assignment.Left[0] as NameExpression;
			CallExpression callExpression = assignment.Right as CallExpression;
			if ((nameExpression != null) && (callExpression != null)) {
				variableName = nameExpression.Name;
				typeName = GetTypeName(callExpression.Target);
			}
		}
		
		/// <summary>
		/// Gets the fully qualified name of the type from the expression.
		/// </summary>
		/// <remarks>
		/// The expression is the first target of a call expression.
		/// 
		/// A call expression is a method or constructor call (right hand side of expression below):
		/// 
		/// a = Root.Test.Class1()
		/// 
		/// So the expression passed to this method will be a field expression in the
		/// above example which refers to Class1. The next target will be a field
		/// expression referring to Test. The The last target will be a name expression
		/// referring to Root.
		/// 
		/// If we have 
		/// 
		/// a = Class1()
		/// 
		/// then the expression will be a name expression referring to Class1.
		/// </remarks>
		string GetTypeName(Expression expression)
		{
			NameExpression nameExpression = expression as NameExpression;
			if (nameExpression != null) {
				return nameExpression.Name;
			}
			return PythonControlFieldExpression.GetMemberName(expression as MemberExpression);
		}
	}
}
