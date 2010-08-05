// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
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
		
		public abstract bool IsEnabled(EditorContext context);
		
		public abstract void Execute(EditorContext context);
		
		public IEnumerable<IContextAction> GetAvailableActions(EditorContext context)
		{
			this.context = context;
			if (this.IsEnabled(context))
				yield return this;
		}
		
		EditorContext context;		
		public void Execute()
		{
			Execute(this.context);
		}
		
		public virtual string Id {
			get { return this.GetType().FullName; }
		}
	}
}
