// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Collections.Specialized;
using System.Drawing.Design;
using System.ComponentModel.Design;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.FormsDesigner.Services
{
	public class DesignerResourceService : System.ComponentModel.Design.IResourceService , IDisposable
	{
		IDesignerHost host;
		
		string FileName  = String.Empty;
	
		#region ResourceStorage
		public class ResourceStorage
		{
			MemoryStream stream;
			IResourceWriter writer;

			public IProject     project = null;
			string fileName;
			byte[] buffer;		
			
			/// <summary>
			/// true, if the currently stored resource is not empty.
			/// Note that this property is only valid after at least one
			/// of GetReader, GetWriter or Save has been called.
			/// </summary>
			public bool ContainsData {
				get {
					return this.buffer != null;
				}
			}
			
			public ResourceStorage(string fileName, IProject project)
			{
				this.project = project;
				this.fileName = fileName;
			}

			public void Dispose()
			{
				if (this.stream != null) {
					this.writer.Dispose();
					this.stream.Dispose();
				}
				this.buffer = null;
			}
			
			/// <summary>
			/// Writes the byte array containing the most recent version of the resource
			/// represented by this instance into the private field "buffer" and returns it.
			/// Returns null, if this resource has not been written to yet.
			/// </summary>
			byte[] GetBuffer()
			{
				if (this.stream != null) {
					byte[] buffer = this.stream.ToArray();
					if (buffer.Length > 0) {
						this.writer.Close();
						this.writer.Dispose();
						this.buffer = this.stream.ToArray();
						this.writer = null;
						this.stream.Dispose();
						this.stream = null;
					}
				}
				return this.buffer;
			}
			
			/// <summary>
			/// Returns a new resource reader for this resource based on the most recent
			/// version available (either in memory or on disk).
			/// </summary>
			public IResourceReader GetReader()
			{
				if (this.GetBuffer() == null) {
					if (File.Exists(this.fileName)) {
						return CreateResourceReader(this.fileName, GetResourceType(this.fileName));
					} else {
						return null;
					}
				} else {
					return CreateResourceReader(new MemoryStream(this.buffer, false), GetResourceType(this.fileName));
				}
			}
			
			/// <summary>
			/// Returns a new resource writer for this resource.
			/// According to the SDK documentation of IResourceService.GetResourceWriter,
			/// a new writer needs to be returned every time one is requested, discarding any
			/// data written by previously returned writers.
			/// </summary>
			public IResourceWriter GetWriter()
			{
				this.stream = new MemoryStream();
				this.writer = CreateResourceWriter(this.stream, GetResourceType(this.fileName));
				return this.writer;
			}

			public void Save(string fileName)
			{
				if (this.GetBuffer() != null) {
					File.WriteAllBytes(fileName, this.buffer);
				}
			}
		}
		#endregion
		
		enum ResourceType {
			Resx = 0,
			Resources = 1
		};

		// In ResourceMemoryStreams are stored:
		// Key: "true" file names from the project
		// Value: ResourceStorage, where the resources are stored
		// If the file is read, after
		// calculating of the "true" file name, it looks for MemoryStream
		// uses it if it exists.
		// Memory streams are cleared, when WriteSerialization will start
		// or File in the editor will be reloaded from the disc and of
		// course in Dispose of the service
		protected Dictionary<string, ResourceStorage> resources = null;
		public Dictionary<string, ResourceStorage> Resources
		{
			get {
				return resources;
			}
			set {
				resources = value;
			}
		}
		public IDesignerHost Host {
			get {
				return host;
			}
			set {
				host = value;
			}
		}

		public DesignerResourceService(string fileName, Dictionary<string, ResourceStorage> resources)
		{
			this.FileName = fileName;
			this.resources = resources;
		}

		IProject _project;
		
		IProject GetProject()
		{
			if (_project == null && ProjectService.OpenSolution != null)
				_project = ProjectService.OpenSolution.FindProjectContainingFile(FileName);
			return _project;
		}
		
		#region System.ComponentModel.Design.IResourceService interface implementation
		public System.Resources.IResourceWriter GetResourceWriter(System.Globalization.CultureInfo info)
		{
			try {
				LoggingService.Debug("ResourceWriter requested for culture: "+info.ToString());
				string fileName = CalcResourceFileName(info);
				ResourceStorage resourceStorage;
				if (resources.ContainsKey(fileName)) {
					resourceStorage = resources[fileName];
				} else {
					resourceStorage = new ResourceStorage(fileName, GetProject());
					resources[fileName] = resourceStorage;
				}
				return resourceStorage.GetWriter();
			} catch (Exception e) {
				MessageService.ShowError(e);
				return null;
			}
		}

		public System.Resources.IResourceReader GetResourceReader(System.Globalization.CultureInfo info)
		{
			try {
				LoggingService.Debug("ResourceReader requested for culture: "+info.ToString());
				string fileName = CalcResourceFileName(info);
				ResourceStorage resourceStorage;
				if (resources != null && resources.ContainsKey(fileName)) {
					resourceStorage = resources[fileName];
				} else {
					resourceStorage = new ResourceStorage(fileName, GetProject());
					resources[fileName] = resourceStorage;
				}
				return resourceStorage.GetReader();
			} catch (Exception e) {
				MessageService.ShowError(e);
				return null;
			}
		}
		#endregion

		public void Save()
		{
			if (resources != null) {
				foreach (KeyValuePair<string, ResourceStorage> entry in resources) {
					string resourceFileName = entry.Key;
					FileUtility.ObservedSave(new NamedFileOperationDelegate(entry.Value.Save), resourceFileName, FileErrorPolicy.Inform);
					
					IProject project = GetProject();
					
					// Add this resource file to the project
					if (entry.Value.ContainsData && project != null && !project.IsFileInProject(resourceFileName)) {
						FileProjectItem newFileProjectItem = new FileProjectItem(project, ItemType.EmbeddedResource);
						newFileProjectItem.DependentUpon = Path.GetFileName(FileName);
						newFileProjectItem.Include = FileUtility.GetRelativePath(project.Directory, resourceFileName);
						ProjectService.AddProjectItem(project, newFileProjectItem);
						
						PadDescriptor pd = WorkbenchSingleton.Workbench.GetPad(typeof(ProjectBrowserPad));
						FileNode formFileNode = ((ProjectBrowserPad)pd.PadContent).ProjectBrowserControl.FindFileNode(FileName);
						if (formFileNode != null) {
							LoggingService.Info("FormFileNode found, adding subitem");
							FileNode fileNode = new FileNode(resourceFileName, FileNodeStatus.BehindFile);
							fileNode.AddTo(formFileNode);
							fileNode.ProjectItem = newFileProjectItem;
						}
						project.Save();
					}
				}
			}
		}

		protected string CalcResourceFileName(System.Globalization.CultureInfo info)
		{
			StringBuilder resourceFileName = null;
			IProject project = GetProject();
			
			if (FileName != null && FileName != String.Empty) {
				resourceFileName = new StringBuilder(Path.GetDirectoryName(FileName));
			} else if (project != null) {
				resourceFileName = new StringBuilder(project.Directory);
			} else {
				// required for untitled files. Untitled files should NOT save their resources.
				resourceFileName = new StringBuilder(Path.GetTempPath());
			}
			resourceFileName.Append(Path.DirectorySeparatorChar);
			resourceFileName.Append(host.RootComponent.Site.Name);
			
			if (info != null && info.Name.Length > 0) {
				resourceFileName.Append('.');
				resourceFileName.Append(info.Name);
			}
			
			// Use .resources filename if file exists.
			if (File.Exists(String.Concat(resourceFileName.ToString(), ".resources"))) {
				resourceFileName.Append(".resources");
			} else {
				resourceFileName.Append(".resx");
			}
			
			return resourceFileName.ToString();
		}
		
		public void Dispose()
		{
			if (resources != null) {
				foreach (ResourceStorage storage in resources.Values) {
					storage.Dispose();
				}
				resources.Clear();
			}
		}
		
		static ResourceType GetResourceType(string fileName)
		{
			if (Path.GetExtension(fileName).ToLowerInvariant() == ".resx") {
				return ResourceType.Resx;
			}
			return ResourceType.Resources;
		}
		
		static IResourceReader CreateResourceReader(string fileName, ResourceType type)
		{
			if (type == ResourceType.Resources) {
				return new ResourceReader(fileName);
			}
			return new ResXResourceReader(fileName);
		}
		
		static IResourceReader CreateResourceReader(Stream stream, ResourceType type)
		{
			if (type == ResourceType.Resources) {
				return new ResourceReader(stream);
			}
			return new ResXResourceReader(stream);
		}
		
		static IResourceWriter CreateResourceWriter(Stream stream, ResourceType type)
		{
			if (type == ResourceType.Resources) {
				return new ResourceWriter(stream);
			}
			return new ResXResourceWriter(stream);
		}
	}
}
