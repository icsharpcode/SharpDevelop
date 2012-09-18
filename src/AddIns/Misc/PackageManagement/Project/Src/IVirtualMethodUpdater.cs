// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement
{
	/// <summary>
	/// Used to update a method's source code and make the method virtual.
	/// </summary>
	public interface IVirtualMethodUpdater
	{
		void MakeMethodVirtual();
	}
}
