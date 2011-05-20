// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageScriptRunner : IPackageScriptRunner
	{
		List<IPackageScript> ScriptsRun = new List<IPackageScript>();
		
		public IPackageScript FirstScriptRun {
			get { return ScriptsRun[0]; }
		}
		
		public void Run(IPackageScript script)
		{
			ScriptsRun.Add(script);
		}
	}
}
