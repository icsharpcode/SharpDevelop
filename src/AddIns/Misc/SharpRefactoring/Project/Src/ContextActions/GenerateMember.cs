// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Description of GenerateMember.
	/// </summary>
	public class GenerateMemberProvider : IContextActionsProvider
	{
		public IEnumerable<IContextAction> GetAvailableActions(EditorContext editorContext)
		{
			var generateCodeAction = GenerateCode.GetContextAction(editorContext.SymbolUnderCaret, editorContext.Editor);
			if (generateCodeAction != null)
				yield return generateCodeAction;
		}
	}
}