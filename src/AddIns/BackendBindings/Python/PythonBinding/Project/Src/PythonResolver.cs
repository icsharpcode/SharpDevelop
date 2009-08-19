// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Python Resolver.
	/// </summary>
	public class PythonResolver : IResolver
	{
		IProjectContent projectContent;
		IClass callingClass;
		ICompilationUnit compilationUnit;
		static string[] standardModuleNames;
		
		public PythonResolver()
		{
		}
		
		public ResolveResult Resolve(ExpressionResult expressionResult, ParseInformation parseInfo, string fileContent)
		{
			Console.WriteLine("Resolve: " + expressionResult.Expression);
			
			if (String.IsNullOrEmpty(fileContent)) {
				return null;
			}
			
			// Get the calling class and member.
			if (!GetCallingMember(parseInfo, expressionResult.Region)) {
				return null;
			}
			
			// Search for a type.
			IClass matchingClass = GetClass(expressionResult.Expression);
			if (matchingClass != null) {
				return new TypeResolveResult(null, null, matchingClass);
			}
			
			// Search for a method.
			MethodGroupResolveResult resolveResult = GetMethodResolveResult(expressionResult.Expression);
			if (resolveResult != null) {
				return resolveResult;
			}
			
			// Search for a local variable.
			LocalResolveResult localResolveResult = GetLocalVariable(expressionResult.Expression, parseInfo.CompilationUnit.FileName, fileContent);
			if (localResolveResult != null) {
				return localResolveResult;
			}
			
			// Search for a namespace.
			string namespaceExpression = GetNamespaceExpression(expressionResult.Expression);
			if (projectContent.NamespaceExists(namespaceExpression)) {
				return new NamespaceResolveResult(null, null, namespaceExpression);
			}
			return null;
		}
				
		/// <summary>
		/// Called when Ctrl+Space is entered by the user.
		/// </summary>
		public ArrayList CtrlSpace(int caretLine, int caretColumn, ParseInformation parseInfo, string fileContent, ExpressionContext context)
		{
			ArrayList results = new ArrayList();
			ICompilationUnit compilationUnit = GetCompilationUnit(parseInfo, true);
			if (compilationUnit != null && compilationUnit.ProjectContent != null) {
				if (context == ExpressionContext.Importable) {
					// Add namespace contents for import code completion.
					compilationUnit.ProjectContent.AddNamespaceContents(results, String.Empty, compilationUnit.ProjectContent.Language, true);
					
					// Add built-in module names.
					results.AddRange(GetStandardPythonModuleNames());
				} else {
					// Add namespace contents.
					CtrlSpaceResolveHelper.AddImportedNamespaceContents(results, compilationUnit, null);
				}
			}
			return results;
		}
								
		/// <summary>
		/// Gets the compilation unit for the specified parse information.
		/// </summary>
		ICompilationUnit GetCompilationUnit(ParseInformation parseInfo, bool mostRecent)
		{
			if (parseInfo != null) {
				return parseInfo.CompilationUnit;
			}
			return null;
		}		
		
		/// <summary>
		/// Determines the class and member at the specified
		/// line and column in the specified file.
		/// </summary>
		bool GetCallingMember(ParseInformation parseInfo, DomRegion region)
		{
			compilationUnit = GetCompilationUnit(parseInfo, true);
			if (compilationUnit == null) {
				return false;
			}
			
			projectContent = compilationUnit.ProjectContent;
			if (projectContent != null) {
				ICompilationUnit bestCompilationUnit = GetCompilationUnit(parseInfo, false);
				callingClass = GetCallingClass(compilationUnit, bestCompilationUnit, region);
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Gets the calling class at the specified.
		/// </summary>
		IClass GetCallingClass(ICompilationUnit mostRecentCompilationUnit, ICompilationUnit bestCompilationUnit, DomRegion region)
		{
			// Try the most recent compilation unit first
			IClass c = GetCallingClass(mostRecentCompilationUnit, region);
			if (c != null) {
				return c;
			}
			
			// Try the best compilation unit.
			if (bestCompilationUnit != null && bestCompilationUnit.ProjectContent != null) {
				IClass oldClass = GetCallingClass(bestCompilationUnit, region);
				if (oldClass != null) {
					return oldClass;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the calling class at the specified line and column.
		/// </summary>
		IClass GetCallingClass(ICompilationUnit compilationUnit, DomRegion region)
		{
			if (compilationUnit.Classes.Count > 0) {
				return compilationUnit.Classes[0];
			}
			return null;
		}
		
		/// <summary>
		/// Removes the "import " or "from " part of a namespace expression if it exists.
		/// </summary>
		static string GetNamespaceExpression(string expression)
		{
			string ns = GetNamespaceExpression("import ", expression);
			if (ns == null) {
				ns = GetNamespaceExpression("from ", expression);
			}
			
			if (ns != null) {
				return ns;
			}
			return expression;
		}

		/// <summary>
		/// Removes the "import " or "from " part of a namespace expression if it exists.
		/// </summary>
		static string GetNamespaceExpression(string importString, string expression)
		{
			int index = expression.IndexOf(importString, StringComparison.OrdinalIgnoreCase);
			if (index >= 0) {
				return expression.Substring(index + importString.Length);
			}
			return null;
		}
		
		/// <summary>
		/// Finds the specified class.
		/// </summary>
		IClass GetClass(string name)
		{
			// Try the project content first. This will
			// match if the name is a fully qualified class name.
			IClass matchedClass = projectContent.GetClass(name, 0);
			if (matchedClass != null) {
				return matchedClass;
			}
			
			// Try the imported classes now. This will
			// match on a partial name (i.e. without the namespace
			// prefix).
			return GetImportedClass(name);
		}
		
		/// <summary>
		/// Looks in the imported namespaces for a class that 
		/// matches the class name. The class name searched for is not fully
		/// qualified.
		/// </summary>
		/// <param name="name">The unqualified class name.</param>
		IClass GetImportedClass(string name)
		{
			foreach (Object o in GetImportedTypes()) {
				IClass c = o as IClass;
				if (c != null && IsSameClassName(name, c.Name)) {
					return c;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Determines whether the two type names are the same.
		/// </summary>
		static bool IsSameClassName(string name1, string name2)
		{
			return name1 == name2;
		}
		
		/// <summary>
		/// Returns an array of the types that are imported by the
		/// current compilation unit.
		/// </summary>
		ArrayList GetImportedTypes()
		{
			ArrayList types = new ArrayList();
			CtrlSpaceResolveHelper.AddImportedNamespaceContents(types, this.compilationUnit, this.callingClass);
			return types;
		}
		
		/// <summary>
		/// Gets the standard python module names and caches the result.
		/// </summary>
		static string[] GetStandardPythonModuleNames()
		{
			if (standardModuleNames == null) {
				StandardPythonModules modules = new StandardPythonModules();
				standardModuleNames = modules.GetNames();
			}
			return standardModuleNames;
		}
		
		/// <summary>
		/// Tries to resolve a method in the expression.
		/// </summary>
		MethodGroupResolveResult GetMethodResolveResult(string expression)
		{
			// Remove last part of the expression and try to
			// find this class.
			PythonExpressionFinder expressionFinder = new PythonExpressionFinder();
			string className = expressionFinder.RemoveLastPart(expression);
			if (!String.IsNullOrEmpty(className)) {
				IClass matchingClass = GetClass(className);
				if (matchingClass != null) {
					string methodName = GetMethodName(expression);
					return new MethodGroupResolveResult(null, null, matchingClass.DefaultReturnType, methodName);
				}
			}
			return null;
		}
		
		/// <summary>
		/// Gets the method name from the expression.
		/// </summary>
		static string GetMethodName(string expression)
		{	
			int index = expression.LastIndexOf('.');
			return expression.Substring(index + 1);
		}
		
		/// <summary>
		/// Tries to find the type that matches the local variable name.
		/// </summary>
		LocalResolveResult GetLocalVariable(string expression, string fileName, string fileContent)
		{
//			PythonVariableResolver resolver = new PythonVariableResolver();
//			string typeName = resolver.Resolve(expression, fileName, fileContent);
//			if (typeName != null) {
//				IClass resolvedClass = GetClass(typeName);
//				if (resolvedClass != null) {
//					DefaultClass dummyClass = new DefaultClass(DefaultCompilationUnit.DummyCompilationUnit, "Global");
//					DefaultMethod dummyMethod = new DefaultMethod(dummyClass, String.Empty);
//					DefaultField.LocalVariableField field = new DefaultField.LocalVariableField(resolvedClass.DefaultReturnType, expression, DomRegion.Empty, dummyClass);
//					return new LocalResolveResult(dummyMethod, field);
//				}
//			}
			return null;
		}
	}
}
