// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
	/// <summary>
	/// Description of EditGridColumnsAndRowsCommand.
	/// </summary>
	public class EditGridColumnsAndRowsCommand : XamlMenuCommand
	{
		protected override bool Refactor(ITextEditor editor, XDocument document)
		{			
			Location startLoc = editor.Document.OffsetToPosition(editor.SelectionStart);
			Location endLoc = editor.Document.OffsetToPosition(editor.SelectionStart + editor.SelectionLength);
			
			XElement selectedItem = (from item in document.Root.Descendants()
			                         where item.IsInRange(startLoc, endLoc) select item).FirstOrDefault();
			
			if (selectedItem == null || selectedItem.Name != XName.Get("Grid", CompletionDataHelper.WpfXamlNamespace)) {
				MessageService.ShowError("Please select a Grid!");
				return false;
			}
			
			EditGridColumnsAndRowsDialog dialog = new EditGridColumnsAndRowsDialog(selectedItem);
			dialog.Owner = WorkbenchSingleton.MainWindow;
			
			if (dialog.ShowDialog() ?? false) {
				selectedItem.ReplaceWith(dialog.ConstructedTree);
				return true;
			}
			
			return false;
		}
	}
}