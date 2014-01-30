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
			TextLocation location = GetDialogElementLocation(fileName, dialogId);
			
			// Jump to text.
			if (!location.IsEmpty) {
				FileService.JumpToFilePosition(fileName, location.Line, location.Column);
			} else {
				MessageService.ShowErrorFormatted(StringParser.Parse("${res:ICSharpCode.WixBinding.ViewDialogXml.DialogIdNotFoundMessage}"), new string[] {dialogId, Path.GetFileName(fileName)});
			}
		}
		
		/// <summary>
		/// Gets the dialog element location given the filename and the dialog id.
		/// </summary>
		static TextLocation GetDialogElementLocation(string fileName, string id)
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
			return TextLocation.Empty;
		}
	}
}
