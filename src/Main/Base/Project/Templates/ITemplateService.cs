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
		/// Loads a file template (.xft file) from a stream.
		/// </summary>
		FileTemplate LoadFileTemplate(Stream stream);
	}
}
