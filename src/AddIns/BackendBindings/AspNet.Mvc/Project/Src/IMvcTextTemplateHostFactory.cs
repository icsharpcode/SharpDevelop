// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextTemplating;

namespace ICSharpCode.AspNet.Mvc
{
	public interface IMvcTextTemplateHostFactory
	{
		IMvcTextTemplateHost CreateMvcTextTemplateHost(
			IMvcProject project,
			IMvcTextTemplateHostAppDomain appDomain);
	}
}
