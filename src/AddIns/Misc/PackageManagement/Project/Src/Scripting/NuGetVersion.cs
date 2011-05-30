// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public static class NuGetVersion
	{
		static readonly Version version;
		
		static NuGetVersion()
		{
			AssemblyName name = typeof(PackageSource).Assembly.GetName();
			version = name.Version;
		}
		
		public static Version Version {
			get { return version; }
		}
	}
}
