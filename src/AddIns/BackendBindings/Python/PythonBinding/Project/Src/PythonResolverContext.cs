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

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonResolverContext
	{
		ICompilationUnit compilationUnit;
		IProjectContent projectContent;
		IClass callingClass;
		string fileContent;
		ExpressionResult expressionResult;
		
		public PythonResolverContext(ParseInformation parseInfo)
			: this(parseInfo, String.Empty)
		{
		}
		
		public PythonResolverContext(ParseInformation parseInfo, string fileContent)
			: this(parseInfo, new ExpressionResult(), fileContent)
		{
		}
		
		public PythonResolverContext(ParseInformation parseInfo, ExpressionResult expressionResult, string fileContent)
		{
			this.fileContent = fileContent;
			this.expressionResult = expressionResult;
			GetCompilationUnit(parseInfo);
			GetProjectContent();
			GetCallingMember();
		}
		
		void GetCompilationUnit(ParseInformation parseInfo)
		{
			if (parseInfo != null) {
				compilationUnit = parseInfo.CompilationUnit;
			}
		}
		
		void GetProjectContent()
		{
			if (compilationUnit != null) {
				projectContent = compilationUnit.ProjectContent;
			}
		}
		
		/// <summary>
		/// Determines the class and member at the specified
		/// line and column in the specified file.
		/// </summary>
		void GetCallingMember()
		{
			if (projectContent != null) {
				GetCallingClass();
			}
		}
				
		/// <summary>
		/// Gets the calling class at the specified line and column.
		/// </summary>
		void GetCallingClass()
		{
			if (compilationUnit.Classes.Count > 0) {
				callingClass = compilationUnit.Classes[0];
			}
		}
		
		public string FileContent {
			get { return fileContent; }
		}
		
		public IProjectContent ProjectContent {
			get { return projectContent; }
		}
		
		public ExpressionResult ExpressionResult {
			get { return expressionResult; }
		}
		
		public MemberName CreateExpressionMemberName()
		{
			return new MemberName(Expression);
		}
		
		public string Expression {
			get { return expressionResult.Expression; }
		}
		
		public ExpressionContext ExpressionContext {
			get { return expressionResult.Context; }
		}
		
		public DomRegion ExpressionRegion {
			get { return expressionResult.Region; }
		}
		
		public bool HasProjectContent {
			get { return projectContent != null; }
		}
		
		public IClass CallingClass {
			get { return callingClass; }
		}
		
		public bool HasCallingClass {
			get { return callingClass != null; }
		}
		
		public bool NamespaceExistsInProjectReferences(string name)
		{
			return projectContent.NamespaceExists(name);
		}
		
		public bool PartialNamespaceExistsInProjectReferences(string name)
		{
			foreach (IProjectContent referencedContent in projectContent.ThreadSafeGetReferencedContents()) {
				if (PartialNamespaceExists(referencedContent, name)) {
					return true;
				}
			}
			return false;
		}
		
		bool PartialNamespaceExists(IProjectContent projectContent, string name)
		{
			foreach (string namespaceReference in projectContent.NamespaceNames) {
				if (namespaceReference.StartsWith(name)) {
					return true;
				}
			}
			return false;
		}
		
		public IClass GetClass(string fullyQualifiedName)
		{
			return projectContent.GetClass(fullyQualifiedName, 0);
		}
		
		/// <summary>
		/// Returns an array of the types that are imported by the
		/// current compilation unit.
		/// </summary>
		public List<ICompletionEntry> GetImportedTypes()
		{
			List<ICompletionEntry> types = new List<ICompletionEntry>();
			CtrlSpaceResolveHelper.AddImportedNamespaceContents(types, compilationUnit, callingClass);
			return types;
		}
		
		public bool HasImport(string name)
		{
			foreach (IUsing u in compilationUnit.UsingScope.Usings) {
				foreach (string ns in u.Usings) {
					if (name == ns) {
						return true;
					}
				}
			}
			return false;
		}
		
		/// <summary>
		/// Looks in the imported namespaces for a class that 
		/// matches the class name. The class name searched for is not fully
		/// qualified.
		/// </summary>
		/// <param name="name">The unqualified class name.</param>
		public IClass GetImportedClass(string name)
		{
			foreach (object obj in GetImportedTypes()) {
				IClass c = obj as IClass;
				if (c != null) {
					if (IsSameClassName(name, c.Name)) {
						return c;
					}
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
		/// Looks for the module name where the specified identifier is imported from.
		/// </summary>
		public string GetModuleForImportedName(string name)
		{
			foreach (IUsing u in compilationUnit.UsingScope.Usings) {
				PythonFromImport pythonFromImport = u as PythonFromImport;
				if (pythonFromImport != null) {
					if (pythonFromImport.IsImportedName(name)) {
						return pythonFromImport.Module;
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Converts a name into the correct identifier name based on any from import as statements.
		/// </summary>
		public string UnaliasImportedName(string name)
		{
			foreach (IUsing u in compilationUnit.UsingScope.Usings) {
				PythonFromImport pythonFromImport = u as PythonFromImport;
				if (pythonFromImport != null) {
					string actualName = pythonFromImport.GetOriginalNameForAlias(name);
					if (actualName != null) {
						return actualName;
					}
				}
			}
			return name;
		}
		
		/// <summary>
		/// Converts the module name to its original unaliased value if it exists.
		/// </summary>
		public string UnaliasImportedModuleName(string  name)
		{
			foreach (IUsing u in compilationUnit.UsingScope.Usings) {
				PythonImport pythonImport = u as PythonImport;
				if (pythonImport != null) {
					string actualName = pythonImport.GetOriginalNameForAlias(name);
					if (actualName != null) {
						return actualName;
					}
				}
			}
			return name;
		}
		
		public string[] GetModulesThatImportEverything()
		{
			List<string> modules = new List<string>();
			foreach (IUsing u in compilationUnit.UsingScope.Usings) {
				PythonFromImport pythonFromImport = u as PythonFromImport;
				if (pythonFromImport != null) {
					if (pythonFromImport.ImportsEverything) {
						modules.Add(pythonFromImport.Module);
					}
				}
			}
			return modules.ToArray();
		}
		
		public bool IsStartOfDottedModuleNameImported(string fullDottedModuleName)
		{
			return FindStartOfDottedModuleNameInImports(fullDottedModuleName) != null;
		}
		
		public string FindStartOfDottedModuleNameInImports(string fullDottedModuleName)
		{
			MemberName memberName = new MemberName(fullDottedModuleName);
			while (memberName.HasName) {
				string partialNamespace = memberName.Type;
				if (HasImport(partialNamespace)) {
					return partialNamespace;
				}
				memberName = new MemberName(partialNamespace);
			}
			return null;
		}
		
		public string UnaliasStartOfDottedImportedModuleName(string fullDottedModuleName)
		{
			string startOfModuleName = FindStartOfDottedModuleNameInImports(fullDottedModuleName);
			if (startOfModuleName != null) {
				return UnaliasStartOfDottedImportedModuleName(startOfModuleName, fullDottedModuleName);
			}
			return fullDottedModuleName;
		}
		
		string UnaliasStartOfDottedImportedModuleName(string startOfModuleName, string fullModuleName)
		{
			string unaliasedStartOfModuleName = UnaliasImportedModuleName(startOfModuleName);
			return unaliasedStartOfModuleName + fullModuleName.Substring(startOfModuleName.Length);
		}
		
		public bool HasDottedImportNameThatStartsWith(string importName)
		{
			string dottedImportNameStartsWith = importName + ".";
			foreach (IUsing u in compilationUnit.UsingScope.Usings) {
				foreach (string ns in u.Usings) {
					if (ns.StartsWith(dottedImportNameStartsWith)) {
						return true;
					}
				}
			}
			return false;
		}
		
		public PythonResolverContext Clone(string newExpression)
		{
			ParseInformation parseInfo = new ParseInformation(compilationUnit);
			ExpressionResult newExpressionResult = new ExpressionResult(newExpression);
			return new PythonResolverContext(parseInfo, newExpressionResult, fileContent);
		}
	}
}
