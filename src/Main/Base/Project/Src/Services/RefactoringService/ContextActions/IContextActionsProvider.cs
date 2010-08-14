// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Provides <see cref="ContextAction" />s to appear in a popup on the left side of the editor.
	/// </summary>
	public interface IContextActionsProvider
	{
		/// <summary>
		/// Gets actions available for current line of the editor.
		/// </summary>
		IEnumerable<IContextAction> GetAvailableActions(EditorContext context);
		
		/// <summary>
		/// Is this provider enabled by user?
		/// </summary>
		bool IsVisible { get; set; }
	}
}
