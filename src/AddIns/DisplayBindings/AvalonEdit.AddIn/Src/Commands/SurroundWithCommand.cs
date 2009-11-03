// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Linq;

using ICSharpCode.AvalonEdit.AddIn.Snippets;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn.Commands
{
	/// <summary>
	/// Description of SurroundWithCommand
	/// </summary>
	public class SurroundWithCommand : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			
			if (provider == null)
				return;
			
			CodeSnippetGroup group = SnippetManager.Instance.FindGroup(Path.GetExtension(provider.TextEditor.FileName));
			
			if (group == null)
				return;
			
			DefaultCompletionItemList list = new DefaultCompletionItemList();
			
			list.Items.AddRange(group.Snippets.Select(item => item.CreateCompletionItem(provider.TextEditor)));
			
			new CodeSnippetCompletionWindow(provider.TextEditor, list).Show();
		}
	}
}
