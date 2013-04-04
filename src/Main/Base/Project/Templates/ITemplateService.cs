// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Templates
{
	[SDService("SD.Templates")]
	public interface ITemplateService
	{
		/// <summary>
		/// Gets the list of file templates that are available in the 'new file' dialog.
		/// </summary>
		IEnumerable<FileTemplate> FileTemplates { get; }
		
		/// <summary>
		/// Gets the list of project templates that are available in the 'new project' dialog.
		/// </summary>
		IEnumerable<ProjectTemplate> ProjectTemplates { get; }
		
		/// <summary>
		/// Reloads the <see cref="FileTemplates"/> and <see cref="ProjectTemplates"/>.
		/// </summary>
		void UpdateTemplates();
		
		/// <summary>
		/// Gets the list of text templates.
		/// </summary>
		IEnumerable<TextTemplateGroup> TextTemplates { get; }
		
		/// <summary>
		/// Loads a file template (.xft or .xpt file) from disk.
		/// </summary>
		/// <returns><see cref="FileTemplate"/> or <see cref="ProjectTemplate"/> instance.</returns>
		TemplateBase LoadTemplate(FileName fileName);
		
		/// <summary>
		/// Loads a file template (.xft or .xpt file) from a text reader.
		/// </summary>
		/// <param name="stream">The stream containing the .xft/.xpt file.</param>
		/// <param name="fileSystem">File system used to open referenced input files.
		/// This should usually be a <see cref="ReadOnlyChrootFileSystem"/> so that the file template
		/// can use relative paths.
		/// The template will keep a reference to the file system instance and use it to create files/projects.
		/// </param>
		/// <returns><see cref="FileTemplate"/> or <see cref="ProjectTemplate"/> instance.</returns>
		TemplateBase LoadTemplate(Stream stream, IReadOnlyFileSystem fileSystem);
	}
}
