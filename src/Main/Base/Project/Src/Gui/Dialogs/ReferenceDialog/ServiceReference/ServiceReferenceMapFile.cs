// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	[XmlRoot(ElementName = "ReferenceGroup", Namespace = "urn:schemas-microsoft-com:xml-wcfservicemap")]
	public class ServiceReferenceMapFile
	{
		ClientOptions clientOptions = new ClientOptions();
		List<MetadataSource> metadataSources = new List<MetadataSource>();
		List<MetadataFile> metadata = new List<MetadataFile>();
		
		public ServiceReferenceMapFile()
		{
			ID = Guid.NewGuid().ToString();
		}
		
		public ServiceReferenceMapFile(ServiceReferenceMapFileName fileName)
			: this()
		{
			FileName = fileName.Path;
		}
		
		public override bool Equals(object obj)
		{
			var rhs = obj as ServiceReferenceMapFile;
			return FileName == rhs.FileName;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public static ServiceReferenceMapFile CreateMapFileWithUrl(string url)
		{
			var mapFile = new ServiceReferenceMapFile();
			mapFile.AddMetadataSourceForUrl(url);
			return mapFile;
		}
		
		public void AddMetadataSourceForUrl(string url)
		{
			var metadataSource = new MetadataSource(url) { SourceId = "1" };
			metadataSources.Add(metadataSource);
		}
		
		[XmlIgnoreAttribute]
		public string FileName { get; set; }
		
		[XmlAttribute]
		public string ID { get; set; }
		
		public ClientOptions ClientOptions {
			get { return clientOptions; }
			set { clientOptions = value; }
		}
		
		public List<MetadataSource> MetadataSources { 
			get { return metadataSources; }
		}
		
		public 	List<MetadataFile> Metadata {
			get { return metadata; }
		}
	}
}
