// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Concurrent;

namespace ICSharpCode.PackageManagement
{
	public class PackageActionsToRun
	{
		ConcurrentQueue<IPackageAction> actions = new ConcurrentQueue<IPackageAction>();
		
		public bool GetNextAction(out IPackageAction action)
		{
			return actions.TryDequeue(out action);
		}
		
		public void AddAction(IPackageAction action)
		{
			actions.Enqueue(action);
		}
	}
}
