// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Concurrent;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageScriptsToRun
	{
		ConcurrentQueue<IPackageScript> scripts = new ConcurrentQueue<IPackageScript>();
		
		public IPackageScript GetNextScript()
		{
			IPackageScript script = null;
			if (GetNextScript(out script)) {
				return script;
			}
			return null;
		}
		
		public bool GetNextScript(out IPackageScript script)
		{
			return scripts.TryDequeue(out script);
		}
		
		public void AddScript(IPackageScript script)
		{
			scripts.Enqueue(script);
		}
	}
}
