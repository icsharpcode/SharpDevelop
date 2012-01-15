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
				new ServiceReferenceFileGenerator(project.CodeDomProvider),
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
		
		public string Namespace { get; set; }
		
		public void AddServiceReference(MetadataSet metadata)
		{
			GenerateServiceReferenceProxy(metadata);
			project.AddAssemblyReference("System.ServiceModel");
			project.Save();
		}
		
		void GenerateServiceReferenceProxy(MetadataSet metadata)
		{
			ServiceReferenceFileName referenceFileName = project.GetServiceReferenceFileName(Namespace);
			CreateFolderForFileIfFolderMissing(referenceFileName.Path);
			
			fileGenerator.GenerateProxyFile(metadata, referenceFileName.Path);
			
			ServiceReferenceMapFileName mapFileName = project.GetServiceReferenceMapFileName(Namespace);
			var mapFile = new ServiceReferenceMapFile(mapFileName);
			fileGenerator.GenerateServiceReferenceMapFile(mapFile);
			
			project.AddServiceReferenceProxyFile(referenceFileName);
			project.AddServiceReferenceMapFile(mapFileName);
		}
		
		void CreateFolderForFileIfFolderMissing(string fileName)
		{
			string folder = Path.GetDirectoryName(fileName);
			fileSystem.CreateDirectoryIfMissing(folder);
		}
	}
}
