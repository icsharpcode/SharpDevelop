// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AddInManager2.Model;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
	/// <summary>
	/// IAddInManagerSettings fake implementation.
	/// </summary>
	public class FakeAddInManagerSettings : IAddInManagerSettings
	{
		public string[] PackageRepositories
		{
			get;
			set;
		}

		public bool ShowPreinstalledAddIns
		{
			get;
			set;
		}
		
		public bool ShowPrereleases
		{
			get;
			set;
		}
		
		public bool AutoSearchForUpdates
		{
			get;
			set;
		}
	}
}
