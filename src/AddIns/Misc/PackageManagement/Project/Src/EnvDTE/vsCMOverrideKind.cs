// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public enum vsCMOverrideKind
	{
		vsCMOverrideKindNone     = 0,
		vsCMOverrideKindAbstract = 1,
		vsCMOverrideKindVirtual  = 2,
		vsCMOverrideKindOverride = 4,
		vsCMOverrideKindNew      = 8,
		vsCMOverrideKindSealed   = 16
	}
}
