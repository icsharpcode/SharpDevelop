// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.AddInManager2.Model
{
	public class AcceptLicensesEventArgs : EventArgs
	{
		public AcceptLicensesEventArgs(IEnumerable<IPackage> packages)
		{
			this.Packages = packages;
		}
		
		public IEnumerable<IPackage> Packages
		{
			get;
			private set;
		}
		
		public bool IsAccepted
		{
			get;
			set;
		}
	}
}
