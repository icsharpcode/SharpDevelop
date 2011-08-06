// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcTextTemplateHostFactory : IMvcTextTemplateHostFactory
	{
		public IProject ProjectPassedToCreateMvcTextTemplateHost;
		public FakeMvcTextTemplateHost FakeMvcTextTemplateHost = new FakeMvcTextTemplateHost();
		
		public IMvcTextTemplateHost CreateMvcTextTemplateHost(IProject project)
		{
			ProjectPassedToCreateMvcTextTemplateHost = project;
			return FakeMvcTextTemplateHost;
		}
	}
}
