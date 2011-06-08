// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Concurrent;

namespace ICSharpCode.PackageManagement
{
	public class PackageActionsToRun
	{
		ConcurrentQueue<ProcessPackageAction> actions = new ConcurrentQueue<ProcessPackageAction>();
		
		public bool GetNextAction(out ProcessPackageAction action)
		{
			return actions.TryDequeue(out action);
		}
		
		public void AddAction(ProcessPackageAction action)
		{
			actions.Enqueue(action);
		}
	}
}
