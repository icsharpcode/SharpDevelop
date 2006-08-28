// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
		string formFileName;
		
		public string FormFileName {
			get {
				return formFileName;
			}
			set {
				formFileName = value;
			}
		}
		
		// Culture name (or empty string) => Resources
		Dictionary<string, DesignerResourceService.ResourceStorage> resources = new Dictionary<string, DesignerResourceService.ResourceStorage>();
		
		#region ResourceStorage
		public class ResourceStorage
		{
			MemoryStream stream;
			IResourceWriter writer;
			byte[] buffer;
			ResourceType type = ResourceType.Resx;
			
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
			public IResourceReader GetReader(string resourceFileName)
			{
				if (this.GetBuffer() == null) {
					if (File.Exists(resourceFileName)) {
						type = GetResourceType(resourceFileName);
						return CreateResourceReader(resourceFileName, type);
					} else {
						return null;
					}
				} else {
					return CreateResourceReader(new MemoryStream(this.buffer, false), type);
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
				this.writer = CreateResourceWriter(this.stream, type);
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
		// Key: Culture name (empty string for invariant culture)
		// Value: ResourceStorage, where the resources are stored
		// Memory streams are cleared, when WriteSerialization will start
		// or File in the editor will be reloaded from the disc and of
		// course in Dispose of the service
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

		public DesignerResourceService(string formFileName)
		{
			this.formFileName = formFileName;
		}

		static IProject GetProject(string formFileName)
		{
			if (ProjectService.OpenSolution != null && formFileName != null)
				return ProjectService.OpenSolution.FindProjectContainingFile(formFileName);
			else
				return null;
		}
		
		#region System.ComponentModel.Design.IResourceService interface implementation
		public System.Resources.IResourceWriter GetResourceWriter(CultureInfo info)
		{
			try {
				LoggingService.Debug("ResourceWriter requested for culture: " + info.ToString());
				ResourceStorage resourceStorage;
				if (resources.ContainsKey(info.Name)) {
					resourceStorage = resources[info.Name];
				} else {
					resourceStorage = new ResourceStorage();
					resources[info.Name] = resourceStorage;
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
				ResourceStorage resourceStorage;
				if (resources != null && resources.ContainsKey(info.Name)) {
					resourceStorage = resources[info.Name];
				} else {
					resourceStorage = new ResourceStorage();
					resources[info.Name] = resourceStorage;
				}
				return resourceStorage.GetReader(CalcResourceFileName(formFileName, info.Name));
			} catch (Exception e) {
				MessageService.ShowError(e);
				return null;
			}
		}
		#endregion

		public void Save(string formFileName)
		{
			this.formFileName = formFileName;
			if (resources != null) {
				foreach (KeyValuePair<string, ResourceStorage> entry in resources) {
					string cultureName = entry.Key;
					string resourceFileName = CalcResourceFileName(formFileName, cultureName);
					FileUtility.ObservedSave(new NamedFileOperationDelegate(entry.Value.Save), resourceFileName, FileErrorPolicy.Inform);
					
					IProject project = GetProject(formFileName);
					
					// Add this resource file to the project
					if (entry.Value.ContainsData && project != null && !project.IsFileInProject(resourceFileName)) {
						FileProjectItem newFileProjectItem = new FileProjectItem(project, ItemType.EmbeddedResource);
						newFileProjectItem.DependentUpon = Path.GetFileName(formFileName);
						newFileProjectItem.Include = FileUtility.GetRelativePath(project.Directory, resourceFileName);
						ProjectService.AddProjectItem(project, newFileProjectItem);
						
						PadDescriptor pd = WorkbenchSingleton.Workbench.GetPad(typeof(ProjectBrowserPad));
						FileNode formFileNode = ((ProjectBrowserPad)pd.PadContent).ProjectBrowserControl.FindFileNode(formFileName);
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

		protected static string CalcResourceFileName(string formFileName, string cultureName)
		{
			StringBuilder resourceFileName = null;
			IProject project = GetProject(formFileName);
			
			if (formFileName != null && formFileName != String.Empty) {
				resourceFileName = new StringBuilder(Path.GetDirectoryName(formFileName));
			} else if (project != null) {
				resourceFileName = new StringBuilder(project.Directory);
			} else {
				// required for untitled files. Untitled files should NOT save their resources.
				resourceFileName = new StringBuilder(Path.GetTempPath());
			}
			resourceFileName.Append(Path.DirectorySeparatorChar);
			string sourceFileName = null;
			if (project != null && formFileName != null) {
				// Try to find the source file name by using the project dependencies first.
				FileProjectItem sourceItem = project.Items.Find(delegate(ProjectItem item) {
				                                                	FileProjectItem fpi = item as FileProjectItem;
				                                                	return fpi != null && fpi.FileName != null && FileUtility.IsEqualFileName(fpi.FileName, formFileName);
				                                                }) as FileProjectItem;
				if (sourceItem != null && sourceItem.DependentUpon != null && sourceItem.DependentUpon.Length > 0) {
					sourceFileName = Path.GetFileNameWithoutExtension(sourceItem.DependentUpon);
				}
			}
			if (sourceFileName == null) {
				// If the source file name cannot be found using the project dependencies,
				// assume the resource file name to be equal to the current source file name.
				// Remove the ".Designer" part if present.
				sourceFileName = Path.GetFileNameWithoutExtension(formFileName);
				if (sourceFileName != null && sourceFileName.ToLowerInvariant().EndsWith(".designer")) {
					sourceFileName = sourceFileName.Substring(0, sourceFileName.Length - 9);
				}
			}
			resourceFileName.Append(sourceFileName);
			
			if (!string.IsNullOrEmpty(cultureName)) {
				resourceFileName.Append('.');
				resourceFileName.Append(cultureName);
			}
			
			// Use .resources filename if file exists.
			if (File.Exists(resourceFileName.ToString() + ".resources")) {
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
