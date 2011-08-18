// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public interface IMvcViewFileGenerator
	{
		MvcTextTemplateLanguage TemplateLanguage { get; set; }
		MvcTextTemplateType TemplateType { get; set; }
		IMvcProject Project { get; set; }
		
		void GenerateFile(MvcViewFileName fileName);
	}
}
