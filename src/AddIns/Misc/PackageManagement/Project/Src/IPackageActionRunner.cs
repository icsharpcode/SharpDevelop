// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageActionRunner
	{
		void Run(ProcessPackageAction action);
		void Run(IEnumerable<ProcessPackageAction> actions);
	}
}
