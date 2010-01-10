// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Reflection;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Modules;
using IronPython.Runtime;

namespace ICSharpCode.PythonBinding
{
	public class PythonImportCompletion
	{
		IProjectContent projectContent;
		static readonly PythonStandardModules standardPythonModules = new PythonStandardModules();

		public PythonImportCompletion(IProjectContent projectContent)
		{
			this.projectContent = projectContent;
		}
		
		public ArrayList GetCompletionItems()
		{
			return GetCompletionItems(String.Empty);
		}
		
		public ArrayList GetCompletionItems(string subNamespace)
		{
			ArrayList items = projectContent.GetNamespaceContents(subNamespace);
			
			if (String.IsNullOrEmpty(subNamespace)) {
				AddStandardPythonModules(items);
			}
			return items;
		}
		
		void AddStandardPythonModules(ArrayList items)
		{
			items.AddRange(standardPythonModules);
		}
		
		public ArrayList GetCompletionItemsFromModule(string module)
		{
			PythonStandardModuleType type = standardPythonModules.GetModuleType(module);
			if (type != null) {
				return GetCompletionItemsFromModule(type);
			}
			return projectContent.GetNamespaceContents(module);
		}
		
		ArrayList GetCompletionItemsFromModule(PythonStandardModuleType type)
		{			
			PythonModuleCompletionItems moduleItems = PythonModuleCompletionItemsFactory.Create(type);
			ArrayList items = new ArrayList(moduleItems);
			return items;
		}
	}
}
