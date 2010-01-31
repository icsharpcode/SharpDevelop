// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
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
		
		public List<ICompletionEntry> GetCompletionItems()
		{
			return GetCompletionItems(String.Empty);
		}
		
		public List<ICompletionEntry> GetCompletionItems(string subNamespace)
		{
			List<ICompletionEntry> items = projectContent.GetNamespaceContents(subNamespace);
			
			if (String.IsNullOrEmpty(subNamespace)) {
				AddStandardPythonModules(items);
			}
			return items;
		}
		
		void AddStandardPythonModules(List<ICompletionEntry> items)
		{
			items.AddRange(standardPythonModules);
		}
		
		public List<ICompletionEntry> GetCompletionItemsFromModule(string module)
		{
			PythonStandardModuleType type = standardPythonModules.GetModuleType(module);
			if (type != null) {
				return GetCompletionItemsFromModule(type);
			}
			return projectContent.GetNamespaceContents(module);
		}
		
		List<ICompletionEntry> GetCompletionItemsFromModule(PythonStandardModuleType type)
		{
			PythonModuleCompletionItems moduleItems = PythonModuleCompletionItemsFactory.Create(type);
			List<ICompletionEntry> items = new List<ICompletionEntry>(moduleItems);
			return items;
		}
	}
}
