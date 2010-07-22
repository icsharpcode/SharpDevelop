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
	/// Description of ImplementAbstractClass.
	/// </summary>
	public class ImplementAbstractClassProvider : IContextActionsProvider
	{
		public IEnumerable<IContextAction> GetAvailableActions(EditorASTProvider editorAST)
		{
			yield break;
		}
	}
	
	public class ImplementAbstractClassAction : IContextAction
	{
		public string Title {
			get { return "Dummy implement abstract class"; }
		}
		
		public void Execute()
		{
			MessageBox.Show("Dummy implement abstract class");
		}
	}
}
