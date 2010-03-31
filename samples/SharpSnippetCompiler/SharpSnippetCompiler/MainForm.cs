// SharpDevelop samples
// Copyright (c) 2010, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;
using ICSharpCode.SharpSnippetCompiler.Core;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpSnippetCompiler
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}
		
		public Control ErrorList {
			get {
				if (errorsTabPage.Controls.Count > 0) {
					return errorsTabPage.Controls[0];
				}
				return null;
			}
			set {
				errorsTabPage.Controls.Clear();
				value.Dock = DockStyle.Fill;
				errorsTabPage.Controls.Add(value);
			}
		}

		public Control OutputList {
			get {
				if (outputTabPage.Controls.Count > 0) {
					return outputTabPage.Controls[0];
				}
				return null;
			}
			set {
				outputTabPage.Controls.Clear();
				value.Dock = DockStyle.Fill;
				outputTabPage.Controls.Add(value);
			}
		}
		
		/// <summary>
		/// Gets the active text editor control.
		/// </summary>
		public TextEditorControl TextEditor {
			get {
				return ActiveSnippetTabPage.SnippetCompilerControl.TextEditor;
			}
		}
		
		public SnippetTabPage ActiveSnippetTabPage {
			get { return fileTabControl.SelectedTab as SnippetTabPage; }
		}
		
		public IViewContent LoadFile(string fileName)
		{
			// Create a new tab page.
			SharpSnippetCompilerControl snippetControl = new SharpSnippetCompilerControl();
			snippetControl.Dock = DockStyle.Fill;
			SnippetTabPage tabPage = new SnippetTabPage(snippetControl);
			tabPage.Text = Path.GetFileName(fileName);

			fileTabControl.TabPages.Add(tabPage);
			
			// Load file
			snippetControl.LoadFile(fileName);
			snippetControl.Focus();
			
			WorkbenchWindow window = new WorkbenchWindow(fileTabControl, tabPage);
			MainViewContent view = new MainViewContent(fileName, snippetControl, window);
			WorkbenchSingleton.Workbench.ShowView(view);
			
			UpdateActiveView(view);
			
			return view;
		}
		
		public void ActivateErrorList()
		{
			tabControl.SelectedIndex = 0;
		}

		public void ActivateOutputList()
		{
			tabControl.SelectedIndex = 1;
		}
		
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			SaveAll();
			Close();
		}
		
		void BuildCurrentToolStripMenuItemClick(object sender, EventArgs e)
		{
			SaveAll();
			BuildSnippetCommand buildSnippet = new BuildSnippetCommand(ProjectService.CurrentProject);
			buildSnippet.Run();
		}
		
		void RunToolStripMenuItemClick(object sender, EventArgs e)
		{
			SaveAll();
			Execute execute = new Execute();
			execute.Run();
		}
		
		void ContinueToolStripMenuItemClick(object sender, EventArgs e)
		{
			ContinueDebuggingCommand continueCommand = new ContinueDebuggingCommand();
			continueCommand.Run();
		}
		
		void StepOverToolStripMenuItemClick(object sender, EventArgs e)
		{
			StepDebuggingCommand stepCommand = new StepDebuggingCommand();
			stepCommand.Run();
		}
		
		void StepIntoToolStripMenuItemClick(object sender, EventArgs e)
		{
			StepIntoDebuggingCommand stepCommand = new StepIntoDebuggingCommand();
			stepCommand.Run();
		}
		
		void StepOutToolStripMenuItemClick(object sender, EventArgs e)
		{
			StepOutDebuggingCommand stepCommand = new StepOutDebuggingCommand();
			stepCommand.Run();
		}
		
		void StopToolStripMenuItemClick(object sender, EventArgs e)
		{
			StopDebuggingCommand stopCommand = new StopDebuggingCommand();
			stopCommand.Run();
		}
		
		void UndoToolStripMenuItemClick(object sender, EventArgs e)
		{
			Undo undo = new Undo();
			undo.Run();
		}
		
		void RedoToolStripMenuItemClick(object sender, EventArgs e)
		{
			Redo redo = new Redo();
			redo.Run();
		}
		
		void CutToolStripMenuItemClick(object sender, EventArgs e)
		{
			Cut cut = new Cut();
			cut.Run();
		}
		
		void CopyToolStripMenuItemClick(object sender, EventArgs e)
		{
			Copy copy = new Copy();
			copy.Run();
		}
		
		void PasteToolStripMenuItemClick(object sender, EventArgs e)
		{
			Paste paste = new Paste();
			paste.Run();
		}
		
		void DeleteToolStripMenuItemClick(object sender, EventArgs e)
		{
			Delete delete = new Delete();
			delete.Run();
		}
		
		void SelectAllToolStripMenuItemClick(object sender, EventArgs e)
		{
			SelectAll selectAll = new SelectAll();
			selectAll.Run();
		}
		
		void ReferencesToolStripMenuItemClick(object sender, EventArgs e)
		{
			IProject project = ProjectService.CurrentProject;
			using (SelectReferenceDialog referenceDialog = new SelectReferenceDialog(project)) {
				
				// Add existing project references to dialog.
				List<ReferenceProjectItem> references = GetReferences(project);
				AddReferences(referenceDialog as ISelectReferenceDialog, references);

				DialogResult result = referenceDialog.ShowDialog();
				if (result == DialogResult.OK) {

					ArrayList selectedReferences = referenceDialog.ReferenceInformations;
					
					// Remove any references removed in the select reference dialog.
					foreach (ReferenceProjectItem existingReference in references) {
						if (!selectedReferences.Contains(existingReference)) {
							ProjectService.RemoveProjectItem(project, existingReference);
						}
					}
					
					// Add new references.
					foreach (ReferenceProjectItem reference in referenceDialog.ReferenceInformations) {
						ProjectService.AddProjectItem(project, reference);
					}
					project.Save();
				}
			}
		}
		
		List<ReferenceProjectItem> GetReferences(IProject project)
		{
			List<ReferenceProjectItem> references = new List<ReferenceProjectItem>();
			foreach (ProjectItem item in project.Items) {
				ReferenceProjectItem reference = item as ReferenceProjectItem;
				if (reference != null) {
					references.Add(reference);
				}
			}
			return references;
		}
		
		void AddReferences(ISelectReferenceDialog dialog, List<ReferenceProjectItem> references)
		{
			foreach (ReferenceProjectItem reference in references) {
				dialog.AddReference(reference.Include, "Gac", reference.FileName, reference);
			}
		}
		
		void UpdateActiveView(IViewContent view)
		{
			Workbench workbench = WorkbenchSingleton.Workbench as Workbench;
			workbench.ActiveViewContent = view;
			workbench.ActiveContent = view;
		}
		
		void FileTabControlSelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateActiveView();
		}

		void UpdateActiveView()
		{
			if (ActiveSnippetTabPage != null) {
				SharpSnippetCompilerControl control = ActiveSnippetTabPage.SnippetCompilerControl;
				foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection) {
					if (view.Control == control) {
						UpdateActiveView(view);
						return;
					}
				}
			} else {
				UpdateActiveView(null);
			}
		}

		void SaveAll()
		{
			foreach (SnippetTabPage tabPage in fileTabControl.TabPages) {
				tabPage.SnippetCompilerControl.Save();
			}
		}
		
		void FileNewToolStripMenuItemClick(object sender, EventArgs e)
		{
			using (NewFileDialog dialog = new NewFileDialog()) {
				dialog.FileName = GetNewFileName();
				if (dialog.ShowDialog() == DialogResult.OK) {
					string fileName = dialog.FileName;
					using (StreamWriter file = File.CreateText(fileName)) {
						file.Write(String.Empty);
					}
					LoadFile(fileName);
					AddFileToProject(fileName);
				}
			}
		}
		
		string GetNewFileName()
		{
			string fileName = SnippetCompilerProject.GetFullFileName("Snippet1.cs");
			string baseFolder = Path.GetDirectoryName(fileName);
			int count = 1;
			while (File.Exists(fileName)) {
				count++;
				fileName = Path.Combine(baseFolder, "Snippet" + count.ToString() + ".cs");
			}
			return fileName;
		}
		
		void FileOpenToolStripMenuItemClick(object sender, EventArgs e)
		{
			using (OpenFileDialog dialog = new OpenFileDialog()) {
				dialog.CheckFileExists = true;
				if (dialog.ShowDialog() == DialogResult.OK) {
					foreach (string fileName in dialog.FileNames) {
						LoadFile(fileName);
						AddFileToProject(fileName);
					}
				}
			}
		}
		
		void AddFileToProject(string fileName)
		{
			IProject project = ProjectService.CurrentProject;
			FileProjectItem item = new FileProjectItem(project, ItemType.Compile, fileName);
			ProjectService.AddProjectItem(project, item);
			project.Save();
		}
		
		void FileCloseToolStripMenuItemClick(object sender, EventArgs e)
		{
			SnippetTabPage activeTabPage = ActiveSnippetTabPage;
			if (activeTabPage != null) {
				SharpSnippetCompilerControl snippetControl = activeTabPage.SnippetCompilerControl;
				snippetControl.Save();
				string fileName = ActiveSnippetTabPage.SnippetCompilerControl.TextEditor.FileName;
				IProject project = ProjectService.CurrentProject;
				FileProjectItem item = project.FindFile(fileName);
				if (item != null) {
					ProjectService.RemoveProjectItem(project, item);
					project.Save();
					
					foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (view.Control == snippetControl) {
							WorkbenchSingleton.Workbench.CloseContent(view);
							break;
						}
					}
						
					fileTabControl.TabPages.Remove(activeTabPage);
					activeTabPage.Dispose();
				}
			}
		}
	}
}
