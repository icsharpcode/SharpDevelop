// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.ServiceModel.Description;
using System.Web.Services.Discovery;
using System.Xml.Schema;
using ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference;
using NUnit.Framework;
using WSDescription = System.Web.Services.Description;
using WSDiscovery = System.Web.Services.Discovery;

namespace ICSharpCode.SharpDevelop.Tests.ServiceReferences
{
	[TestFixture]
	public class WebServiceMetadataSetTests
	{
		DiscoveryClientProtocol discoveryProtocol;
		WebServiceMetadataSet metadata;
		
		void CreateDiscoveryProtocol()
		{
			discoveryProtocol = new DiscoveryClientProtocol();
		}
		
		WSDescription.ServiceDescription AddServiceDescriptionToDiscoveryProtocol()
		{
			var serviceDescription = new WSDescription.ServiceDescription();
			discoveryProtocol.Documents.Add("http://ServiceDescription", serviceDescription);
			return serviceDescription;
		}
		
		void CreateMetadata()
		{
			metadata = new WebServiceMetadataSet(discoveryProtocol);
		}
		
		XmlSchema AddXmlSchemaToDiscoveryProtocol()
		{
			var schema = new XmlSchema();
			discoveryProtocol.Documents.Add("http://XmlSchema", schema);
			return schema;
		}
		
		[Test]
		public void Constructor_DiscoveryProtocolHasOneServiceDescription_ServiceDescriptionAddedToMetadata()
		{
			CreateDiscoveryProtocol();
			WSDescription.ServiceDescription serviceDescription = 
				AddServiceDescriptionToDiscoveryProtocol();
			CreateMetadata();
			
			MetadataSection section = metadata.MetadataSections.First();
			
			Assert.AreEqual(serviceDescription, section.Metadata);
		}
		
		[Test]
		public void Constructor_DiscoveryProtocolHasOneServiceDescription_ServiceDescriptionMetadataHasServiceDescriptionDialect()
		{
			CreateDiscoveryProtocol();
			WSDescription.ServiceDescription serviceDescription = 
				AddServiceDescriptionToDiscoveryProtocol();
			CreateMetadata();
			
			MetadataSection section = metadata.MetadataSections.First();
			
			Assert.AreEqual(MetadataSection.ServiceDescriptionDialect, section.Dialect);
		}
		
		[Test]
		public void Constructor_DiscoveryProtocolHasOneXmlSchema_XmlSchemaAddedToMetadata()
		{
			CreateDiscoveryProtocol();
			XmlSchema schema = AddXmlSchemaToDiscoveryProtocol();
			CreateMetadata();
			
			MetadataSection section = metadata.MetadataSections.First();
			
			Assert.AreEqual(schema, section.Metadata);
		}
		
		[Test]
		public void Constructor_DiscoveryProtocolHasOneXmlSchema_XmlSchemaAddedToMetadataHasXmlSchemaDialect()
		{
			CreateDiscoveryProtocol();
			XmlSchema schema = AddXmlSchemaToDiscoveryProtocol();
			CreateMetadata();
			
			MetadataSection section = metadata.MetadataSections.First();
			
			Assert.AreEqual(MetadataSection.XmlSchemaDialect, section.Dialect);
		}
		
	}
}
