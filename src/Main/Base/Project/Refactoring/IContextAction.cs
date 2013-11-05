// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading.Tasks;
using System.Windows;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Context action.
	/// </summary>
	public interface IContextAction
	{
		/// <summary>
		/// Gets the provider that was used to create this action.
		/// </summary>
		IContextActionProvider Provider { get; }
		
		/// <summary>
		/// Returns the name displayed in the context action's popup.
		/// </summary>
		/// <param name="context">Refactoring context that can be used by the context action to create the display name.</param>
		string GetDisplayName(EditorRefactoringContext context);
		
		/// <summary>
		/// Executes this action. Called when this action is selected from the context actions popup.
		/// </summary>
		void Execute(EditorRefactoringContext context);
	}
}
