// SharpDevelop samples
// Copyright (c) 2013, AlphaSierraPapa
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;
using ICSharpCode.SharpSnippetCompiler.Core;
using Microsoft.Win32;

namespace ICSharpCode.SharpSnippetCompiler
{
	public class MainViewModel : INotifyPropertyChanged
	{
		ObservableCollection<MainViewContent> files = new ObservableCollection<MainViewContent>();
		ObservableCollection<PadViewModel> pads = new ObservableCollection<PadViewModel>();
		
		public MainViewModel()
		{
			ExitCommand = new DelegateCommand(param => Exit());
			NewFileCommand = new DelegateCommand(param => NewFile());
			BuildCurrentCommand = new DelegateCommand(param => BuildCurrentProject());
			RunCommand = new DelegateCommand(param => Run());
			StopCommand = new DelegateCommand(param => StopDebugging());
			ContinueCommand = new DelegateCommand(param => ContinueDebugging());
			StepOverCommand = new DelegateCommand(param => StepOver());
			StepIntoCommand = new DelegateCommand(param => StepInto());
			StepOutCommand = new DelegateCommand(param => StepOut());
			AddReferencesCommand = new DelegateCommand(param => AddReferences());
			OpenFileCommand = new DelegateCommand(param => OpenFile());
			CloseFileCommand = new DelegateCommand(param => CloseFile());
		}
		
		public ICommand ExitCommand { get; private set; }
		public ICommand NewFileCommand { get; private set; }
		public ICommand BuildCurrentCommand { get; private set; }
		public ICommand RunCommand { get; private set; }
		public ICommand StopCommand { get; private set; }
		public ICommand ContinueCommand { get; private set; }
		public ICommand StepOverCommand { get; private set; }
		public ICommand StepIntoCommand { get; private set; }
		public ICommand StepOutCommand { get; private set; }
		public ICommand AddReferencesCommand { get; private set; }
		public ICommand OpenFileCommand { get; private set; }
		public ICommand CloseFileCommand { get; private set; }
		
		public ObservableCollection<MainViewContent> Files {
			get { return files; }
		}
		
		public ObservableCollection<PadViewModel> Pads {
			get { return pads; }
		}
		
		public MainViewContent SelectedFile { get; set; }
		
		public void AddInitialPads()
		{
			AddPad(typeof(ErrorListPad));
			AddPad(typeof(CompilerMessageView));
		}
		
		void AddPad(Type type)
		{
			PadDescriptor descriptor = WorkbenchSingleton.Workbench.GetPad(type);
			Pads.Add(new PadViewModel(descriptor));
		}
		
		public IViewContent LoadFile(string fileName)
		{
			var window = new WorkbenchWindow();
			var view = new MainViewContent(fileName, window);
			WorkbenchSingleton.Workbench.ShowView(view);
			
			UpdateActiveView(view);
			
			files.Add(view);
			
			return view;
		}
		
		void UpdateActiveView(IViewContent view)
		{
			Workbench workbench = WorkbenchSingleton.Workbench as Workbench;
			workbench.ActiveViewContent = view;
			workbench.ActiveContent = view;
		}
		
		void Exit()
		{
			SaveAll();
			Application.Current.Shutdown();
		}
		
		void SaveAll()
		{
			foreach (MainViewContent view in files) {
				view.Save();
			}
		}
		
		void NewFile()
		{
			using (var dialog = new NewFileDialog()) {
				dialog.FileName = GetNewFileName();
				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
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
		
		void AddFileToProject(string fileName)
		{
			IProject project = ProjectService.CurrentProject;
			var item = new FileProjectItem(project, ItemType.Compile, fileName);
			ProjectService.AddProjectItem(project, item);
			project.Save();
		}
		
		void BuildCurrentProject()
		{
			SaveAll();
			var buildSnippet = new BuildSnippetCommand(ProjectService.CurrentProject);
			buildSnippet.Run();
		}
		
		void Run()
		{
			SaveAll();
			var execute = new Execute();
			execute.Run();
		}
		
		void ContinueDebugging()
		{
			var continueCommand = new ContinueDebuggingCommand();
			continueCommand.Run();
		}
		
		void StopDebugging()
		{
			var stopCommand = new StopDebuggingCommand();
			stopCommand.Run();
		}
		
		void StepOver()
		{
			var stepCommand = new StepDebuggingCommand();
			stepCommand.Run();
		}
		
		void StepInto()
		{
			var stepCommand = new StepIntoDebuggingCommand();
			stepCommand.Run();
		}
		
		void StepOut()
		{
			var stepCommand = new StepOutDebuggingCommand();
			stepCommand.Run();
		}
		
		void AddReferences()
		{
			IProject project = ProjectService.CurrentProject;
			using (var referenceDialog = new SelectReferenceDialog(project)) {
				
				// Add existing project references to dialog.
				List<ReferenceProjectItem> references = GetReferences(project);
				AddReferences(referenceDialog as ISelectReferenceDialog, references);
				
				System.Windows.Forms.DialogResult result = referenceDialog.ShowDialog();
				if (result == System.Windows.Forms.DialogResult.OK) {
				
					ArrayList selectedReferences = referenceDialog.ReferenceInformations;
					
					// Remove any references removed in the select reference dialog.
					foreach (ReferenceProjectItem existingReference in references) {
						if (!selectedReferences.Contains(existingReference)) {
							ProjectService.RemoveProjectItem(project, existingReference);
						}
					}
					
					// Add new references.
					foreach (ReferenceProjectItem reference in referenceDialog.ReferenceInformations) {
						if (!references.Contains(reference)) {
							ProjectService.AddProjectItem(project, reference);
						}
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
		
		void OpenFile()
		{
			var dialog = new OpenFileDialog();
			dialog.CheckFileExists = true;
			if (dialog.ShowDialog() == true) {
				foreach (string fileName in dialog.FileNames) {
					LoadFile(fileName);
					AddFileToProject(fileName);
				}
			}
		}
		
		void CloseFile()
		{
			if (SelectedFile != null) {
				SelectedFile.Save();
				string fileName = SelectedFile.PrimaryFileName;
				IProject project = ProjectService.CurrentProject;
				FileProjectItem item = project.FindFile(fileName);
				if (item != null) {
					ProjectService.RemoveProjectItem(project, item);
					project.Save();
					
					files.Remove(SelectedFile);
				}
			}
		}
		
		public PadViewModel SelectedPad { get; set; }
		
		public void ActivateOutputList()
		{
			SelectedPad = pads[1];
			NotifyPropertyChanged("SelectedPad");
		}
		
		public void ActivateErrorList()
		{
			SelectedPad = pads[0];
			NotifyPropertyChanged("SelectedPad");
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		void NotifyPropertyChanged(string name)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
