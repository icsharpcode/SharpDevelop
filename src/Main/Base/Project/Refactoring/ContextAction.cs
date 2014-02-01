// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.NRefactory;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Base class for implementing one context action.
	/// Useful for implementing <see cref="IContextActionProvider" /> that provides just one action - common scenario.
	/// </summary>
	public abstract class ContextAction : IContextActionProvider, IContextAction
	{
		public virtual string ID {
			get { return GetType().FullName; }
		}
		
		public abstract string DisplayName { get; }
		
		public virtual string GetDisplayName(EditorRefactoringContext context)
		{
			return DisplayName;
		}
		
		public virtual string Category {
			get { return string.Empty; }
		}
		
		public bool IsVisible { get; set; }
		
		public virtual bool AllowHiding {
			get { return true; }
		}
		
		/// <summary>
		/// Gets whether this context action is available in the given context.
		/// </summary>
		/// <remarks><inheritdoc cref="IContextActionsProvider.GetAvailableActionsAsync"/></remarks>
		public abstract Task<bool> IsAvailableAsync(EditorRefactoringContext context, CancellationToken cancellationToken);
		
		public abstract void Execute(EditorRefactoringContext context);
		
		async Task<IContextAction[]> IContextActionProvider.GetAvailableActionsAsync(EditorRefactoringContext context, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (await IsAvailableAsync(context, cancellationToken).ConfigureAwait(false))
				return new IContextAction[] { this };
			else
				return new IContextAction[0];
		}
		
		IContextActionProvider IContextAction.Provider {
			get { return this; }
		}
	}
}
