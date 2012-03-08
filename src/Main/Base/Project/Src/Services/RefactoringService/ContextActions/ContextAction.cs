// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.NRefactory;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Base class for implementing one context action.
	/// Useful for implementing <see cref="IContextActionsProvider" /> that provides just one action - common scenario.
	/// </summary>
	public abstract class ContextAction : IContextActionsProvider, IContextAction
	{
		public abstract string Title { get; }
		
		public bool IsVisible { get; set; }
		
		public abstract Task<bool> IsAvailableAsync(EditorContext context, CancellationToken cancellationToken);
		
		public abstract void Execute(EditorContext context);
		
		async Task<IContextAction[]> IContextActionsProvider.GetAvailableActionsAsync(EditorContext context, CancellationToken cancellationToken)
		{
			if (await IsAvailableAsync(context, cancellationToken).ConfigureAwait(false))
				return new IContextAction[] { this };
			else
				return new IContextAction[0];
		}
	}
}
