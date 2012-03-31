// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.ServiceModel.Description;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceGenerator
	{
		IProjectWithServiceReferences project;
		IServiceReferenceFileGenerator fileGenerator;
		IFileSystem fileSystem;
		
		public ServiceReferenceGenerator(IProject project)
			: this(new ProjectWithServiceReferences(project))
		{
		}
		
		public ServiceReferenceGenerator(IProjectWithServiceReferences project)
			: this(
				project,
				new ServiceReferenceFileGenerator(),
				new ServiceReferenceFileSystem())
		{
		}
		
		public ServiceReferenceGenerator(
			IProjectWithServiceReferences project,
			IServiceReferenceFileGenerator fileGenerator,
			IFileSystem fileSystem)
		{
			this.project = project;
			this.fileGenerator = fileGenerator;
			this.fileSystem = fileSystem;
		}
		
		public ServiceReferenceGeneratorOptions Options {
			get { return fileGenerator.Options; }
			set { fileGenerator.Options = value; }
		}
		
		public void AddServiceReference()
		{
			GenerateServiceReferenceProxy();
			project.AddAssemblyReference("System.ServiceModel");
			project.Save();
		}
		
		void GenerateServiceReferenceProxy()
		{
			ServiceReferenceFileName referenceFileName = GenerateProxyFile();
			ServiceReferenceMapFileName mapFileName = CreateServiceReferenceMapFile();
			project.AddServiceReferenceProxyFile(referenceFileName);
			project.AddServiceReferenceMapFile(mapFileName);
			if (!project.HasAppConfigFile()) {
				project.AddAppConfigFile();
			}
		}
		
		ServiceReferenceFileName GenerateProxyFile()
		{
			ServiceReferenceFileName referenceFileName = project.GetServiceReferenceFileName(fileGenerator.Options.Namespace);
			CreateFolderForFileIfFolderMissing(referenceFileName.Path);
			
			Options.OutputFileName = referenceFileName.Path;
			Options.AppConfigFileName = project.GetAppConfigFileName();
			Options.NoAppConfig = false;
			Options.MergeAppConfig = project.HasAppConfigFile();
			Options.MapProjectLanguage(project.Language);
			Options.AddProjectReferencesIfUsingTypesFromProjectReferences(project.GetReferences());
			fileGenerator.GenerateProxyFile();
			
			return referenceFileName;
		}
		
		ServiceReferenceMapFileName CreateServiceReferenceMapFile()
		{
			ServiceReferenceMapFileName mapFileName = project.GetServiceReferenceMapFileName(fileGenerator.Options.Namespace);
			var mapFile = new ServiceReferenceMapFile(mapFileName);
			fileGenerator.GenerateServiceReferenceMapFile(mapFile);
			return mapFileName;
		}
		
		void CreateFolderForFileIfFolderMissing(string fileName)
		{
			string folder = Path.GetDirectoryName(fileName);
			fileSystem.CreateDirectoryIfMissing(folder);
		}
	}
}
