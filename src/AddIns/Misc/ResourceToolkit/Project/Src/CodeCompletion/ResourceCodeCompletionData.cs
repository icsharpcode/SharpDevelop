// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace Hornung.ResourceToolkit.CodeCompletion
{
	/// <summary>
	/// Represents a code completion data entry for resource keys.
	/// </summary>
	public class ResourceCodeCompletionData : DefaultCompletionData
	{
		
		readonly IOutputAstVisitor outputVisitor;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceCodeCompletionData" /> class.
		/// </summary>
		/// <param name="key">The resource key.</param>
		/// <param name="description">The resource description.</param>
		/// <param name="outputVisitor">The NRefactory output visitor to be used to generate the inserted code. If <c>null</c>, the key is inserted literally.</param>
		public ResourceCodeCompletionData(string key, string description, IOutputAstVisitor outputVisitor)
			: base(key, description, ClassBrowserIconService.GotoArrowIndex)
		{
			this.outputVisitor = outputVisitor;
		}
		
		/// <summary>
		/// Insert the element represented by the completion data into the text
		/// editor.
		/// </summary>
		/// <param name="textArea">TextArea to insert the completion data in.</param>
		/// <param name="ch">Character that should be inserted after the completion data.
		/// \0 when no character should be inserted.</param>
		/// <returns>Returns true when the insert action has processed the character
		/// <paramref name="ch"/>; false when the character was not processed.</returns>
		public override bool InsertAction(TextArea textArea, char ch)
		{
			string insertString;
			
			if (this.outputVisitor != null) {
				PrimitiveExpression pre = new PrimitiveExpression(this.Text, this.Text);
				pre.AcceptVisitor(this.outputVisitor, null);
				insertString = this.outputVisitor.Text;
			} else {
				insertString = this.Text;
			}
			
			textArea.InsertString(insertString);
			if (ch == insertString[insertString.Length - 1]) {
				return true;
			}
			return false;
		}
		
	}
}
