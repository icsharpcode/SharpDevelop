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
