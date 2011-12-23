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
		IServiceReferenceProxyGenerator proxyGenerator;
		IFileSystem fileSystem;
		
		public ServiceReferenceGenerator(IProject project)
			: this(new ProjectWithServiceReferences(project))
		{
		}
		
		public ServiceReferenceGenerator(IProjectWithServiceReferences project)
			: this(
				project,
				new ServiceReferenceProxyGenerator(project.CodeDomProvider),
				new ServiceReferenceFileSystem())
		{
		}
		
		public ServiceReferenceGenerator(
			IProjectWithServiceReferences project,
			IServiceReferenceProxyGenerator proxyGenerator,
			IFileSystem fileSystem)
		{
			this.project = project;
			this.proxyGenerator = proxyGenerator;
			this.fileSystem = fileSystem;
		}
		
		public string Namespace { get; set; }
		
		public void AddServiceReference(MetadataSet metadata)
		{
			GenerateServiceReferenceProxy(metadata);
		}
		
		void GenerateServiceReferenceProxy(MetadataSet metadata)
		{
			ServiceReferenceFileName fileName = project.GetServiceReferenceFileName(Namespace);
			CreateFolderForFileIfFolderMissing(fileName.Path);
			
			proxyGenerator.GenerateProxy(metadata, fileName.Path);
			
			project.AddServiceReferenceProxyFile(fileName);
			
			project.Save();
		}
		
		void CreateFolderForFileIfFolderMissing(string fileName)
		{
			string folder = Path.GetDirectoryName(fileName);
			fileSystem.CreateDirectoryIfMissing(folder);
		}
	}
}
