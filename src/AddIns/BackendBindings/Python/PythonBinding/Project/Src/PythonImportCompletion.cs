// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		public static readonly NamespaceEntry ImportAll = new NamespaceEntry("*");
		
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
			List<ICompletionEntry> items = GetCompletionItemsFromStandardPythonModules(module);
			if (items == null) {
				items = projectContent.GetNamespaceContents(module);
			}
			items.Add(ImportAll);
			return items;
		}
		
		List<ICompletionEntry> GetCompletionItemsFromStandardPythonModules(string module)
		{
			PythonStandardModuleType type = standardPythonModules.GetModuleType(module);
			if (type != null) {
				return GetCompletionItemsFromModule(type);
			}
			return null;
		}
		
		List<ICompletionEntry> GetCompletionItemsFromModule(PythonStandardModuleType type)
		{
			PythonModuleCompletionItems moduleItems = PythonModuleCompletionItemsFactory.Create(type);
			List<ICompletionEntry> items = new List<ICompletionEntry>(moduleItems);
			return items;
		}
	}
}
