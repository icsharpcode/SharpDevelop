// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

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
				MessageService.ShowErrorFormatted("Unable to find '{0}' in {1}", new string[] {dialogId, Path.GetFileName(fileName)});
			}
		}
		
		/// <summary>
		/// Gets the dialog element location given the filename and the dialog id.
		/// </summary>
		static Location GetDialogElementLocation(string fileName, string id)
		{
			try {
				using (TextReader reader = WixBindingService.GetWixDocumentText(fileName)) {
					return WixDocument.GetDialogStartElementLocation(reader, id);
				}
			} catch (XmlException ex) {
				WixBindingService.ShowErrorInErrorList(fileName, ex);
			}
			return Location.Empty;
		}

	}
}
