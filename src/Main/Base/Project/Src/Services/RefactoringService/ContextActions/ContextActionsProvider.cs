// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of ContextActionsProvider.
	/// </summary>
	public abstract class ContextActionsProvider : IContextActionsProvider
	{
		public bool IsVisible { get; set; }
		
		public abstract IEnumerable<IContextAction> GetAvailableActions(EditorContext context);
	}
}
