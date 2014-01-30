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
using System.Collections.Specialized;
using System.Linq;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class SolutionExtensibilityGlobals
	{
		Solution solution;
		const string ExtensibilityGlobalsSectionName = "ExtensibilityGlobals";
		List<SolutionSectionItem> nonPersistedSolutionItems = new List<SolutionSectionItem>();
		
		public SolutionExtensibilityGlobals(Solution solution)
		{
			this.solution = solution;
		}
		
		public object this[string name] {
			get {
				SolutionSectionItem item = GetItemFromSolutionOrNonPersistedItems(name);
				if (item == null) {
					ThrowNoVariableExistsException(name);
				}
				return item.Value;
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
		
		SolutionSectionItem GetItemFromSolutionOrNonPersistedItems(string name)
		{
			SolutionSectionItem item = GetNonPersistedSolutionItem(name);
			if (item != null) {
				return item;
			}
			return GetItemFromSolution(name);
		}
		
		SolutionSectionItem GetNonPersistedSolutionItem(string name)
		{
			return GetMatchingSolutionItem(nonPersistedSolutionItems, name);
		}
		
		SolutionSectionItem GetMatchingSolutionItem(List<SolutionSectionItem> items, string name)
		{
			return items.SingleOrDefault(item => IsMatchIgnoringCase(item.Name,name));
		}
		
		SolutionSectionItem GetMatchingSolutionItem(SD.SolutionSection section, string name)
		{
			string matchedName = section.Keys.SingleOrDefault(key => IsMatchIgnoringCase(key, name));
			if (matchedName != null) {
				return new SolutionSectionItem(section, matchedName);
			}
			return null;
		}
		
		SolutionSectionItem GetItemFromSolution(string name)
		{
			SD.SolutionSection section = GetExtensibilityGlobalsSection();
			if (section != null) {
				return GetMatchingSolutionItem(section, name);
			}
			return null;
		}
		
		SD.SolutionSection GetExtensibilityGlobalsSection()
		{
			return solution.Sections.SingleOrDefault(section => section.SectionName == ExtensibilityGlobalsSectionName);
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
			SolutionSectionItem item = GetItemFromSolution(name);
			if (item != null) {
				item.Value = value;
				solution.Save();
				return true;
			}
			return false;
		}
		
		void UpdateOrCreateNonPersistedSolutionItem(string name, string value)
		{
			SolutionSectionItem item = GetNonPersistedSolutionItem(name);
			if (item != null) {
				item.Value = value;
			} else {
				CreateNonPersistedSolutionItem(name, value);
			}
		}
		
		void CreateNonPersistedSolutionItem(string name, string value)
		{
			var item = new SolutionSectionItem(name, value);
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
			
			SolutionSectionItem item = GetNonPersistedSolutionItem(name);
			nonPersistedSolutionItems.Remove(item);
			SD.SolutionSection section = GetOrCreateExtensibilityGlobalsSection();
			section.Add(item.Name, item.Value);
			solution.Save();
		}
		
		SD.SolutionSection GetOrCreateExtensibilityGlobalsSection()
		{
			SD.SolutionSection section = GetExtensibilityGlobalsSection();
			if (section != null) {
				return section;
			}
			var newSection = new SD.SolutionSection(ExtensibilityGlobalsSectionName, "postSolution");
			solution.Sections.Add(newSection);
			return newSection;
		}
		
		internal void RemoveItemFromSolution(string name)
		{
			SolutionSectionItem item = GetItemFromSolution(name);
			if (item != null) {
				RemoveItemFromSolution(item);
			}
		}
			
		void RemoveItemFromSolution(SolutionSectionItem item)
		{
			SD.SolutionSection section = GetExtensibilityGlobalsSection();
			section.Remove(item.Name);
			nonPersistedSolutionItems.Add(item);
			solution.Save();
		}
	}
}
