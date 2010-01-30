// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.Scripting;
using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler;
using IronPython.Compiler.Ast;
using IronPython.Runtime;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Walks the python parse tree.
	/// </summary>
	public class PythonAstWalker : PythonWalker
	{
		DefaultCompilationUnit compilationUnit;
		DefaultClass currentClass;
		DefaultClass globalClass;
		string currentNamespace;
		
		/// <summary>
		/// All classes in a file take the namespace of the filename. 
		/// </summary>
		public PythonAstWalker(IProjectContent projectContent, string fileName)
		{
			compilationUnit = new DefaultCompilationUnit(projectContent);
			compilationUnit.FileName = fileName;
			currentNamespace = Path.GetFileNameWithoutExtension(fileName);
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
			if (node.Body == null) {
				return true;
			}
			
			bool ignoreFirstMethodParameter = true;
			IClass c = currentClass;
			if (currentClass == null) {
				// Walking a global method.
				CreateGlobalClass();
				c = globalClass;
				ignoreFirstMethodParameter = false;
			}

			// Create method.
			string methodName = node.Name;
			DomRegion bodyRegion = GetBodyRegion(node.Body, node.Header);
			DomRegion region = GetMethodRegion(node);
			
			DefaultMethod method;
			if (methodName == "__init__") {
				method = new Constructor(ModifierEnum.Public, region, bodyRegion, c);
			} else {
				method = new DefaultMethod(methodName, new DefaultReturnType(c), ModifierEnum.Public, region, bodyRegion, c);
			}
			foreach (IParameter parameter in ConvertParameters(node.Parameters, ignoreFirstMethodParameter)) {
				method.Parameters.Add(parameter);
			}
			c.Methods.Add(method);
			return true;
		}
		
		/// <summary>
		/// Walks an import statement and adds it to the compilation unit's
		/// Usings.
		/// </summary>
		public override bool Walk(ImportStatement node)
		{
			PythonImport import = new PythonImport(compilationUnit.ProjectContent, node);
			compilationUnit.UsingScope.Usings.Add(import);
			return false;
		}
		
		public override bool Walk(FromImportStatement node)
		{
			PythonFromImport import = new PythonFromImport(compilationUnit.ProjectContent, node);
			compilationUnit.UsingScope.Usings.Add(import);
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
			int columnAfterColonCharacter = header.Column + 1;
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
				MemberExpression memberExpression = expression as MemberExpression;
				if (nameExpression != null) {
					AddBaseType(c, nameExpression.Name);
				} else if (memberExpression != null) {
					AddBaseType(c, PythonControlFieldExpression.GetMemberName(memberExpression));
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
		/// <remarks>If the parameters belong to a class method then the first
		/// "self" parameter can be ignored.</remarks>
		IParameter[] ConvertParameters(IList<Parameter> parameters, bool ignoreFirstParameter)
		{
			List<IParameter> convertedParameters = new List<IParameter>();

			int startingIndex = 0;
			if (ignoreFirstParameter) {
				startingIndex = 1;
			}
			
			for (int i = startingIndex; i < parameters.Count; ++i) {				
				DefaultParameter parameter = new DefaultParameter(parameters[i].Name, null, new DomRegion());
				convertedParameters.Add(parameter);
			}
			return convertedParameters.ToArray();
		}
		
		
		/// <summary>
		/// Adds the namespace to the class name taken from the class definition.
		/// </summary>
		string GetFullyQualifiedClassName(ClassDefinition classDef)
		{
			return String.Concat(currentNamespace, ".", classDef.Name);
		}
		
		/// <summary>
		/// Creates the dummy class that is used to hold global methods.
		/// </summary>
		void CreateGlobalClass()
		{
			if (globalClass == null) {
				globalClass = new DefaultClass(compilationUnit, currentNamespace);
				compilationUnit.Classes.Add(globalClass);
			}
		}
	}
}
