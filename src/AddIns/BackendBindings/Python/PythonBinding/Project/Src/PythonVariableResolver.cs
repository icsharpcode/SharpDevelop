// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

using ICSharpCode.SharpDevelop.Dom;
using IronPython;
using IronPython.Compiler;
using IronPython.Compiler.Ast;
using IronPython.Runtime;
using IronPython.Hosting;
using IronPython.Runtime.Exceptions;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Determines the type of a variable.
	/// </summary>
	public class PythonVariableResolver : PythonWalker
	{
		string variableName = String.Empty;
		string typeName;
		AssignmentStatement currentAssignStatement;
		bool foundVariableAssignment;
		
		public PythonVariableResolver()
		{
		}
		
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
		public string Resolve(string variableName, string fileName, string code)
		{
			if (code != null) {
				ScriptEngine scriptEngine = IronPython.Hosting.Python.CreateEngine();
				PythonCompilerSink sink = new PythonCompilerSink();
				SourceUnit source = DefaultContext.DefaultPythonContext.CreateFileUnit(fileName, code);
				CompilerContext context = new CompilerContext(source, new PythonCompilerOptions(), sink);
				Parser parser = Parser.CreateParser(context, new PythonOptions());
				PythonAst ast = parser.ParseFile(false);

				return Resolve(variableName, ast);
			}
			return null;
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
		static string GetTypeName(Expression node)
		{
			// Collect the names that make up the type name.
			NameExpression nameExpression = null;
			List<string> names = new List<string>();
			do {
				nameExpression = node as NameExpression;
				MemberExpression memberExpression = node as MemberExpression;
				string name = String.Empty;
				if (memberExpression != null) {
					name = memberExpression.Name;
					node = memberExpression.Target;
				} else if (nameExpression != null) {
					name = nameExpression.Name;
				}
				names.Add(name);
			} while (nameExpression == null);
			
			// Create the fully qualified type name by adding the names
			// in reverse order.
			StringBuilder typeName = new StringBuilder();
			typeName.Append(names[names.Count - 1]);
			for (int i = names.Count - 2; i >= 0; --i) {
				typeName.Append('.');
				typeName.Append(names[i]);
			}
			return typeName.ToString();
		}
		
		string Resolve(string variableName, PythonAst ast)
		{
			this.variableName = variableName;
			ast.Walk(this);
			return TypeName;		
		}
	}
}
