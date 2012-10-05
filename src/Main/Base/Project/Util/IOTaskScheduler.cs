// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Scheduler for IO-intensive tasks.
	/// </summary>
	public class IOTaskScheduler
	{
		static readonly CustomThreadPoolTaskScheduler scheduler = new CustomThreadPoolTaskScheduler(
			Math.Min(Environment.ProcessorCount, 2));
		
		static readonly TaskFactory factory = new TaskFactory(scheduler);
		
		public static TaskScheduler Scheduler {
			get { return scheduler; }
		}
		
		public static TaskFactory Factory {
			get { return factory; }
		}
	}
}
