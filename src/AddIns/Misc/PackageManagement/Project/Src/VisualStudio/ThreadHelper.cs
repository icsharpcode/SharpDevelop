// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace Microsoft.VisualStudio.Shell
{
	public class ThreadHelper : MarshalByRefObject
	{
		static readonly ThreadHelper threadHelper = new ThreadHelper();
		
		ThreadHelper()
		{
		}
		
		public static ThreadHelper Generic {
			get { return threadHelper; }
		}
		
		public T Invoke<T>(Func<T> method)
		{
			return WorkbenchSingleton.SafeThreadFunction<T>(method);
		}
	}
}
