// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		/// <summary>
		/// Length of "def".
		/// </summary>
		const int MethodDefinitionLength = 3;

		DefaultCompilationUnit compilationUnit;
		DefaultClass currentClass;
		DefaultClass globalClass;
		SourceUnit sourceUnit;
		
		/// <summary>
		/// All classes in a file take the namespace of the filename. 
		/// </summary>
		public RubyAstWalker(IProjectContent projectContent, string fileName, SourceUnit sourceUnit)
		{
			compilationUnit = new DefaultCompilationUnit(projectContent);
			compilationUnit.FileName = fileName;
			this.sourceUnit = sourceUnit;
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
				if (HasArguments(node)) {
					string requireString = GetRequireString(node.Arguments.Expressions);
					if (requireString != null) {
						AddUsing(requireString);
					}
				}
			}
			base.Walk(node);
		}
		
		bool HasArguments(MethodCall node)
		{
			return (node.Arguments != null) && (!node.Arguments.IsEmpty);
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
		
		protected override void Walk(ModuleDefinition node)
		{
			globalClass = CreateClass(node);
			
			currentClass = globalClass;
			base.Walk(node);
			currentClass = null;
		}		
		
		protected override void Walk(ClassDefinition node)
		{
			DefaultClass c = CreateClass(node);
			AddBaseType(c, node);
			
			// Walk through all the class items.
			currentClass = c;
			base.Walk(node);
			currentClass = null;	
		}
		
		protected override void Walk(MethodDefinition methodDef)
		{
			IClass c = currentClass;
			if (currentClass == null) {
				// Walking a global method.
				CreateGlobalClass();
				c = globalClass;
			}
			
			// Create method.
			string methodName = methodDef.Name;
			DomRegion region = GetMethodRegion(methodDef);
			DomRegion bodyRegion = GetMethodBodyRegion(methodDef.Body.Location, region);
			
			DefaultMethod method;
			if (methodName == "initialize") {
				method = new Constructor(ModifierEnum.Public, region, bodyRegion, c);
			} else {
				method = new DefaultMethod(methodName, new DefaultReturnType(c), ModifierEnum.Public, region, bodyRegion, c);
			}
			foreach (IParameter parameter in ConvertParameters(methodDef.Parameters)) {
				method.Parameters.Add(parameter);
			}
			c.Methods.Add(method);
		}
				
		/// <summary>
		/// Converts a SourceSpan to a DomRegion. 
		/// </summary>
		DomRegion GetRegion(SourceSpan location)
		{
			return new DomRegion(location.Start.Line, location.Start.Column, location.End.Line, location.End.Column);
		}
		
		/// <summary>
		/// Gets the region of a method. This does not include the body.
		/// </summary>
		DomRegion GetMethodRegion(MethodDefinition methodDef)
		{
			SourceLocation parametersEndLocation = GetParametersEndLocation(methodDef);
			return CreateRegion(methodDef.Location.Start, parametersEndLocation);
		}
		
		SourceLocation GetParametersEndLocation(MethodDefinition methodDef)
		{
 			SourceLocation parametersEndLocation = methodDef.Parameters.Location.End;
			if (parametersEndLocation.IsValid) {
				if (ParametersEndLocationNeedsCorrecting(parametersEndLocation)) {
					parametersEndLocation = CorrectParametersEndLocation(parametersEndLocation);
				}
			} else {
				return GetParametersEndLocationBasedOnMethodNameEnd(methodDef);
 			}
			return parametersEndLocation;
		}
			
		SourceLocation GetParametersEndLocationBasedOnMethodNameEnd(MethodDefinition methodDef)
		{
			const int methodParenthesesLength = 2;
			const int spaceLength = 1;
			int methodDefinitionLength = methodDef.Name.Length + MethodDefinitionLength + methodParenthesesLength + spaceLength;
			SourceLocation methodStartLocation = methodDef.Location.Start;
			int index = methodStartLocation.Index + methodDefinitionLength;
			int column = methodStartLocation.Column + methodDefinitionLength;
			int line = methodStartLocation.Line;
			return new SourceLocation(index, line, column);
 		}
		
		/// <summary>
		/// Returns true if the IronRuby parser has not found the closing bracket for a method
		/// and moved to the next line.
		/// </summary>
		bool ParametersEndLocationNeedsCorrecting(SourceLocation parametersEndLocation)
		{
			return parametersEndLocation.Column == 1;
		}
		
		SourceLocation CorrectParametersEndLocation(SourceLocation parametersEndLocation)
		{
			int endLine = parametersEndLocation.Line - 1;
			
			string lastLineOfCodeForMethodDef = sourceUnit.GetCodeLine(endLine);
			int LengthOfNewLineCharacters = 2;
			int endColumn = lastLineOfCodeForMethodDef.Length + LengthOfNewLineCharacters + 1;
			
			return new SourceLocation(0, endLine, endColumn);
		}
		
		DomRegion CreateRegion(SourceLocation start, SourceLocation end)
		{
			int startLine = start.Line;
			int startColumn = start.Column;
			int endLine = end.Line;
			int endColumn = end.Column;
			return new DomRegion(startLine, startColumn, endLine, endColumn);
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
		DomRegion GetMethodBodyRegion(SourceSpan bodyLocation, DomRegion methodDefinitionRegion)
		{
			int startLine = methodDefinitionRegion.EndLine;
			int startColumn = methodDefinitionRegion.EndColumn;
			int endLine = bodyLocation.End.Line;
			int endColumn = bodyLocation.End.Column;
			return new DomRegion(startLine, startColumn, endLine, endColumn);
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
		
		DefaultClass CreateClass(ModuleDefinition node)
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
		void AddBaseType(IClass c, ClassDefinition classDef)
		{
			string name = RubyComponentWalker.GetBaseClassName(classDef);
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
