// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.WixBinding
{
	public class ViewDialogXmlCommand : AbstractMenuCommand
	{
		public override void Run()
		{			
			// Get currently selected setup dialog.
			SetupDialogListViewItem selectedDialogListItem = SetupDialogListPad.Instance.SelectedDialog;
			if (selectedDialogListItem == null) {
				return;
			}
			
			SetupDialogErrorListViewItem errorDialogListItem = selectedDialogListItem as SetupDialogErrorListViewItem;
			if (errorDialogListItem == null) {
				ViewDialogXml(selectedDialogListItem.FileName, selectedDialogListItem.Id);
			} else {
				FileService.JumpToFilePosition(errorDialogListItem.FileName, errorDialogListItem.Line, errorDialogListItem.Column);
			}
		}
		
		static void ViewDialogXml(string fileName, string dialogId)
		{
			// Find dialog xml in text.
			Location location = GetDialogElementLocation(fileName, dialogId);
			
			// Jump to text.
			if (!location.IsEmpty) {
				FileService.JumpToFilePosition(fileName, location.Y, location.X);
			} else {
				MessageService.ShowErrorFormatted(StringParser.Parse("${res:ICSharpCode.WixBinding.ViewDialogXml.DialogIdNotFoundMessage}"), new string[] {dialogId, Path.GetFileName(fileName)});
			}
		}
		
		/// <summary>
		/// Gets the dialog element location given the filename and the dialog id.
		/// </summary>
		static Location GetDialogElementLocation(string fileName, string id)
		{
			try {
				WorkbenchTextFileReader workbenchTextFileReader = new WorkbenchTextFileReader();
				using (TextReader reader = workbenchTextFileReader.Create(fileName)) {
					WixDocumentReader wixReader = new WixDocumentReader(reader);
					return wixReader.GetStartElementLocation("Dialog", id);
				}
			} catch (XmlException ex) {
				WixBindingService.ShowErrorInErrorList(fileName, ex);
			}
			return Location.Empty;
		}
	}
}
