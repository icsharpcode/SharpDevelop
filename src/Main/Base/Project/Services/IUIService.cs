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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Templates;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// SharpDevelop UI service.
	/// 
	/// This service provides methods for accessing the dialogs and other UI element built into SharpDevelop.
	/// </summary>
	public interface IUIService
	{
		// TODO: consider if we should move all the methods to other services, based on which SD components
		// the dialogs belong to.
		// Or, if we don't do that, consider moving UI-related methods from the services here (e.g. FileService.BrowseForFolder)
		
		/// <summary>
		/// Shows the 'Edit Solution Configurations' dialog.
		/// </summary>
		void ShowSolutionConfigurationEditorDialog(ISolution solution);
		
		/// <summary>
		/// Shows the 'New File' dialog.
		/// </summary>
		/// <param name="project">The parent project to which the new file should be added.
		/// May be <c>null</c> to create files outside of a project.</param>
		/// <param name="directory">The target directory to which the new file should be saved.
		/// May be <c>null</c> to create an untitled file.</param>
		/// <param name="templates">The list of templates that are available in the dialog.
		/// Pass <c>null</c> to use the default list (<see cref="ITemplateService.TemplateCategories"/>)</param>
		/// <returns>Returns a <see cref="FileTemplateResult"/>; or null if no file was created.</returns>
		FileTemplateResult ShowNewFileDialog(IProject project, DirectoryName directory, IEnumerable<TemplateCategory> templates = null);
		
		/// <summary>
		/// Show the 'New Project' dialog.
		/// </summary>
		/// <param name="solutionFolder">The parent solution folder to which the new project should be added.
		/// May be <c>null</c> to create a new solution.</param>
		/// <param name="templates">The list of templates that are available in the dialog.
		/// Pass <c>null</c> to use the default list (<see cref="ITemplateService.TemplateCategories"/>)</param>
		/// <returns>Returns a <see cref="ProjectTemplateResult"/>; or null if no project was created.</returns>
		ProjectTemplateResult ShowNewProjectDialog(ISolutionFolder solutionFolder, IEnumerable<TemplateCategory> templates = null);
	}
}
