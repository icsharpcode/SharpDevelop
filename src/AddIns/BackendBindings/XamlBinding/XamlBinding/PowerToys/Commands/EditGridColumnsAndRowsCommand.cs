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

using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XamlBinding.PowerToys.Dialogs;

namespace ICSharpCode.XamlBinding.PowerToys.Commands
{
	public class EditGridColumnsAndRowsCommand : XamlMenuCommand
	{
		protected override bool Refactor(ITextEditor editor, XDocument document)
		{
			Location startLoc = editor.Document.OffsetToPosition(editor.SelectionStart);
			Location endLoc = editor.Document.OffsetToPosition(editor.SelectionStart + editor.SelectionLength);
			
			XName[] names = CompletionDataHelper.WpfXamlNamespaces.Select(item => XName.Get("Grid", item)).ToArray();
			
			XElement selectedItem = document.Root.Descendants()
				.FirstOrDefault(item => item.IsInRange(startLoc, endLoc) && names.Any(ns => ns == item.Name))
				?? document.Root.Elements().FirstOrDefault(i => names.Any(ns => ns == i.Name));
			
			if (selectedItem == null) {
				MessageService.ShowError("Please select a Grid!");
				return false;
			}
			
			EditGridColumnsAndRowsDialog dialog = new EditGridColumnsAndRowsDialog(selectedItem);
			dialog.Owner = WorkbenchSingleton.MainWindow;
			
			if (dialog.ShowDialog() == true) {
				selectedItem.ReplaceWith(dialog.ConstructedTree);
				return true;
			}
			
			return false;
		}
	}
}
