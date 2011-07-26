// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementPropertyService : IPropertyService
	{
		public string DataDirectory {
			get { return PropertyService.DataDirectory; }
		}
		
		public string ConfigDirectory {
			get { return PropertyService.ConfigDirectory; }
		}
	}
}
