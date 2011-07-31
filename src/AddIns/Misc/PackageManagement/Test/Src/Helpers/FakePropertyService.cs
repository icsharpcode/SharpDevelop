// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;

namespace PackageManagement.Tests.Helpers
{
	public class FakePropertyService : IPropertyService
	{
		public FakePropertyService()
		{
			ConfigDirectory = String.Empty;
			DataDirectory = String.Empty;
		}
		
		public string DataDirectory { get; set; }
		public string ConfigDirectory { get; set; }
	}
}
