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
using System.Text;

using ICSharpCode.SharpDevelop.Dom;
using IronRuby.Builtins;
using IronRuby.Compiler;
using IronRuby.Compiler.Ast;
using IronRuby.Runtime;

namespace ICSharpCode.RubyBinding
{
	public class RubyAstWalker : Walker
	{
		DefaultCompilationUnit compilationUnit;
		DefaultClass currentClass;
		DefaultClass globalClass;
		
		/// <summary>
		/// All classes in a file take the namespace of the filename. 
		/// </summary>
		public RubyAstWalker(IProjectContent projectContent, string fileName)
		{
			compilationUnit = new DefaultCompilationUnit(projectContent);
			compilationUnit.FileName = fileName;
		}
		
		/// <summary>
		/// Returns the compilation unit created after the Walk method
		/// has been called.
		/// </summary>
		public ICompilationUnit CompilationUnit {
			get { return compilationUnit; }
		}
		
		protected override void Walk(MethodCall node)
		{
			if (node.MethodName == "require") {
				if (!node.Arguments.IsEmpty) {
					string requireString = GetRequireString(node.Arguments.Expressions);
					if (requireString != null) {
						AddUsing(requireString);
					}
				}
			}
			base.Walk(node);
		}
		
		string GetRequireString(Expression[] expressions)
		{
			foreach (Expression expression in expressions) {
				StringConstructor stringCtor = expression as StringConstructor;
				if (stringCtor != null) {
					return (string)RubyCodeDeserializer.Deserialize(stringCtor);
				}
			}
			return null;
		}
		
		void AddUsing(string requireString)
		{
			string assemblyName = GetAssemblyName(requireString);
			
			DefaultUsing defaultUsing = new DefaultUsing(compilationUnit.ProjectContent);
			defaultUsing.Usings.Add(assemblyName);
			compilationUnit.UsingScope.Usings.Add(defaultUsing);
		}
		
		string GetAssemblyName(string requireString)
		{
			int index = requireString.IndexOf(',');
			if (index > 0) {
				return requireString.Substring(0, index);
			}
			return requireString;
		}
		
		protected override void Walk(ModuleDeclaration node)
		{
			globalClass = CreateClass(node);
			
			currentClass = globalClass;
			base.Walk(node);
			currentClass = null;
		}		
		
		protected override void Walk(ClassDeclaration node)
		{
			DefaultClass c = CreateClass(node);
			AddBaseType(c, node);
			
			// Walk through all the class items.
			currentClass = c;
			base.Walk(node);
			currentClass = null;	
		}
		
		protected override void Walk(MethodDeclaration node)
		{
			IClass c = currentClass;
			if (currentClass == null) {
				// Walking a global method.
				CreateGlobalClass();
				c = globalClass;
			}

			// Create method.
			string methodName = node.Name;
			DomRegion region = GetMethodRegion(node);
			DomRegion bodyRegion = GetMethodBodyRegion(node.Body.Location, node.Parameters.Location.End);
			
			DefaultMethod method;
			if (methodName == "initialize") {
				method = new Constructor(ModifierEnum.Public, region, bodyRegion, c);
			} else {
				method = new DefaultMethod(methodName, new DefaultReturnType(c), ModifierEnum.Public, region, bodyRegion, c);
			}
			foreach (IParameter parameter in ConvertParameters(node.Parameters)) {
				method.Parameters.Add(parameter);
			}
			c.Methods.Add(method);
		}
				
		/// <summary>
		/// Converts a SourceSpan to a DomRegion. 
		/// </summary>
		DomRegion GetRegion(SourceSpan location)
		{
			return new DomRegion(location.Start.Line, location.Start.Column, location.End.Line, location.End.Column);;
		}
		
		/// <summary>
		/// Gets the region of a method. This does not include the body.
		/// </summary>
		DomRegion GetMethodRegion(MethodDeclaration node)
		{
			return new DomRegion(node.Location.Start.Line, node.Location.Start.Column, node.Parameters.Location.End.Line, node.Parameters.Location.End.Column);
		}
		
		/// <summary>
		/// The class body region starts from the end of qualified name of the class.
		/// </summary>
		/// <param name="headerEnd">Location of the end of the qualified name of the class.</param>
		DomRegion GetClassBodyRegion(SourceSpan bodyLocation, SourceLocation headerEnd)
		{
			return new DomRegion(headerEnd.Line, headerEnd.Column, bodyLocation.End.Line, bodyLocation.End.Column);
		}
		
		/// <summary>
		/// The method body region starts from the end of method parameters.
		/// </summary>
		/// <param name="parametersEnd">Location of the end of the parameters.</param>
		DomRegion GetMethodBodyRegion(SourceSpan bodyLocation, SourceLocation parametersEnd)
		{
			return new DomRegion(parametersEnd.Line, parametersEnd.Column, bodyLocation.End.Line, bodyLocation.End.Column);
		}
	
		/// <summary>
		/// Converts from Ruby AST expressions to parameters.
		/// </summary>
		IParameter[] ConvertParameters(Parameters parameters)
		{
			List<IParameter> convertedParameters = new List<IParameter>();
			foreach (LocalVariable variable in GetParameterVariables(parameters)) {
				DefaultParameter parameter = new DefaultParameter(variable.Name, null, new DomRegion());
				convertedParameters.Add(parameter);
			}
			return convertedParameters.ToArray();
		}
		
		/// <summary>
		/// Converts the Parameters object into a list of LocalVariables
		/// </summary>
		List<LocalVariable> GetParameterVariables(Parameters parameters)
		{
			List<LocalVariable> variables = new List<LocalVariable>();
			if (parameters.Mandatory != null) {
				foreach (LocalVariable variable in parameters.Mandatory) {
					variables.Add(variable);
				}
			}
			
			if (parameters.Optional != null) {
				foreach (SimpleAssignmentExpression expression in parameters.Optional) {
					LocalVariable optionalVariable = expression.Left as LocalVariable;
					if (optionalVariable != null) {
						variables.Add(optionalVariable);
					}
				}
			}
			return variables;			
		}
		
		/// <summary>
		/// Creates the dummy class that is used to hold global methods.
		/// </summary>
		void CreateGlobalClass()
		{
			if (globalClass == null) {
				globalClass = new DefaultClass(compilationUnit, Path.GetFileNameWithoutExtension(compilationUnit.FileName));
				compilationUnit.Classes.Add(globalClass);
			}
		}
		
		DefaultClass CreateClass(ModuleDeclaration node)
		{
			DefaultClass c = new DefaultClass(compilationUnit, node.QualifiedName.Name);
			c.Region = GetRegion(node.Location);
			c.BodyRegion = GetClassBodyRegion(node.Body.Location, node.QualifiedName.Location.End);
			compilationUnit.Classes.Add(c);
			return c;
		}
		
		/// <summary>
		/// Adds the named base type to the class.
		/// </summary>
		void AddBaseType(IClass c, ClassDeclaration classDec)
		{
			string name = RubyComponentWalker.GetBaseClassName(classDec);
			c.BaseTypes.Add(new SearchClassReturnType(c.ProjectContent, c, 0, 0, name, 0));
		}
		
		string GetFullyQualifiedName(ConstantVariable variable)
		{
			StringBuilder name = new StringBuilder();
			bool firstName = true;
			while (variable != null) {
				if (!firstName) {
					name.Insert(0, '.');
				}
				name.Insert(0, variable.Name);
				variable = variable.Qualifier as ConstantVariable;
				firstName = false;
			}
			return name.ToString();
		}
	}
}
