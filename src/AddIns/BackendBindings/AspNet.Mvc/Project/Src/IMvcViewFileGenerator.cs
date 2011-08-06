// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public interface IMvcViewFileGenerator
	{
		MvcTextTemplateLanguage Language { get; set; }
		IProject Project { get; set; }
		
		void GenerateView(MvcViewFileName fileName);
	}
}
