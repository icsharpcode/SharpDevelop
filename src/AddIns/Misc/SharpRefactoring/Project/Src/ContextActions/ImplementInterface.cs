// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.Generic;
using System.Windows;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Description of ImplementInterface.
	/// </summary>
	public class ImplementInterfaceProvider : IContextActionsProvider
	{
		public IEnumerable<IContextAction> GetAvailableActions(ITextEditor editor)
		{
			var currentLine = editor.Document.GetLine(editor.Caret.Line);
			yield break;
		}
	}
	
	public class ImplementInterfaceAction : IContextAction
	{
		public string Title {
			get { return "Dummy implement interface"; }
		}
		
		public void Execute()
		{
			MessageBox.Show("Dummy implement interface");
		}
	}
}
