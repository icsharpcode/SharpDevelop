// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
