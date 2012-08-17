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
		Solution solution;
		const string ExtensibilityGlobalsSectionName = "ExtensibilityGlobals";
		List<SD.SolutionItem> nonPersistedSolutionItems = new List<SD.SolutionItem>();
		
		public SolutionExtensibilityGlobals(Solution solution)
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
				UpdateOrCreateSolutionItem(name, value as string);
			}
		}
		
		void ThrowNoVariableExistsException(string name)
		{
			throw new ArgumentException("Variable name does not exist.", name);
		}
		
		internal bool ItemExists(string name)
		{
			return GetItemFromSolutionOrNonPersistedItems(name) != null;
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
		
		SD.SolutionItem GetItemFromSolution(string name)
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
		
		void UpdateOrCreateSolutionItem(string name, string value)
		{
			if (UpdateItemInSolution(name, value)) {
				return;
			}
			
			UpdateOrCreateNonPersistedSolutionItem(name, value);
		}
		
		bool UpdateItemInSolution(string name, string value)
		{
			SD.SolutionItem item = GetItemFromSolution(name);
			if (item != null) {
				item.Location = value;
				solution.Save();
				return true;
			}
			return false;
		}
		
		void UpdateOrCreateNonPersistedSolutionItem(string name, string value)
		{
			SD.SolutionItem item = GetNonPersistedSolutionItem(name);
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
		
		internal bool ItemExistsInSolution(string name)
		{
			return GetItemFromSolution(name) != null;
		}
		
		internal void AddItemToSolution(string name)
		{
			if (ItemExistsInSolution(name)) {
				return;
			}
			
			SD.SolutionItem item = GetNonPersistedSolutionItem(name);
			nonPersistedSolutionItems.Remove(item);
			SD.ProjectSection section = GetOrCreateExtensibilityGlobalsSection();
			section.Items.Add(item);
			solution.Save();
		}
		
		SD.ProjectSection GetOrCreateExtensibilityGlobalsSection()
		{
			SD.ProjectSection section = GetExtensibilityGlobalsSection();
			if (section != null) {
				return section;
			}
			var newSection = new SD.ProjectSection(ExtensibilityGlobalsSectionName, "postSolution");
			solution.Sections.Add(newSection);
			return newSection;
		}
		
		internal void RemoveItemFromSolution(string name)
		{
			SD.SolutionItem item = GetItemFromSolution(name);
			if (item != null) {
				RemoveItemFromSolution(item);
			}
		}
			
		void RemoveItemFromSolution(SD.SolutionItem item)
		{
			SD.ProjectSection section = GetExtensibilityGlobalsSection();
			section.Items.Remove(item);
			nonPersistedSolutionItems.Add(item);
			solution.Save();
		}
	}
}
