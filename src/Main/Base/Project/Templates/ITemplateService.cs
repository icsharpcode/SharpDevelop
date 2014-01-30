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
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Templates
{
	[SDService("SD.Templates")]
	public interface ITemplateService
	{
		/// <summary>
		/// Gets the list of template categories that are available in the 'new project' or 'new file' dialog.
		/// </summary>
		IReadOnlyList<TemplateCategory> TemplateCategories { get; }
		
		/// <summary>
		/// Reloads the <see cref="TemplateCategories"/>.
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
		/// <exception cref="TemplateLoadException">Invalid file format</exception>
		/// <exception cref="IOException">Error reading the file</exception>
		TemplateBase LoadTemplate(FileName fileName);
		
		/// <summary>
		/// Loads a file template (.xft or .xpt file) from a text reader.
		/// </summary>
		/// <param name="stream">The stream containing the .xft/.xpt file.</param>
		/// <param name="fileSystem">File system used to open referenced input files.
		/// This should usually be a <see cref="ReadOnlyChrootFileSystem"/> so that the file template
		/// can use relative paths.
		/// The template will keep a reference to the file system instance and use it to read the referenced input files when the template is used.
		/// </param>
		/// <returns><see cref="FileTemplate"/> or <see cref="ProjectTemplate"/> instance.</returns>
		/// <exception cref="TemplateLoadException">Invalid file format</exception>
		/// <exception cref="IOException">Error reading from the stream</exception>
		TemplateBase LoadTemplate(Stream stream, IReadOnlyFileSystem fileSystem);
	}
}
