// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using ICSharpCode.Core;
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
			if (editor.SelectionLength == 0) {
				MessageService.ShowError("Nothing selected!");
				return false;
			}
			
			Location startLoc = editor.Document.OffsetToPosition(editor.SelectionStart);
			Location endLoc = editor.Document.OffsetToPosition(editor.SelectionStart + editor.SelectionLength);
			
			XElement selectedItem = (from item in document.Root.Descendants()
			                         where item.IsInRange(startLoc, endLoc) select item).FirstOrDefault();
			
			if (selectedItem == null || selectedItem.Name != XName.Get("Grid", CompletionDataHelper.WpfXamlNamespace)) {
				MessageService.ShowError("Please select a Grid!");
				return false;
			}
			
			EditGridColumnsAndRowsDialog dialog = new EditGridColumnsAndRowsDialog(selectedItem);
			
			if (dialog.ShowDialog() ?? false) {
				
			}
			
			return false;
		}
	}
}