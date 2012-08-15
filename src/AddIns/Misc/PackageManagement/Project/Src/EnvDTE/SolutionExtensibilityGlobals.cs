// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class SolutionExtensibilityGlobals
	{
		SD.Solution solution;
		const string ExtensibilityGlobalsSectionName = "ExtensibilityGlobals";
		List<SD.SolutionItem> nonPersistedSolutionItems = new List<SD.SolutionItem>();
		
		public SolutionExtensibilityGlobals(SD.Solution solution)
		{
			this.solution = solution;
		}
		
		public object this[string name] {
			get {
				SD.SolutionItem item = GetItemFromSolutionOrNonPersistedItems(name);
				if (item == null) {
					ThrowNoVariableExistsException(name);
				}
				return item.Location;
			}
			set {
				GetOrCreateSolutionItem(name, value as string);
			}
		}
		
		void ThrowNoVariableExistsException(string name)
		{
			throw new ArgumentException("Variable name does not exist.", name);
		}
		
		SD.SolutionItem GetItemFromSolutionOrNonPersistedItems(string name)
		{
			SD.SolutionItem item = GetNonPersistedSolutionItem(name);
			if (item != null) {
				return item;
			}
			return GetItemFromSolution(name);
		}
		
		SD.SolutionItem GetNonPersistedSolutionItem(string name)
		{
			return GetMatchingSolutionItem(nonPersistedSolutionItems, name);
		}
		
		SD.SolutionItem GetMatchingSolutionItem(List<SD.SolutionItem> items, string name)
		{
			return items.SingleOrDefault(item => IsMatchIgnoringCase(item.Name, name));
		}
		
		internal SD.SolutionItem GetItemFromSolution(string name)
		{
			SD.ProjectSection section = GetExtensibilityGlobalsSection();
			if (section != null) {
				return GetMatchingSolutionItem(section.Items, name);
			}
			return null;
		}
		
		SD.ProjectSection GetExtensibilityGlobalsSection()
		{
			return solution.Sections.SingleOrDefault(section => section.Name == ExtensibilityGlobalsSectionName);
		}
		
		bool IsMatchIgnoringCase(string a, string b)
		{
			return String.Equals(a, b, StringComparison.OrdinalIgnoreCase);
		}
		
		void GetOrCreateSolutionItem(string name, string value)
		{
			SD.SolutionItem item = GetItemFromSolution(name);
			if (item != null) {
				item.Location = value;
			} else {
				CreateNonPersistedSolutionItem(name, value);
			}
		}
		
		void CreateNonPersistedSolutionItem(string name, string value)
		{
			var item = new SD.SolutionItem(name, value);
			nonPersistedSolutionItems.Add(item);
		}
	}
}
