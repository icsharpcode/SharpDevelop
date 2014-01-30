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

/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: trecio
 * Data: 2009-07-07
 * Godzina: 12:26
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Linq;
using ICSharpCode.Core;
using Microsoft.Build.Construction;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Item Definition Group representation in the MSBuild project file.
	/// </summary>
	public class MSBuildItemDefinitionGroup
	{
		public MSBuildItemDefinitionGroup(MSBuildBasedProject project) 
			: this(project, "")
		{
		}
		
		public MSBuildItemDefinitionGroup(MSBuildBasedProject project, string condition)
		{
			ProjectRootElement root = project.MSBuildProjectFile;			
			group = root.ItemDefinitionGroups.SingleOrDefault(item => item.Condition == condition);
			if (group == null)
			{
				group = root.CreateItemDefinitionGroupElement();
				group.Condition = condition;
				root.AppendChild(group);
			}
		}

		public MSBuildItemDefinitionGroup(MSBuildBasedProject project, ConfigurationAndPlatform configuration)
			: this(project, configuration.ToCondition())
		{
		}

		/// <summary>
		/// Creates a new element in the item definition group.
		/// </summary>
		/// <param name="element">element name</param>
		public ProjectItemDefinitionElement AddElement(string element)
		{
			return group.AddItemDefinition(element);
		}

		/// <summary>
		/// Gets a metadata value of an element. 
		/// </summary>
		/// <param name="element">element name</param>
		/// <param name="name">metadata name</param>
		/// <returns>read metadata value, or null if metadata item with given name doesn't exist</returns>
		public string GetElementMetadata(string element, string name)
		{
			ProjectItemDefinitionElement elem;
			elem = (ProjectItemDefinitionElement)group.Children.SingleOrDefault(item => item is ProjectItemDefinitionElement && ((ProjectItemDefinitionElement)item).ItemType == element);
			if (elem == null) return null;
			ProjectMetadataElement metadataElement;
			metadataElement = elem.Metadata.SingleOrDefault(item => item.Name == name);
			if (metadataElement == null) return null;
			return metadataElement.Value;
		}

		/// <summary>
		/// Sets a metadata value of an element. Creates the element if it doesn't exist.
		/// </summary>
		/// <param name="element">element name</param>
		/// <param name="name">metadata name</param>
		/// <param name="value">metadata value</param>
		public void SetElementMetadata(string element, string name, string value)
		{
			ProjectItemDefinitionElement elem;
			elem = (ProjectItemDefinitionElement)group.Children.SingleOrDefault(item => item is ProjectItemDefinitionElement && ((ProjectItemDefinitionElement)item).ItemType == element);
			if (elem == null)
				elem = AddElement(element);
			ProjectMetadataElement metadataElement;
			metadataElement = elem.Metadata.SingleOrDefault(item => item.Name == name);
			if (metadataElement != null)
				metadataElement.Value = value;
			else
				elem.AddMetadata(name, value);
        	}

		ProjectItemDefinitionGroupElement group;
	}
}
