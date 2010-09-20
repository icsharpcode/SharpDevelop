// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using System.Windows.Forms;

using Hornung.ResourceToolkit.Gui;
using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Gui;

namespace Hornung.ResourceToolkit.CodeCompletion
{
	/// <summary>
	/// Provides a code completion item used to add a new string resource.
	/// </summary>
	public sealed class NewResourceCodeCompletionItem : ResourceCodeCompletionItem
	{
		readonly IResourceFileContent content;
		readonly string preEnteredName;
		
		public NewResourceCodeCompletionItem(IResourceFileContent content, IOutputAstVisitor outputVisitor, string preEnteredName)
			: base(StringParser.Parse("${res:Hornung.ResourceToolkit.CodeCompletion.AddNewEntry}"), String.Format(CultureInfo.CurrentCulture, StringParser.Parse("${res:Hornung.ResourceToolkit.CodeCompletion.AddNewDescription}"), content.FileName), outputVisitor)
		{
			this.content = content;
			this.preEnteredName = preEnteredName;
		}
		
		public override void Complete(CompletionContext context)
		{
			using (EditStringResourceDialog dialog = new EditStringResourceDialog(this.content, this.preEnteredName, null, true)) {
				dialog.Text = this.Description;
				if (dialog.ShowDialog(WorkbenchSingleton.MainWin32Window) != DialogResult.OK) {
					return;
				}
				
				this.content.Add(dialog.Key, dialog.Value);
				
				this.CompleteInternal(context, dialog.Key);
			}
		}
	}
}
