// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ServiceModel.Description;
using System.Xml.Schema;

using WSDescription = System.Web.Services.Description;
using WSDiscovery = System.Web.Services.Discovery;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class WebServiceMetadataSet : MetadataSet
	{
		public static readonly string EmptyMetadataIdentifier = String.Empty;
		
		public WebServiceMetadataSet(WSDiscovery.DiscoveryClientProtocol discoveryClient)
		{
			AddToMetadata(discoveryClient.Documents);
		}
		
		void AddToMetadata(WSDiscovery.DiscoveryClientDocumentCollection documents)
		{
			foreach (object document in documents.Values) {
				AddToMetadataIfNotNull(document as WSDescription.ServiceDescription);
				AddToMetadataIfNotNull(document as XmlSchema);
			}
		}
		
		void AddToMetadataIfNotNull(WSDescription.ServiceDescription serviceDescription)
		{
			if (serviceDescription != null) {
				AddMetadataSection(MetadataSection.ServiceDescriptionDialect, serviceDescription);
			}
		}
		
		void AddMetadataSection(string dialect, object metadata)
		{
			var metadataSection = new MetadataSection(dialect, EmptyMetadataIdentifier, metadata);
			MetadataSections.Add(metadataSection);
		}
		
		void AddToMetadataIfNotNull(XmlSchema schema)
		{
			if (schema != null) {
				AddMetadataSection(MetadataSection.XmlSchemaDialect, schema);
			}
		}
	}
}