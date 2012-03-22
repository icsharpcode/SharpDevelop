// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Provides a set of refactoring <see cref="ContextAction" />s.
	/// </summary>
	public interface IContextActionProvider
	{
		/// <summary>
		/// Unique identifier for the context actions provider; used to hide context actions
		/// that were disabled by the user.
		/// </summary>
		string ID { get; }
		
		/// <summary>
		/// Gets the title for this context action provider - should be similar to the DisplayName
		/// of the generated context actions. Displayed in the options dialog for disabling providers.
		/// </summary>
		string DisplayName { get; }
		
		/// <summary>
		/// Gets a category for this context action provider - used to group context actions
		/// in the options dialog.
		/// </summary>
		string Category { get; }
		
		/// <summary>
		/// Gets actions available for current line of the editor.
		/// </summary>
		/// <remarks>
		/// This method gets called on the GUI thread. The method implementation should use
		/// 'Task.Run()' to move the implementation onto a background thread.
		/// </remarks>
		Task<IContextAction[]> GetAvailableActionsAsync(EditorRefactoringContext context, CancellationToken cancellationToken);
		
		/// <summary>
		/// Gets whether the user is allowed to disable this provider.
		/// </summary>
		bool AllowHiding { get; }
		
		/// <summary>
		/// Is this provider enabled by user?
		/// </summary>
		bool IsVisible { get; set; }
	}
}
