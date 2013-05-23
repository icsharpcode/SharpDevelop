// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Templates
{
	public class FileTemplateResult
	{
		readonly FileTemplateOptions options;
		
		public FileTemplateResult(FileTemplateOptions options)
		{
			if (options == null)
				throw new ArgumentNullException("options");
			this.options = options;
		}
		
		/// <summary>
		/// Gets the options used to instanciate the template.
		/// </summary>
		public FileTemplateOptions Options {
			get { return options; }
		}
		
		IList<FileName> newFiles = new List<FileName>();
		
		/// <summary>
		/// Gets the list of newly created files.
		/// </summary>
		public IList<FileName> NewFiles {
			get { return newFiles; }
		}
	}
}
