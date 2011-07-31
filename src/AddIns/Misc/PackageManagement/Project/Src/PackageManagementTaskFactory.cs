// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementTaskFactory : ITaskFactory
	{
		public ITask<TResult> CreateTask<TResult>(
			Func<TResult> function,
			Action<ITask<TResult>> continueWith)
		{
			return new PackageManagementTask<TResult>(function, continueWith);
		}
	}
}
