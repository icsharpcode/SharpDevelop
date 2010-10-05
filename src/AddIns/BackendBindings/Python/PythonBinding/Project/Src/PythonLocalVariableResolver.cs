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
		PythonClassResolver classResolver;
		string variableName = String.Empty;
		string typeName;
		
		public PythonLocalVariableResolver(PythonClassResolver classResolver)
		{
			this.classResolver = classResolver;
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
			typeName = null;
			this.variableName = variableName;
			ast.Walk(this);
			return typeName;
		}
				
		public override bool Walk(AssignmentStatement node)
		{
			PythonLocalVariableAssignment localVariableAssignment = new PythonLocalVariableAssignment(node);
			if (localVariableAssignment.IsLocalVariableAssignment()) {
				if (localVariableAssignment.VariableName == variableName) {
					typeName = localVariableAssignment.TypeName;
				}
			}
			return base.Walk(node);
		}
		
		public ResolveResult Resolve(PythonResolverContext resolverContext)
		{
			return GetLocalVariable(resolverContext);
		}
		
		/// <summary>
		/// Tries to find the type that matches the local variable name.
		/// </summary>
		LocalResolveResult GetLocalVariable(PythonResolverContext resolverContext)
		{
			string code = GetLocalMethodCode(resolverContext);
			string typeName = Resolve(resolverContext.Expression, code);
			if (typeName != null) {
				return CreateLocalResolveResult(typeName, resolverContext);
			}
			return null;
		}
		
		string GetLocalMethodCode(PythonResolverContext resolverContext)
		{
			ScriptingLocalMethod localMethod = new ScriptingLocalMethod(resolverContext.FileContent);
			int beginLine = resolverContext.ExpressionRegion.BeginLine;
			return localMethod.GetCode(beginLine);
		}
		
		LocalResolveResult CreateLocalResolveResult(string typeName, PythonResolverContext resolverContext)
		{
			IClass resolvedClass = classResolver.GetClass(resolverContext, typeName);
			if (resolvedClass != null) {
				string identifier = resolverContext.Expression;
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
