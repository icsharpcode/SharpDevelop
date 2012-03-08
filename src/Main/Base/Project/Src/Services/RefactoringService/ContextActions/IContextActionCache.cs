// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Temporary data shared by multiple Context actions. Stored in EditorContext.GetCached().
	/// </summary>
	public interface IContextActionCache
	{
		void Initialize(EditorContext context);
	}
}
