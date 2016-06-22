// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
