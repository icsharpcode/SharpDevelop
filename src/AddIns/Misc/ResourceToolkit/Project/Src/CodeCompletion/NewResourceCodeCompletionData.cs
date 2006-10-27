// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Globalization;
using System.Windows.Forms;

using Hornung.ResourceToolkit.Gui;
using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

namespace Hornung.ResourceToolkit.CodeCompletion
{
	/// <summary>
	/// Provides a code completion entry used to add a new string resource.
	/// </summary>
	public class NewResourceCodeCompletionData : ResourceCodeCompletionData
	{
		
		readonly IResourceFileContent content;
		readonly string preEnteredName;
		
		public NewResourceCodeCompletionData(IResourceFileContent content, IOutputAstVisitor outputVisitor, string preEnteredName)
			: base(StringParser.Parse("${res:Hornung.ResourceToolkit.CodeCompletion.AddNewEntry}"), String.Format(CultureInfo.CurrentCulture, StringParser.Parse("${res:Hornung.ResourceToolkit.CodeCompletion.AddNewDescription}"), content.FileName), outputVisitor)
		{
			this.content = content;
			this.preEnteredName = preEnteredName;
		}
		
		/// <summary>
		/// Present a form to the user where he enters the name for the new
		/// string resource and then insert the key value into the text editor.
		/// </summary>
		/// <param name="textArea">TextArea to insert the completion data in.</param>
		/// <param name="ch">Character that should be inserted after the completion data.
		/// \0 when no character should be inserted.</param>
		/// <returns>Returns true when the insert action has processed the character
		/// <paramref name="ch"/>; false when the character was not processed.</returns>
		public override bool InsertAction(TextArea textArea, char ch)
		{
			
			EditStringResourceDialog dialog = new EditStringResourceDialog(this.content, this.preEnteredName, null, true);
			dialog.Text = this.Description;
			if (dialog.ShowDialog(WorkbenchSingleton.MainForm) != DialogResult.OK) {
				return false;
			}
			
			this.Text = dialog.Key;
			
			this.content.Add(dialog.Key, dialog.Value);
			
			return base.InsertAction(textArea, ch);
		}
		
	}
}
