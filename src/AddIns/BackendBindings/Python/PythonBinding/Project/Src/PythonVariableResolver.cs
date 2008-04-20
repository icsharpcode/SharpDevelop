// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler;
using IronPython.Compiler.Ast;
using IronPython.Runtime; 

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Determines the type of a variable.
	/// </summary>
	public class PythonVariableResolver : AstWalker
	{
		string variableName = String.Empty;
		string typeName;
		AssignStatement currentAssignStatement;
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
		public string Resolve(string variableName, string code)
		{
			if (code != null) {
				PythonCompilerSink sink = new PythonCompilerSink();
				CompilerContext context = new CompilerContext(null, sink);
				Parser parser = Parser.FromString(null, context, code);
				Statement statement = parser.ParseFileInput();
				return Resolve(variableName, statement);
			}
			return null;
		}
				
		public override bool Walk(AssignStatement node)
		{
			currentAssignStatement = node;
			foundVariableAssignment = false;
			return base.Walk(node);
		}		
		
		public override bool Walk(NameExpression node)
		{
			if (currentAssignStatement != null) {
				string nodeName = node.Name.ToString();
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
			FieldExpression fieldExpression = null;
			List<string> names = new List<string>();
			do {
				NameExpression nameExpression = node as NameExpression;
				fieldExpression = node as FieldExpression;
				SymbolId symbol = new SymbolId(0);
				if (fieldExpression != null) {
					symbol = fieldExpression.Name;
					node = fieldExpression.Target;
				} else if (nameExpression != null) {
					symbol = nameExpression.Name;
				}
				names.Add(symbol.ToString());
			} while (fieldExpression != null);
			
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
		
		string Resolve(string variableName, Statement statement)
		{
			this.variableName = variableName;
			statement.Walk(this);
			return TypeName;		
		}
	}
}
