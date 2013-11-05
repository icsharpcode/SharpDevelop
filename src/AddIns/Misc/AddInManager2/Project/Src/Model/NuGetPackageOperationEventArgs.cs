// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.AddInManager2.Model
{
	/// <summary>
	/// Event data for operations related to NuGet packages.
	/// </summary>
	public class NuGetPackageOperationEventArgs : EventArgs
	{
		public NuGetPackageOperationEventArgs(IPackage package)
		{
			Package = package;
		}
		
		public IPackage Package
		{
			get;
			private set;
		}
	}
}
