// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		/// Pass <c>null</c> to use the default list (<see cref="ITemplateService.FileTemplates"/>)</param>
		/// <returns>
		/// Returns the FileTemplateResult; or null if no file was created.
		/// </returns>
		FileTemplateResult ShowNewFileDialog(IProject project, DirectoryName directory, IEnumerable<FileTemplate> templates = null);
	}
}
