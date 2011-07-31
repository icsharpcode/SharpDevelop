// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// One editor Context action.
	/// </summary>
	public interface IContextAction
	{
		/// <summary>
		/// Title displayed in the context actions popup.
		/// </summary>
		string Title { get; }
		/// <summary>
		/// Executes this action. Called when this action is selected from the context actions popup.
		/// </summary>
		void Execute();
	}
}
