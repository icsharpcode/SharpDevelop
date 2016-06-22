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
