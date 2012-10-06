// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

		public MSBuildItemDefinitionGroup(MSBuildBasedProject project, string configuration, string platform) 
			: this(project, MSBuildBasedProject.CreateCondition(configuration, platform))
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
