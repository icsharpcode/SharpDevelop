// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
