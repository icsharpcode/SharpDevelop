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
