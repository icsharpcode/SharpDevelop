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
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.WixBinding
{
	public class SetupDialogListPad : AbstractPadContent
	{
		SetupDialogListView setupDialogListView;
		bool disposed;
		static SetupDialogListPad instance;
		
		public SetupDialogListPad()
		{
			instance = this;
			
			setupDialogListView = new SetupDialogListView();
			setupDialogListView.ContextMenuStrip = MenuService.CreateContextMenu(setupDialogListView, "/SharpDevelop/Pads/WixSetupDialogListPad/ContextMenu");
			setupDialogListView.ItemActivate += SetupDialogListViewItemActivate;
			setupDialogListView.Enter += SetupDialogListViewEnter;
			
			// Show dialogs in currently open wix project.
			ShowDialogList();
			
			ProjectService.CurrentProjectChanged += CurrentProjectChanged;
		}
		
		static public SetupDialogListPad Instance {
			get {
				return instance;
			}
		}
	
		public override object Control {
			get {
				return setupDialogListView;
			}
		}
		
		public override void Dispose()
		{
			if (!disposed) {
				disposed = true;
				setupDialogListView.Dispose();
				setupDialogListView = null;
				ProjectService.CurrentProjectChanged -= CurrentProjectChanged;
			}
		}
		
		/// <summary>
		/// Opens the selected dialog and displays it in the designer.
		/// </summary>
		public void OpenSelectedDialog()
		{
			SetupDialogListViewItem selectedDialog = SelectedDialog;
			if (selectedDialog != null) {
				SetupDialogErrorListViewItem errorItem = selectedDialog as SetupDialogErrorListViewItem;
				if (errorItem == null) {
					OpenDialog(selectedDialog.FileName, selectedDialog.Id);
				} else {
					FileService.JumpToFilePosition(errorItem.FileName, errorItem.Line, errorItem.Column);
				}
			}
		}
		
		/// <summary>
		/// Gets the selected dialog list view item.
		/// </summary>
		public SetupDialogListViewItem SelectedDialog {
			get {
				if (setupDialogListView.SelectedItems.Count > 0) {
					return (SetupDialogListViewItem)(setupDialogListView.SelectedItems[0]);
				}
				return null;
			}
		}

		/// <summary>
		/// Adds all the dialog ids from all the files in the project to the list view.
		/// </summary>
		/// <remarks>
		/// If an error occurs an error item is added to the list. The error
		/// list is only cleared the first time an error occurs
		/// since there may be multiple errors, one in each Wix file.
		/// Also we do not clear the error list unless we have an error
		/// so any previously added errors from a build, for example, are not 
		/// cleared unless we have to.
		/// </remarks>
		void ShowDialogList()
		{
			// Make sure we do not leave any errors in the error list from a previous call.
			if (setupDialogListView.HasErrors) {
				WixBindingService.ClearErrorList();
			}

			setupDialogListView.Items.Clear();
			
			WixProject openWixProject = ProjectService.CurrentProject as WixProject;
			if (openWixProject != null) {
				bool clearedErrorList = false;
				foreach (FileProjectItem wixFile in openWixProject.WixFiles) {
					if (File.Exists(wixFile.FileName)) {
						try {
							AddDialogListItems(wixFile.FileName);
						} catch (XmlException ex) {
							// Clear the error list the first time only.
							if (!clearedErrorList) {
								clearedErrorList = true;
								WixBindingService.ClearErrorList();
							}
							setupDialogListView.AddError(wixFile.FileName, ex);
							WixBindingService.AddErrorToErrorList(wixFile.FileName, ex);
						}
					}
				}
				if (clearedErrorList) {
					WixBindingService.ShowErrorList();
				}
			}
		}
		
		/// <summary>
		/// Adds dialog ids to the list.
		/// </summary>
		void AddDialogListItems(string fileName)
		{
			WorkbenchTextFileReader workbenchTextFileReader = new WorkbenchTextFileReader();
			using (TextReader reader = workbenchTextFileReader.Create(fileName)) {
				WixDocumentReader wixReader = new WixDocumentReader(reader);
				setupDialogListView.AddDialogs(fileName, wixReader.GetDialogIds());
			}
		}
		
		void CurrentProjectChanged(object source, ProjectEventArgs e)
		{
			ShowDialogList();
		}

		void SetupDialogListViewItemActivate(object source, EventArgs e)
		{
			OpenSelectedDialog();
		}
		
		/// <summary>
		/// When the setup dialog list gets focus update the list of dialogs since
		/// the Wix document may have changed.
		/// </summary>
		void SetupDialogListViewEnter(object source, EventArgs e)
		{
			UpdateDialogList();
		}
		
		/// <summary>
		/// Opens the specified dialog id into the designer.
		/// </summary>
		static void OpenDialog(string fileName, string dialogId)
		{
			// Open the Wix file.
			IViewContent viewContent = FileService.OpenFile(fileName);
			
			// Show the designer.
			WixDialogDesigner designer = WixDialogDesigner.GetDesigner(viewContent);
			if (designer != null) {
				designer.OpenDialog(dialogId);
			} else {
				LoggingService.Debug("Could not open Wix dialog designer for: " + fileName);
			}
		}
		
		/// <summary>
		/// Updates the list if the Wix documents can be read otherwise the list
		/// items are unchanged for that document.
		/// </summary>
		void UpdateDialogList()
		{
			try {
				setupDialogListView.BeginUpdate();
				ShowDialogList();
			} finally {
				setupDialogListView.EndUpdate();
			}
		}
	}
}
