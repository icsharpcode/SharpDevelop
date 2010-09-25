// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;

using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Dom;
using IronPython;
using IronPython.Compiler;
using IronPython.Compiler.Ast;
using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Determines the type of a variable.
	/// </summary>
	public class PythonLocalVariableResolver : PythonWalker, IPythonResolver
	{
		string variableName = String.Empty;
		string typeName;
		AssignmentStatement currentAssignStatement;
		bool foundVariableAssignment;
		
		/// <summary>
		/// The resolved type name.
		/// </summary>
		public string TypeName {
			get { return typeName; }
		}
		
		/// <summary>
		/// Resolves the type of the variable name specified.
		/// </summary>
		/// <param name="variableName">Name of the variable.</param>
		/// <param name="code">The python code containing the variable.</param>
		public string Resolve(string variableName, string code)
		{
			if (code != null) {
				PythonParser parser = new PythonParser();
				PythonAst ast = parser.CreateAst("resolver.py", code);
				return Resolve(variableName, ast);
			}
			return null;
		}
		
		string Resolve(string variableName, PythonAst ast)
		{
			this.variableName = variableName;
			ast.Walk(this);
			return TypeName;
		}
				
		public override bool Walk(AssignmentStatement node)
		{
			currentAssignStatement = node;
			foundVariableAssignment = false;
			return base.Walk(node);
		}		
		
		public override bool Walk(NameExpression node)
		{
			if (currentAssignStatement != null) {
				string nodeName = node.Name;
				if (nodeName == variableName) {
					foundVariableAssignment = true;
				}
			}
			return base.Walk(node);
		}
		
		public override bool Walk(CallExpression node)
		{
			if (foundVariableAssignment) {
				typeName = GetTypeName(node.Target);
			}
			return base.Walk(node);
		}
		
		/// <summary>
		/// Gets the fully qualified name of the type from the expression.
		/// 
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
		public static string GetTypeName(Expression node)
		{
			NameExpression nameExpression = node as NameExpression;
			if (nameExpression != null) {
				return nameExpression.Name;
			}
			return PythonControlFieldExpression.GetMemberName(node as MemberExpression);
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			return GetLocalVariable(resolverContext, expressionResult);
		}
		
		/// <summary>
		/// Tries to find the type that matches the local variable name.
		/// </summary>
		LocalResolveResult GetLocalVariable(PythonResolverContext resolverContext, ExpressionResult expressionResult)
		{
			string code = GetLocalMethodCode(resolverContext.FileContent, expressionResult);
			string typeName = Resolve(expressionResult.Expression, code);
			if (typeName != null) {
				return CreateLocalResolveResult(typeName, expressionResult.Expression, resolverContext);
			}
			return null;
		}
		
		string GetLocalMethodCode(string fullCode, ExpressionResult expressionResult)
		{
			ScriptingLocalMethod localMethod = new ScriptingLocalMethod(fullCode);
			return localMethod.GetCode(expressionResult.Region.BeginLine);
		}
		
		LocalResolveResult CreateLocalResolveResult(string typeName, string identifier, PythonResolverContext resolverContext)
		{
			IClass resolvedClass = resolverContext.GetClass(typeName);
			if (resolvedClass != null) {
				return CreateLocalResolveResult(identifier, resolvedClass);
			}
			return null;
		}
		
		LocalResolveResult CreateLocalResolveResult(string identifier, IClass resolvedClass)
		{
			DefaultMethod dummyMethod = CreateDummyMethod();
			DefaultField.LocalVariableField field = CreateLocalVariableField(identifier, resolvedClass, dummyMethod.DeclaringType);
			return new LocalResolveResult(dummyMethod, field);
		}
		
		DefaultField.LocalVariableField CreateLocalVariableField(string identifier, IClass resolvedClass, IClass callingClass)
		{
			return new DefaultField.LocalVariableField(resolvedClass.DefaultReturnType,
				identifier,
				DomRegion.Empty, 
				callingClass);
		}
		
		DefaultMethod CreateDummyMethod()
		{
			DefaultClass dummyClass = new DefaultClass(DefaultCompilationUnit.DummyCompilationUnit, "Global");
			return new DefaultMethod(dummyClass, String.Empty);
		}
	}
}
