// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler;
using IronPython.Compiler.Ast;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Walks the python parse tree.
	/// </summary>
	public class PythonAstWalker : AstWalker
	{
		DefaultCompilationUnit compilationUnit;
		DefaultClass currentClass;
		
		public PythonAstWalker(IProjectContent projectContent)
		{
			compilationUnit = new DefaultCompilationUnit(projectContent);
		}
		
		/// <summary>
		/// Returns the compilation unit created after the Walk method
		/// has been called.
		/// </summary>
		public ICompilationUnit CompilationUnit {
			get { return compilationUnit; }
		}
		
		/// <summary>
		/// Walks the python statement returned from the parser.
		/// </summary>
		public void Walk(Statement statement)
		{
			statement.Walk(this);
		}
		
		/// <summary>
		/// Walks a class definition.
		/// </summary>
		public override bool Walk(ClassDefinition node)
		{
			DefaultClass c = new DefaultClass(compilationUnit, node.Name.ToString());
			c.Region = GetClassRegion(node);
			c.BodyRegion = GetBodyRegion(node.Body);
			AddBaseTypes(c, node.Bases);

			// Save the class.
			compilationUnit.Classes.Add(c);
			
			// Walk through all the class items.
			currentClass = c;
			node.Body.Walk(this);
			currentClass = null;
			
			return false;
		}
		
		/// <summary>
		/// Walks a function definition.
		/// </summary>
		public override bool Walk(FunctionDefinition node)
		{
			if (currentClass != null) {
				string methodName = node.Name.ToString();
				DomRegion bodyRegion = GetBodyRegion(node.Body);
				DomRegion region = GetMethodRegion(node);
				
				DefaultMethod method;
				if (methodName == "__init__") {
					method = new Constructor(ModifierEnum.Public, region, bodyRegion, currentClass);
				} else {
					method = new DefaultMethod(methodName, new DefaultReturnType(currentClass), ModifierEnum.Public, region, bodyRegion, currentClass);
				}
				foreach (IParameter parameter in ConvertParameters(node.Parameters)) {
					method.Parameters.Add(parameter);
				}
				currentClass.Methods.Add(method);
			}
			return true;
		}
		
		/// <summary>
		/// Walks an import statement and adds it to the compilation unit's
		/// Usings.
		/// </summary>
		public override bool Walk(ImportStatement node)
		{
			Console.WriteLine("Walk.Import");
			DefaultUsing newUsing = new DefaultUsing(compilationUnit.ProjectContent);
			foreach (DottedName name in node.Names) {
				Console.WriteLine("Name: " + name.MakeString());
				newUsing.Usings.Add(name.MakeString());
			}
			compilationUnit.Usings.Add(newUsing);
			return false;
		}
		
		/// <summary>
		/// Gets the body region that the AST statement node covers.
		/// </summary>
		/// <remarks>
		/// Note that SharpDevelop line numbers are zero based but the
		/// DomRegion values are one based. IronPython columns are zero
		/// based but the lines are one based.
		/// Also note that IronPython  seems to get the end column
		/// incorrect for classes.
		/// </remarks>
		DomRegion GetBodyRegion(Node node)
		{
			// Get the body location.
			Location start = node.Start;
			Location end = node.End;
			
			// Add two to the start column so the body region starts after the
			// colon character. Add nothing to the end column since IronPython gets this value
			// wrong for classes.
			Console.WriteLine("BodyRegion: Start.Col: " + start.Column + " End.Col: " + end.Column);
			return new DomRegion(start.Line, start.Column + 2, end.Line, end.Column);
		}
		
		/// <summary>
		/// Gets the region of the class declaration.
		/// </summary>
		DomRegion GetClassRegion(ScopeStatement statement)
		{
			return new DomRegion(statement.Start.Line, statement.Start.Column + 1, statement.Start.Line, statement.Body.Start.Column + 1);
		}
		
		/// <summary>
		/// Gets the region of the method declaration.
		/// </summary>
		/// <remarks>We add two to the method's body start column to jump
		/// over the colon character. Should be only adding 1 though.</remarks>
		DomRegion GetMethodRegion(ScopeStatement statement)
		{
			Console.WriteLine("MethodRegion: Start.Col: " + statement.Start.Column + " Body.Start.Col: " + statement.Body.Start.Column);
			return new DomRegion(statement.Start.Line, statement.Start.Column + 1, statement.Start.Line, statement.Body.Start.Column + 2);
		}
		
		/// <summary>
		/// Looks for any base types for the class defined in the
		/// list of expressions and adds them to the class.
		/// </summary>
		void AddBaseTypes(IClass c, IList<Expression> baseTypes)
		{
			foreach (Expression expression in baseTypes) {
				NameExpression nameExpression = expression as NameExpression;
				FieldExpression fieldExpression = expression as FieldExpression;
				if (nameExpression != null) {
					AddBaseType(c, nameExpression.Name.ToString());
				} else if (fieldExpression != null) {
					AddBaseType(c, fieldExpression.Name.ToString());
				}
			}
		}
		
		/// <summary>
		/// Adds the named base type to the class.
		/// </summary>
		void AddBaseType(IClass c, string name)
		{
			c.BaseTypes.Add(new SearchClassReturnType(c.ProjectContent, c, 0, 0, name, 0));
		}
		
		/// <summary>
		/// Converts from Python AST expressions to parameters.
		/// </summary>
		IParameter[] ConvertParameters(IList<Expression> expressions)
		{
			List<IParameter> parameters = new List<IParameter>();

			// Ignore first parameter since this is the "self" parameter.
			for (int i = 1; i < expressions.Count; ++i) {
				NameExpression expression = expressions[i] as NameExpression;
				DefaultParameter parameter = new DefaultParameter(expression.Name.ToString(), null, new DomRegion());
				parameters.Add(parameter);
			}
			return parameters.ToArray();
		}
	}
}
