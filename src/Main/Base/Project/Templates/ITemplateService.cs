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
		IEnumerable<FileTemplate> FileTemplates { get; }
		
		FileTemplate LoadFileTemplate(Stream stream);
	}
}
