// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.PackageManagement
{
	public class ProjectTemplatePackagesSettingsFileName
	{
		string directory;
		
		public ProjectTemplatePackagesSettingsFileName(IPropertyService propertyService)
		{
			directory = Path.Combine(propertyService.ConfigDirectory, "templates");
		}
		
		public string Directory {
			get { return directory; }
		}
	}
}
