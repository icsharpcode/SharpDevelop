// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICSharpCode.SharpDevelop.Util
{
	/// <summary>
	/// Scheduler for IO-intensive tasks.
	/// </summary>
	public class IOTaskScheduler
	{
		// TODO: use a limited-concurrency scheduler instead
		public static TaskFactory Factory {
			get { return Task.Factory; }
		}
	}
}
