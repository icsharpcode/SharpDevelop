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
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler.Ast;

namespace ICSharpCode.PythonBinding
{
	public class PythonPropertyAssignment
	{
		AssignmentStatement assignmentStatement;
		
		public PythonPropertyAssignment(AssignmentStatement assignmentStatement)
		{
			this.assignmentStatement = assignmentStatement;
		}
		
		public PythonProperty CreateProperty(IClass c)
		{
			if (IsProperty()) {
				NameExpression nameExpression = assignmentStatement.Left[0] as NameExpression;
				if (nameExpression != null) {
					string propertyName = nameExpression.Name;
					return CreateProperty(c, propertyName);
				}
			}
			return null;
		}
		
		bool IsProperty()
		{
			CallExpression callExpression = assignmentStatement.Right as CallExpression;
			if (callExpression != null) {
				return IsPropertyFunctionBeingCalled(callExpression);
			}
			return false;
		}
		
		bool IsPropertyFunctionBeingCalled(CallExpression callExpression)
		{
			NameExpression nameExpression = callExpression.Target as NameExpression;
			if (nameExpression != null) {
				return nameExpression.Name == "property";
			}
			return false;
		}
		
		PythonProperty CreateProperty(IClass c, string propertyName)
		{
			return new PythonProperty(c, propertyName, assignmentStatement.Start);
		}
	}
}
