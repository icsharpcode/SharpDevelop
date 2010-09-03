// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Base class for implementing custom context actions.
	/// </summary>
	public abstract class ContextAction : IContextActionsProvider, IContextAction
	{
		public abstract string Title { get; }
		
		public bool IsVisible { get; set; }
		
		public abstract bool IsAvailable(EditorContext context);
		
		public abstract void Execute(EditorContext context);
		
		public IEnumerable<IContextAction> GetAvailableActions(EditorContext context)
		{
			this.Context = context;
			if (this.IsAvailable(context))
				yield return this;
		}
		
		public EditorContext Context { get; private set; }
		
		public void Execute()
		{
			Execute(this.Context);
		}
	}
}
