// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Resources;
using System.Text;
using System.Collections.Specialized;
using System.Drawing.Design;
using System.ComponentModel.Design;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.FormDesigner.Services
{
	public class DesignerResourceService : System.ComponentModel.Design.IResourceService , IDisposable
	{
		IDesignerHost host;
		
		public string FileName  = String.Empty;
		public string NameSpace = String.Empty;
		public string RootType  = String.Empty;
		
		protected IProject project;
		
		protected Hashtable Readers = new Hashtable();
		protected Hashtable Writers = new Hashtable();
		
		#region ResourceStorage
		public class ResourceStorage
		{
			public MemoryStream stream  = null;
			public byte[]       storage = null;
			public IProject     project = null;
			
			public ResourceStorage(MemoryStream stream)
			{
				this.stream = stream;
			}

			public ResourceStorage(ResourceStorage rs)
			{
				this.storage = (byte []) rs.storage.Clone();
				this.stream = new MemoryStream(this.storage);
			}

			public void Dispose()
			{
				this.storage = null;
				this.stream.Close();	
			}

			public void Save(string fileName)
			{
				using (BinaryWriter binWriter = new BinaryWriter(System.IO.File.OpenWrite(fileName))) {
					binWriter.Write(this.storage);
				}
				
				// Add this resource file to the project
				if (project != null && !project.IsFileInProject(fileName)) {
//					ProjectFile fileInformation = ProjectService.AddFileToProject(project, fileName, BuildAction.EmbedAsResource);
//			TODO:	Project system...
//					ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ProjectBrowserView pbv = (ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ProjectBrowserView)WorkbenchSingleton.Workbench.GetPad(typeof(ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ProjectBrowserView));
//					pbv.UpdateCombineTree();
//					projectService.SaveCombine();
				}				
			}
		}
		#endregion
		
		// In ResourceMemoryStreams are stored:
		// Key: "true" file names from the project
		// Value: ResourceStorage, where the resources are stored
		// If the file is read, after
		// calculating of the "true" file name, it looks for MemoryStream
		// uses it if it exists.
		// Memory streams are cleared, when WriteSerialization will start
		// or File in the editor will be reloaded from the disc and of
		// course in Dispose of the service
		protected Hashtable resources      = null;
		public Hashtable Resources
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
		
		public DesignerResourceService(Hashtable resources)
		{
			project = ProjectService.CurrentProject;
			this.resources = resources;
		}
		
		#region System.ComponentModel.Design.IResourceService interface implementation
		public System.Resources.IResourceWriter GetResourceWriter(System.Globalization.CultureInfo info)
		{
			try {
				IResourceWriter resourceWriter = (IResourceWriter)Writers[info];
				string fileName = CalcResourceFileName(info);
				
				if (resourceWriter == null) {
					ResourceStorage resourceStorage = new ResourceStorage(new MemoryStream());
					resources[fileName] = resourceStorage;
					resourceWriter = new ResourceWriter(resourceStorage.stream);
					Writers[info] = resourceWriter;
					resourceStorage.project = project;
					
				}
				return resourceWriter;
			} catch (Exception e) {
				System.Windows.Forms.MessageBox.Show(e.ToString());
				return null;
			}
		}
		
		public System.Resources.IResourceReader GetResourceReader(System.Globalization.CultureInfo info)
		{
			try {
				string fileName = CalcResourceFileName(info);
				IResourceReader resourceReader = (IResourceReader)Readers[info];
				if (resourceReader == null) {
					if (resources != null && resources[fileName] != null) {
						MemoryStream stream = (MemoryStream) ((ResourceStorage)resources[fileName]).stream;
						stream.Seek( 0, System.IO.SeekOrigin.Begin );
						resourceReader = new ResourceReader( stream );
					} else if (File.Exists(fileName)) {
						resourceReader = new ResourceReader(fileName);
					}
					if (resourceReader != null) {
						Readers[info] = resourceReader;
					}
				}
				return resourceReader;
			} catch (Exception e) {
				System.Windows.Forms.MessageBox.Show(e.ToString());
				return null;
			}
		}
		#endregion

		public void Save()
		{
			if (resources != null) {
				foreach (DictionaryEntry entry in resources) {
					
					FileUtility.ObservedSave(new NamedFileOperationDelegate(((ResourceStorage)entry.Value).Save), (string) entry.Key, FileErrorPolicy.Inform);
				}
			}
		}

		protected string CalcResourceFileName(System.Globalization.CultureInfo info)
		{
			StringBuilder resourceFileName = null;
			if (FileName != null && FileName != String.Empty) {
				resourceFileName = new StringBuilder(Path.GetDirectoryName(FileName));
			} else if (project != null) {
				resourceFileName = new StringBuilder(project.Directory);
			} else {
				// required for untitled files. Untitled files should NOT save their resources.
				resourceFileName = new StringBuilder(Path.GetTempPath());
			}
			resourceFileName.Append(Path.DirectorySeparatorChar);
			resourceFileName.Append(host.RootComponentClassName);
			
			if (info != null && info.Name.Length > 0) {
				resourceFileName.Append('.');
				resourceFileName.Append(info.Name);
			}
			resourceFileName.Append(".resources");
			return resourceFileName.ToString();
		}

		public void SerializationStarted(bool serialize)
		{
			if (serialize == true) {
				if (resources == null) {
					resources = new Hashtable();
				}
				foreach (ResourceStorage storage in resources.Values) {
					storage.storage = null;
					storage.stream.Close();	
				}
				resources.Clear();
			} else {
				if (resources != null) {
					foreach (ResourceStorage storage in resources.Values) {
						storage.stream = new MemoryStream(storage.storage);
					}
				}
			}
		}
		
		public void SerializationEnded(bool serialize)
		{
			if (serialize == true && resources != null) {
				foreach (ResourceStorage storage in Resources.Values) {
					storage.storage = storage.stream.ToArray();
				}
			}
			
			foreach (IResourceWriter resourceWriter in Writers.Values) {
				if (resourceWriter != null) {
					resourceWriter.Close();
					resourceWriter.Dispose();
				}
			}
			Writers.Clear();
			
			foreach (IResourceReader resourceReader in Readers.Values) {
				if (resourceReader != null) {
					resourceReader.Close();
					resourceReader.Dispose();
				}
			}
			Readers.Clear();
		}
		
		public void Dispose()
		{
			if (resources != null) {
				foreach (ResourceStorage storage in resources.Values) {
					storage.Dispose();
				}
				resources.Clear();
			}
			SerializationEnded(false);
		}
	}
}
