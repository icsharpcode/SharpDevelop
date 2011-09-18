// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public interface IMvcControllerFileGenerator
	{
		IMvcProject Project { get; set; }
		MvcControllerTextTemplate Template { get; set; }
		
		void GenerateFile(MvcControllerFileName fileName);
	}
}
