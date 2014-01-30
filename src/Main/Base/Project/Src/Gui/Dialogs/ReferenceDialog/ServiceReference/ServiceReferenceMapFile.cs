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
