// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Scripting;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler;
using IronPython.Compiler.Ast;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Walks the python parse tree.
	/// </summary>
	public class PythonAstWalker : PythonWalker
	{
		DefaultCompilationUnit compilationUnit;
		DefaultClass currentClass;
		string ns;
		
		/// <summary>
		/// All classes in a file take the namespace of the filename. 
		/// </summary>
		public PythonAstWalker(IProjectContent projectContent, string fileName)
		{
			compilationUnit = new DefaultCompilationUnit(projectContent);
			compilationUnit.FileName = fileName;
			ns = Path.GetFileNameWithoutExtension(fileName);
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
			DefaultClass c = new DefaultClass(compilationUnit, GetFullyQualifiedClassName(node));
			c.Region = GetRegion(node);
			c.BodyRegion = GetBodyRegion(node.Body, node.Header);
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
				DomRegion bodyRegion = GetBodyRegion(node.Body, node.Header);
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
		/// Gets the body region for a class or a method.
		/// </summary>
		/// <remarks>
		/// Note that SharpDevelop line numbers are zero based but the
		/// DomRegion values are one based. IronPython columns and lines are one based.
		/// </remarks>
		/// <param name="body">The body statement.</param>
		/// <param name="header">The location of the header. This gives the end location for the
		/// method or class definition up to the colon.</param>
		DomRegion GetBodyRegion(Statement body, SourceLocation header)
		{
			// Add one so the region starts from just after the colon.
			return new DomRegion(header.Line, header.Column + 1, body.End.Line, body.End.Column);			
		}
		
		/// <summary>
		/// Gets the region of the scope statement (typically a ClassDefinition). 
		/// </summary>
		/// <remarks>
		/// A class region includes the body.
		/// </remarks>
		DomRegion GetRegion(ScopeStatement statement)
		{
			return new DomRegion(statement.Start.Line, statement.Start.Column, statement.End.Line, statement.End.Column);
		}
		
		/// <summary>
		/// Gets the region of a method. This does not include the body.
		/// </summary>
		DomRegion GetMethodRegion(FunctionDefinition node)
		{
			return new DomRegion(node.Start.Line, node.Start.Column, node.Header.Line, node.Header.Column + 1);
		}
		
		/// <summary>
		/// Looks for any base types for the class defined in the
		/// list of expressions and adds them to the class.
		/// </summary>
		void AddBaseTypes(IClass c, IList<Expression> baseTypes)
		{
			foreach (Expression expression in baseTypes) {
				NameExpression nameExpression = expression as NameExpression;
				//FieldExpression fieldExpression = expression as FieldExpression;
				if (nameExpression != null) {
					AddBaseType(c, nameExpression.Name.ToString());
				//} else if (fieldExpression != null) {
				//	AddBaseType(c, fieldExpression.Name.ToString());
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
		IParameter[] ConvertParameters(Parameter[] parameters)
		{
			List<IParameter> convertedTarameters = new List<IParameter>();

			// Ignore first parameter since this is the "self" parameter.
			for (int i = 1; i < parameters.Length; ++i) {
				DefaultParameter parameter = new DefaultParameter(parameters[i].Name.ToString(), null, new DomRegion());
				convertedTarameters.Add(parameter);
			}
			return convertedTarameters.ToArray();
		}
		
		
		/// <summary>
		/// Adds the namespace to the class name taken from the class definition.
		/// </summary>
		string GetFullyQualifiedClassName(ClassDefinition classDef)
		{
			return String.Concat(ns, ".", classDef.Name.ToString());
		}
	}
}
