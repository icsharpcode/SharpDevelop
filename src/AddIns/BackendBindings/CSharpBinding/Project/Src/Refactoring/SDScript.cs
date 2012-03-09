// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Refactoring change script.
	/// </summary>
	sealed class SDScript : Script
	{
		readonly ITextEditor editor;
		
		public SDScript(ITextEditor editor, SDRefactoringContext context) : base(context)
		{
			this.editor = editor;
		}
		
		public static void RunActions (IList<ICSharpCode.NRefactory.CSharp.Refactoring.Action> actions, Script script)
		{
			for (int i = 0; i < actions.Count; i++) {
				actions [i].Perform (script);
				var replaceChange = actions [i] as TextReplaceAction;
				if (replaceChange == null)
					continue;
				for (int j = 0; j < actions.Count; j++) {
					if (i == j)
						continue;
					var change = actions [j] as TextReplaceAction;
					if (change == null)
						continue;
					if (replaceChange.Offset >= 0 && change.Offset >= 0) {
						if (replaceChange.Offset < change.Offset) {
							change.Offset -= replaceChange.RemovedChars;
							if (!string.IsNullOrEmpty (replaceChange.InsertedText))
								change.Offset += replaceChange.InsertedText.Length;
						} else if (replaceChange.Offset < change.Offset + change.RemovedChars) {
							change.RemovedChars = Math.Max (0, change.RemovedChars - replaceChange.RemovedChars);
							change.Offset = replaceChange.Offset + (!string.IsNullOrEmpty (replaceChange.InsertedText) ? replaceChange.InsertedText.Length : 0);
						}
					}
				}
			}
		}
		
		public override void Dispose ()
		{
			using (editor.Document.OpenUndoGroup ()) {
				RunActions (changes, this);
			}
		}
		
		public override void InsertWithCursor(string operation, AstNode node, InsertPosition defaultPosition)
		{
			throw new NotImplementedException();
		}
		
		internal class SDNodeOutputAction : NodeOutputAction
		{
			IDocument doc;
			
			public SDNodeOutputAction(IDocument doc, int offset, int removedChars, NodeOutput output) : base (offset, removedChars, output)
			{
				if (doc == null)
					throw new ArgumentNullException ("doc");
				if (output == null)
					throw new ArgumentNullException ("output");
				this.doc = doc;
			}
			
			public override void Perform (Script script)
			{
				doc.Replace (Offset, RemovedChars, NodeOutput.Text);
			}
		}
	}
}
