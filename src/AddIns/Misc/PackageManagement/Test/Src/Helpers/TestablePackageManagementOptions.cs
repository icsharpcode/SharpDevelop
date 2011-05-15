// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;

namespace PackageManagement.Tests.Helpers
{
	public class TestablePackageManagementOptions : PackageManagementOptions
	{
		public Properties Properties;
		public FakeSettings FakeSettings;
		
		public TestablePackageManagementOptions()
			: this(new Properties(), new FakeSettings())
		{
		}
		
		public TestablePackageManagementOptions(Properties properties, FakeSettings fakeSettings)
			: base(properties, fakeSettings)
		{
			this.Properties = properties;
			this.FakeSettings = fakeSettings;
		}
	}
}
