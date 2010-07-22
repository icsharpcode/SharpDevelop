// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Provides context actions available for current line of the editor.
	/// </summary>
	public sealed class ContextActionsService
	{
		private static ContextActionsService instance = new ContextActionsService();
		
		public static ContextActionsService Instance {
			get {
				return instance;
			}
		}
		
		List<IContextActionsProvider> providers;
		
		private ContextActionsService()
		{
			this.providers = AddInTree.BuildItems<IContextActionsProvider>("/SharpDevelop/ViewContent/AvalonEdit/ContextActionProviders", null, false);
		}
		
		/// <summary>
		/// Gets actions available for current line of the editor.
		/// </summary>
		public IEnumerable<IContextAction> GetAvailableActions(ITextEditor editor)
		{
			var editorAST = new EditorASTProvider(editor);
			// could run providers in parallel
			foreach (var provider in this.providers) {
				foreach (var action in provider.GetAvailableActions(editorAST)) {
					yield return action;
				}
			}
		}
	}
}
