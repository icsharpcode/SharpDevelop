// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading.Tasks;
using System.Windows;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// One editor Context action.
	/// </summary>
	public interface IContextAction
	{
		/// <summary>
		/// Name displayed in the context actions popup.
		/// </summary>
		string DisplayName { get; }
		/// <summary>
		/// Executes this action. Called when this action is selected from the context actions popup.
		/// </summary>
		Task ExecuteAsync(EditorContext context);
	}
}
