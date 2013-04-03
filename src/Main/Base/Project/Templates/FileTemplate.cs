// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Templates
{
	public abstract class FileTemplate : TemplateBase
	{
		/// <summary>
		/// Gets whether this template is available for the specified project.
		/// </summary>
		/// <param name="project">The project to which the new file should be added.
		/// Can be <c>null</c> when creating a file outside of a project.</param>
		public virtual bool IsVisible(IProject project)
		{
			return true;
		}
		
		/// <summary>
		/// Proposes a name for the new file.
		/// </summary>
		/// <param name="basePath">The target directory where the new file will be placed.
		/// <c>null</c> if an untitled file will be created.</param>
		public abstract string SuggestFileName(DirectoryName basePath);
		
		/// <summary>
		/// Creates an object with the customizable properties for this template.
		/// The NewFileDialog allows changing the properties of this object using a property grid.
		/// </summary>
		public virtual object CreateCustomizationObject()
		{
			return null;
		}
		
		/// <summary>
		/// Instanciates the template, writes the new files to disk, and adds them to the project.
		/// </summary>
		public abstract FileTemplateResult Create(FileTemplateOptions options);
		
		public virtual void RunActions(FileTemplateResult result)
		{
		}
	}
}
