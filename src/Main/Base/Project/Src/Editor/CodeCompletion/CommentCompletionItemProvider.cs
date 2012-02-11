// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// Data provider for code completion.
	/// </summary>
	public class CommentCompletionItemProvider : AbstractCompletionItemProvider
	{
		static readonly string[][] commentTags = {
			new string[] {"c", "marks text as code"},
			new string[] {"code", "marks text as code"},
			new string[] {"example", "A description of the code example\n(must have a <code> tag inside)"},
			new string[] {"exception cref=\"\"", "description to an exception thrown"},
			new string[] {"list type=\"\"", "A list"},
			new string[] {"listheader", "The header from the list"},
			new string[] {"item", "A list item"},
			new string[] {"inheritdoc/", "Inherit documentation from base class"},
			new string[] {"term", "A term in a list"},
			new string[] {"description", "A description to a term in a list"},
			new string[] {"para", "A text paragraph"},
			new string[] {"param name=\"\"", "A description for a parameter"},
			new string[] {"paramref name=\"\"", "A reference to a parameter"},
			new string[] {"permission cref=\"\"", ""},
			new string[] {"remarks", "Gives description for a member"},
			new string[] {"include file=\"\" path=\"\"", "Includes comments from other files"},
			new string[] {"returns", "Gives description for a return value"},
			new string[] {"see cref=\"\"", "A reference to a member"},
			new string[] {"seealso cref=\"\"", "A reference to a member in the seealso section"},
			new string[] {"summary", "A summary of the object"},
			new string[] {"value", "A description of a property"},
			new string[] {"typeparam name=\"\"", "A description for a type parameter"},
			new string[] {"typeparamref name=\"\"", "A reference to a type parameter"}
		};
		
		public override ICompletionItemList GenerateCompletionList(ITextEditor editor)
		{
			int caretLineNumber = editor.Caret.Line;
			int caretColumn     = editor.Caret.Column;
			IDocumentLine caretLine = editor.Document.GetLine(caretLineNumber);
			string lineText = caretLine.Text;
			if (!lineText.Trim().StartsWith("///", StringComparison.Ordinal)
			    && !lineText.Trim().StartsWith("'''", StringComparison.Ordinal)) 
			{
				return null;
			}
			
			DefaultCompletionItemList list = new DefaultCompletionItemList();
			foreach (string[] tag in commentTags) {
				list.Items.Add(new DefaultCompletionItem(tag[0]) { Description = tag[1] });
			}
			list.SortItems();
			return list;
		}
	}
}
