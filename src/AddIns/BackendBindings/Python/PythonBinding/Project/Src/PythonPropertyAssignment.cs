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
		
		public bool IsProperty()
		{
			CallExpression rhs = assignmentStatement.Right as CallExpression;
			if (rhs != null) {
				NameExpression nameExpression = rhs.Target as NameExpression;
				if (nameExpression != null) {
					return nameExpression.Name == "property";
				}
			}
			return false;
		}
		
		public void AddPropertyToClass(IClass c)
		{
			NameExpression nameExpression = assignmentStatement.Left[0] as NameExpression;
			if (nameExpression != null) {
				string propertyName = nameExpression.Name;
				AddPropertyToClass(c, propertyName);
			}
		}
		
		void AddPropertyToClass(IClass c, string propertyName)
		{
			DefaultProperty property = new DefaultProperty(c, propertyName);
			property.Region = GetPropertyRegion();
			c.Properties.Add(property);
		}
		
		DomRegion GetPropertyRegion()
		{
			int line = assignmentStatement.Start.Line;
			int column = assignmentStatement.Start.Column;
			return new DomRegion(line, column, line, column);
		}
	}
}
