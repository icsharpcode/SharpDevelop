// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using IronPython.Compiler.Ast;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class PythonLocalVariableAssignmentTests
	{
		[Test]
		public void TypeName_CallExpressionTargetIsNotNameOrMemberExpression_ReturnsEmptyStringAndDoesNotGetStuckInInfiniteLoop()
		{
			string code = "a = 2";
			AssignmentStatement statement = PythonParserHelper.GetAssignmentStatement(code);
			Expression constantExpression = statement.Right;
			
			CallExpression callExpression = new CallExpression(constantExpression, new Arg[0]);
			List<Expression> expressions = new List<Expression>(statement.Left);
			statement = new AssignmentStatement(expressions.ToArray(), callExpression);
			
			PythonLocalVariableAssignment localVariableAssignment = new PythonLocalVariableAssignment(statement);
			string typeName = localVariableAssignment.TypeName;
			
			Assert.AreEqual(String.Empty, typeName);
		}
	}
}
