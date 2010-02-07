// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonResolverContext
	{
		ICompilationUnit mostRecentCompilationUnit;
		ICompilationUnit bestCompilationUnit;
		IProjectContent projectContent;
		IClass callingClass;
		
		public PythonResolverContext(ParseInformation parseInfo)
		{
			GetCompilationUnits(parseInfo);
			GetProjectContent();
		}
		
		void GetCompilationUnits(ParseInformation parseInfo)
		{
			mostRecentCompilationUnit = GetCompilationUnit(parseInfo, true);
			bestCompilationUnit = GetCompilationUnit(parseInfo, false);
		}
		
		void GetProjectContent()
		{
			if (mostRecentCompilationUnit != null) {
				projectContent = mostRecentCompilationUnit.ProjectContent;
			}
		}
		
		public ICompilationUnit MostRecentCompilationUnit {
			get { return mostRecentCompilationUnit; }
		}
		
		public IProjectContent ProjectContent {
			get { return projectContent; }
		}
		
		public bool HasProjectContent {
			get { return projectContent != null; }
		}
		
		public IClass CallingClass {
			get { return callingClass; }
		}
		
		public bool NamespaceExistsInProjectReferences(string name)
		{
			return projectContent.NamespaceExists(name);
		}
		
		public bool PartialNamespaceExistsInProjectReferences(string name)
		{
			foreach (IProjectContent referencedContent in projectContent.ReferencedContents) {
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
		
		/// <summary>
		/// Determines the class and member at the specified
		/// line and column in the specified file.
		/// </summary>
		public bool GetCallingMember(DomRegion region)
		{
			if (mostRecentCompilationUnit == null) {
				return false;
			}
			
			if (projectContent != null) {
				callingClass = GetCallingClass(mostRecentCompilationUnit, bestCompilationUnit, region);
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Gets the compilation unit for the specified parse information.
		/// </summary>
		public ICompilationUnit GetCompilationUnit(ParseInformation parseInfo, bool mostRecent)
		{
			if (parseInfo != null) {
				if (mostRecent) {
					return parseInfo.MostRecentCompilationUnit;
				}
				return parseInfo.BestCompilationUnit;
			}
			return null;
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
		
		public IClass GetClass(string fullyQualifiedName)
		{
			return projectContent.GetClass(fullyQualifiedName, 0);
		}
		
		/// <summary>
		/// Returns an array of the types that are imported by the
		/// current compilation unit.
		/// </summary>
		public ArrayList GetImportedTypes()
		{
			ArrayList types = new ArrayList();
			CtrlSpaceResolveHelper.AddImportedNamespaceContents(types, mostRecentCompilationUnit, callingClass);
			return types;
		}
		
		public bool HasImport(string name)
		{
			foreach (IUsing u in mostRecentCompilationUnit.UsingScope.Usings) {
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
				if ((c != null) && IsSameClassName(name, c.Name)) {
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
		/// Looks for the module name where the specified identifier is imported from.
		/// </summary>
		public string GetModuleForImportedName(string name)
		{
			foreach (IUsing u in mostRecentCompilationUnit.UsingScope.Usings) {
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
			foreach (IUsing u in mostRecentCompilationUnit.UsingScope.Usings) {
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
			foreach (IUsing u in mostRecentCompilationUnit.UsingScope.Usings) {
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
			foreach (IUsing u in mostRecentCompilationUnit.UsingScope.Usings) {
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
			foreach (IUsing u in mostRecentCompilationUnit.UsingScope.Usings) {
				foreach (string ns in u.Usings) {
					if (ns.StartsWith(dottedImportNameStartsWith)) {
						return true;
					}
				}
			}
			return false;
		}
	}
}
