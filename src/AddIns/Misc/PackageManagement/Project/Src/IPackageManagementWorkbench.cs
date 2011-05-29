// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageManagementWorkbench
	{
		bool InvokeRequired { get; }
		
		void SafeThreadAsyncCall<A>(Action<A> method, A arg1);
		void SafeThreadAsyncCall<A, B>(Action<A, B> method, A arg1, B arg2);
		void CreateConsolePad();
	}
}
