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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Description;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceGenerator
	{
		IProjectWithServiceReferences project;
		IServiceReferenceFileGenerator fileGenerator;
		IServiceReferenceFileSystem fileSystem;
		IActiveTextEditors activeTextEditors;
		string tempAppConfigFileName;
		ServiceReferenceFileName referenceFileName;
		
		public ServiceReferenceGenerator(IProject project)
			: this(new ProjectWithServiceReferences(project))
		{
		}
		
		public ServiceReferenceGenerator(IProjectWithServiceReferences project)
			: this(
				project,
				new ServiceReferenceFileGenerator(),
				new ServiceReferenceFileSystem(),
				new ActiveTextEditors())
		{
		}
		
		public ServiceReferenceGenerator(
			IProjectWithServiceReferences project,
			IServiceReferenceFileGenerator fileGenerator,
			IServiceReferenceFileSystem fileSystem,
			IActiveTextEditors activeTextEditors)
		{
			this.project = project;
			this.fileGenerator = fileGenerator;
			this.fileSystem = fileSystem;
			this.activeTextEditors = activeTextEditors;
		}
		
		public ServiceReferenceGeneratorOptions Options {
			get { return fileGenerator.Options; }
			set { fileGenerator.Options = value; }
		}
		
		public event EventHandler<GeneratorCompleteEventArgs> Complete;
		
		void OnComplete(GeneratorCompleteEventArgs e)
		{
			if (Complete != null) {
				Complete(this, e);
			}
		}
		
		public void AddServiceReference()
		{
			referenceFileName = StartProxyFileGeneration();
		}
		
		ServiceReferenceFileName StartProxyFileGeneration()
		{
			ServiceReferenceFileName referenceFileName = project.GetServiceReferenceFileName(fileGenerator.Options.ServiceName);
			CreateFolderForFileIfFolderMissing(referenceFileName.Path);
			
			CreateTempAppConfigFileIfOpenInTextEditor();
			
			Options.OutputFileName = referenceFileName.Path;
			Options.AppConfigFileName = GetAppConfigFileName();
			Options.NoAppConfig = false;
			Options.MergeAppConfig = project.HasAppConfigFile();
			Options.MapProjectLanguage(project.Language);
			Options.GenerateNamespace(project.RootNamespace);
			Options.AddProjectReferencesIfUsingTypesFromProjectReferences(project.GetReferences());
			
			fileGenerator.Complete += ProxyFileGenerationComplete;
			fileGenerator.GenerateProxyFile();
			
			return referenceFileName;
		}
		
		string GetAppConfigFileName()
		{
			if (tempAppConfigFileName != null) {
				return tempAppConfigFileName;
			}
			return project.GetAppConfigFileName();
		}
		
		void CreateTempAppConfigFileIfOpenInTextEditor()
		{
			string appConfigText = activeTextEditors.GetTextForOpenFile(project.GetAppConfigFileName());
			if (appConfigText != null) {
				tempAppConfigFileName = fileSystem.CreateTempFile(appConfigText);
			}
		}
		
		ServiceReferenceMapFileName CreateServiceReferenceMapFile()
		{
			ServiceReferenceMapFileName mapFileName = project.GetServiceReferenceMapFileName(fileGenerator.Options.ServiceName);
			var mapFile = new ServiceReferenceMapFile(mapFileName);
			fileGenerator.GenerateServiceReferenceMapFile(mapFile);
			return mapFileName;
		}
		
		void CreateFolderForFileIfFolderMissing(string fileName)
		{
			string folder = Path.GetDirectoryName(fileName);
			fileSystem.CreateDirectoryIfMissing(folder);
		}
		
		void ProxyFileGenerationComplete(object sender, GeneratorCompleteEventArgs e)
		{
			if (e.IsSuccess) {
				UpdateProjectWithGeneratedServiceReference();
			}
			
			if (tempAppConfigFileName != null) {
				if (e.IsSuccess) {
					UpdateAppConfigInTextEditor();
				}
				DeleteTempAppConfigFile();
			}
			OnComplete(e);
		}
		
		void UpdateProjectWithGeneratedServiceReference()
		{
			ServiceReferenceMapFileName mapFileName = CreateServiceReferenceMapFile();
			project.AddServiceReferenceProxyFile(referenceFileName);
			project.AddServiceReferenceMapFile(mapFileName);
			
			project.AddAssemblyReference("System.Runtime.Serialization");
			project.AddAssemblyReference("System.ServiceModel");
			
			if (!project.HasAppConfigFile()) {
				project.AddAppConfigFile();
			}
			
			project.Save();
		}
		
		void DeleteTempAppConfigFile()
		{
			fileSystem.DeleteFile(tempAppConfigFileName);
		}
		
		void UpdateAppConfigInTextEditor()
		{
			string text = fileSystem.ReadAllFileText(tempAppConfigFileName);
			if (activeTextEditors.IsFileOpen(project.GetAppConfigFileName())) {
				activeTextEditors.UpdateTextForOpenFile(project.GetAppConfigFileName(), text);
			} else {
				fileSystem.WriteAllText(project.GetAppConfigFileName(), text);
			}
		}
		
		public IEnumerable<CheckableAssemblyReference> GetCheckableAssemblyReferences()
		{
			return GetUnsortedCheckableAssemblyReferences()
				.OrderBy(reference => reference.Description)
				.ToArray();
		}
		
		IEnumerable<CheckableAssemblyReference> GetUnsortedCheckableAssemblyReferences()
		{
			foreach (ReferenceProjectItem item in project.GetReferences()) {
				yield return new CheckableAssemblyReference(item);
			}
		}
		
		public void UpdateAssemblyReferences(IEnumerable<CheckableAssemblyReference> references)
		{
			Options.Assemblies.Clear();
			foreach (CheckableAssemblyReference reference in references) {
				if (reference.ItemChecked) {
					Options.Assemblies.Add(reference.GetFileName());
				}
			}
		}
	}
}
