// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ConfigurationManager : MarshalByRefObject, global::EnvDTE.ConfigurationManager
	{
		Configuration activeConfiguration;
		
		public ConfigurationManager(Project project)
		{
			activeConfiguration = new Configuration(project);
		}
		
		public global::EnvDTE.Configuration ActiveConfiguration {
			get { return activeConfiguration; }
		}
	}
}
