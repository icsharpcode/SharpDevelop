// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageAction
	{
		void Execute();
		bool HasPackageScriptsToRun();
		IPackageScriptRunner PackageScriptRunner { get; set; }
	}
}
